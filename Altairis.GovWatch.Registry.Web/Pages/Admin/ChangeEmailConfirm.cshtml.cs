using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Altairis.GovWatch.Registry.Data;

namespace Altairis.GovWatch.Registry.Web.Pages.Admin {
public class ChangeEmailConfirmModel : PageModel {
    private readonly UserManager<ApplicationUser> _userManager;

    public ChangeEmailConfirmModel(UserManager<ApplicationUser> userManager) {
        this._userManager = userManager;
    }

    public string Message { get; set; }

    public async Task OnGetAsync(string newEmail, string token) {
        // Get user
        var user = await this._userManager.GetUserAsync(this.User);

        // Try to change e-mail address
        var result = await this._userManager.ChangeEmailAsync(user, newEmail, token);
        if (result.Succeeded) {
            this.Message = "Zmìna e-mailu byla úspìšnì potvrzena.";
        }
        else {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            this.Message = "Nepodaøilo se potvrdit zmìnu e-mailu: " + errors;
        }
    }
}
}