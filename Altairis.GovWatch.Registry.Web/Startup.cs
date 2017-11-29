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

            // Configure mailing
            services.AddPickupFolderMailerService(new PickupFolderMailerServiceOptions {
                PickupFolderName = appSettings.Mailing.PickupFolder,
                DefaultFrom = new MailAddressDto(appSettings.Mailing.SenderAddress, appSettings.Mailing.SenderName)
            });

            // Configure identity
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
            services.AddAuthorization(options => {
                options.AddPolicy("IsLoggedIn", policy => policy.RequireAuthenticatedUser());
                options.AddPolicy("IsAdministrator", policy => policy.RequireRole(ApplicationRole.AdministratorRoleName));
                options.AddPolicy("IsOperator", policy => policy.RequireRole(ApplicationRole.OperatorRoleName));
                options.AddPolicy("IsMonitor", policy => policy.RequireRole(ApplicationRole.MonitorRoleName));
            });

            // Configure system services
            services.AddMvc(options => {
                options.SetConventionalMetadataProviders(typeof(Resources.ModelMetadata));
            }).AddRazorPagesOptions(options => {
                options.Conventions.AuthorizeFolder("/Admin", "IsLoggedIn");
                options.Conventions.AuthorizeFolder("/Admin/Sites", "IsOperator");
                options.Conventions.AuthorizeFolder("/Admin/Users", "IsAdministrator");
            });
            services.AddDbContext<RegistryDbContext>(options => {
                options.UseSqlServer(this.Configuration.GetConnectionString("RegistryDb"));
            });

            // Configure other services
            services.AddSingleton<IDateProvider>(new DefaultDateProvider());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, RegistryDbContext dc, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) {
            // Configure database
            this.ConfigureDatabase(dc, userManager, roleManager).Wait();

            // Configure middleware
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.Use((context, next) => {
                CultureInfo.CurrentCulture = new CultureInfo("cs-CZ");
                CultureInfo.CurrentUICulture = new CultureInfo("cs-CZ");
                return next();
            });
            app.UseMvc();
            app.UseStaticFiles(new StaticFileOptions {
                OnPrepareResponse = context => {
                    context.Context.Response.Headers.Add("Cache-Control", "public,max-age=31536000");
                }
            });
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

            if (!userManager.Users.Any()) {
                var user = new ApplicationUser { UserName = "administrator", DisplayName = "Správce systému" };
                EnsureIdentitySuccess(await userManager.CreateAsync(user, "password"));
            }

            async void EnsureRoleCreated(string roleName) {
                if (await roleManager.FindByNameAsync(roleName) != null) return;
                EnsureIdentitySuccess(await roleManager.CreateAsync(new ApplicationRole { Name = roleName }));
            }
            EnsureRoleCreated(ApplicationRole.AdministratorRoleName);
            EnsureRoleCreated(ApplicationRole.OperatorRoleName);
            EnsureRoleCreated(ApplicationRole.MonitorRoleName);
        }

    }
}
