using System.Linq;
using System.Threading.Tasks;
using Altairis.GovWatch.Registry.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Altairis.GovWatch.Registry.Web.Pages.Admin.Security {
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
            this.Message = "Zm�na e-mailu byla �sp�n� potvrzena.";
        }
        else {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            this.Message = "Nepoda�ilo se potvrdit zm�nu e-mailu: " + errors;
        }
    }
}
}