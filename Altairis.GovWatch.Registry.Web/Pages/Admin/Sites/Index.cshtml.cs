using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Altairis.GovWatch.Registry.Data;
using Altairis.GovWatch.Registry.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Altairis.GovWatch.Registry.Web.Pages.Admin.Sites
{
    public class IndexModel : PagedPageModel<WebSite>
    {
        private readonly RegistryDbContext _dc;

        public IndexModel(RegistryDbContext dc) {
            this._dc = dc;
        }

        public async Task OnGetAsync(int pageNumber) {
            await base.GetData(_dc.WebSites.OrderBy(x=>x.Name), pageNumber, 20);
        }

    }
}