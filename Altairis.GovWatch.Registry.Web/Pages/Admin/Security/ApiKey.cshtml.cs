using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Altairis.GovWatch.Registry.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Altairis.GovWatch.Registry.Web.Pages.Admin.Security {
    public class ApiKeyModel : PageModel {
        private readonly UserManager<ApplicationUser> _userManager;

        public ApiKeyModel(UserManager<ApplicationUser> userManager) {
            _userManager = userManager;
        }

        public string Key { get; set; }

        public async Task OnGetAsync() {
            this.Key = await this.GetKey(reset: false);
        }

        public async Task<IActionResult> OnPostAsync() {
            await this.GetKey(reset: true);
            return this.RedirectToPage();
        }

        private async Task<string> GetKey(bool reset) {
            var currentUser = await _userManager.GetUserAsync(this.User);
            if (reset || string.IsNullOrWhiteSpace(currentUser.ApiKey)) {
                currentUser.ApiKey = SecurityHelper.GenerateRandomPassword(64);
                await _userManager.UpdateAsync(currentUser);
            }
            return currentUser.ApiKey;
        }

    }
}