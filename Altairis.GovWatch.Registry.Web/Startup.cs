using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Altairis.GovWatch.Registry.Data;
using Altairis.GovWatch.Registry.Web.Services;
using Altairis.Services.Mailing;
using Altairis.Services.Mailing.Rfc2822;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
            })
                .AddEntityFrameworkStores<RegistryDbContext>()
                .AddDefaultTokenProviders();
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
            services.AddMvc().AddRazorPagesOptions(options => {
                options.Conventions.AuthorizeFolder("/My", "IsLoggedIn");
                options.Conventions.AuthorizeFolder("/WebSites", "IsOperator");
                options.Conventions.AuthorizeFolder("/Users", "IsAdministrator");
            });
            services.AddDbContext<RegistryDbContext>(options => {
                options.UseSqlServer(this.Configuration.GetConnectionString("RegistryDb"));
            });

            // Configure other services
            services.AddSingleton<IDateProvider>(new DefaultDateProvider());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
            app.UseStaticFiles();
        }

    }
}
