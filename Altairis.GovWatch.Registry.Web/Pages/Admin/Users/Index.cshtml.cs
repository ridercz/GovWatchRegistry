using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Altairis.GovWatch.Registry.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Altairis.GovWatch.Registry.Web.Pages.Admin.Users {
    public class IndexModel : PageModel {
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager) {
            this._userManager = userManager;
        }

        public IEnumerable<ApplicationUser> Users { get; set; }


        public async Task OnGet() {
            this.Users = _userManager.Users.OrderBy(x => x.UserName);
        }
    }
}