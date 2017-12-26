using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Altairis.ConventionalMetadataProviders;
using Altairis.GovWatch.Registry.Data;
using Altairis.GovWatch.Registry.Web.Services;
using Altairis.Services.Mailing;
using Altairis.Services.Mailing.Rfc2822;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Altairis.GovWatch.Registry.Web {
    public class Startup {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config) {
            this.Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services) {
            // Load configuration
            var appSettings = new AppSettings();
            this.Configuration.Bind(appSettings);
            services.Configure<AppSettings>(this.Configuration);

            // Configure database
            services.AddDbContext<RegistryDbContext>(options => {
                options.UseSqlServer(this.Configuration.GetConnectionString("RegistryDb"));
            });

            // Configure identity
            services.AddAuthorization(options => {
                options.AddPolicy("IsLoggedIn", policy => policy.RequireAuthenticatedUser());
                options.AddPolicy("IsAdministrator", policy => policy.RequireRole(ApplicationRole.AdministratorRoleName));
                options.AddPolicy("IsOperator", policy => policy.RequireRole(ApplicationRole.OperatorRoleName, ApplicationRole.AdministratorRoleName));
                options.AddPolicy("IsMonitor", policy => policy.RequireRole(ApplicationRole.MonitorRoleName));
            });
            services.AddIdentity<ApplicationUser, ApplicationRole>(options => {
                options.Password.RequiredLength = 12;
                options.Password.RequiredUniqueChars = 4;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<RegistryDbContext>().AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options => {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
            });

            // Configure MVC
            services.AddMvc(options => {
                options.SetConventionalMetadataProviders(typeof(Resources.ModelMetadata));
            }).AddRazorPagesOptions(options => {
                options.Conventions.AuthorizeFolder("/Admin", "IsLoggedIn");
                options.Conventions.AuthorizeFolder("/Admin/Sites", "IsOperator");
                options.Conventions.AuthorizeFolder("/Admin/Users", "IsAdministrator");
                options.Conventions.AuthorizeFolder("/Admin/Categories", "IsAdministrator");
            });

            // Configure other services
            services.AddSingleton<IDateProvider>(new DefaultDateProvider());
            services.AddPickupFolderMailerService(new PickupFolderMailerServiceOptions {
                PickupFolderName = appSettings.Mailing.PickupFolder,
                DefaultFrom = new MailAddressDto(appSettings.Mailing.SenderAddress, appSettings.Mailing.SenderName)
            });
            services.Configure<Altairis.TagHelpers.TimeTagHelperOptions>(options => {
                options.NullDateFormatter = () => "nikdy";
                options.TodayDateFormatter = value => $"dnes, {value:t}";
                options.YesterdayDateFormatter = value => $"včera, {value:t}";
                options.TomorrowDateFormatter = value => $"zítra, {value:t}";
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, RegistryDbContext dc, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) {
            // Configure database
            this.ConfigureDatabase(dc, userManager, roleManager).Wait();

            // Configure middleware
            app.UseRequestLocalization(new RequestLocalizationOptions {
                SupportedCultures = { new CultureInfo("cs-CZ") },
                SupportedUICultures = { new CultureInfo("cs-CZ") }
            });
            if (env.IsDevelopment()) {
                // Development
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseStaticFiles();
            }
            else {
                // Production
                app.UseStaticFiles(new StaticFileOptions {
                    OnPrepareResponse = context => {
                        context.Context.Response.Headers.Add("Cache-Control", "public,max-age=31536000");
                    }
                });
            }
            app.UseAuthentication();
            app.UseMvc();
        }

        public async Task ConfigureDatabase(RegistryDbContext dc, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) {
            // Migrate database to latest version
            await dc.Database.MigrateAsync();

            // Configure identity
            void EnsureIdentitySuccess(IdentityResult result) {
                if (result == IdentityResult.Success) return;
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                throw new Exception("Identity operation failed: " + errors);
            }

            // Create roles
            async Task EnsureRoleCreated(string roleName) {
                if (await roleManager.FindByNameAsync(roleName) != null) return;
                EnsureIdentitySuccess(await roleManager.CreateAsync(new ApplicationRole { Name = roleName }));
            }
            await EnsureRoleCreated(ApplicationRole.AdministratorRoleName);
            await EnsureRoleCreated(ApplicationRole.OperatorRoleName);
            await EnsureRoleCreated(ApplicationRole.MonitorRoleName);

            // Create admin user
            if (!userManager.Users.Any()) {
                var user = new ApplicationUser { UserName = "administrator", DisplayName = "Správce systému" };
                EnsureIdentitySuccess(await userManager.CreateAsync(user, "pass.word123"));
                EnsureIdentitySuccess(await userManager.AddToRoleAsync(user, ApplicationRole.AdministratorRoleName));
            }
        }

    }
}