using System;
using System.Linq;
using Altairis.GovWatch.Registry.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Altairis.GovWatch.Registry.Web.Pages.Admin {
    public class IndexModel : PageModel {

        public IndexModel(RegistryDbContext dc, IHostingEnvironment env) {
            this.NumberOfSites = dc.WebSites.Count();
            this.NumberOfUsers = dc.Users.Count();
            this.EnvironmentName = env.EnvironmentName;
        }

        public int NumberOfSites { get; set; }

        public int NumberOfUsers { get; set; }

        public string EnvironmentName { get; set; }

    }
}