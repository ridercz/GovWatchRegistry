using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Altairis.GovWatch.Registry.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Altairis.GovWatch.Registry.Web.Pages.Admin.Categories {
    public class IndexModel : PageModel {
        private readonly RegistryDbContext _dc;

        public IndexModel(RegistryDbContext dc) {
            this._dc = dc;
        }

        public IEnumerable<Category> Data => _dc.Categories.OrderBy(x => x.Name).ToList();

    }
}