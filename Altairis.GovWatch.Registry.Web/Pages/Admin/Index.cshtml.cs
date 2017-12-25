using System.Linq;
using Altairis.GovWatch.Registry.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Altairis.GovWatch.Registry.Web.Pages.Admin {
    public class IndexModel : PageModel
    {
        private RegistryDbContext _dc;

        public IndexModel(RegistryDbContext dc) {
            _dc = dc;
        }

        public int NumberOfSites => _dc.WebSites.Count();

        public int NumberOfUsers => _dc.Users.Count();

    }
}