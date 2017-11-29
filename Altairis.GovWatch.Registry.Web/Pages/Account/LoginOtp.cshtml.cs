using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Altairis.GovWatch.Registry.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Altairis.GovWatch.Registry.Web.Pages.Account {
    public class LoginOtpModel : PageModel {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginOtpModel(SignInManager<ApplicationUser> signInManager) {
            this._signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel {
            [Required]
            public string OtpCode { get; set; }

            public bool RememberDevice { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl) {
            var user = await this._signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null) return this.RedirectToPage("Login", new { returnUrl });
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = "/", bool rememberMe = false) {
            // Verify user is authenticated with password
            var user = await this._signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null) return this.RedirectToPage("Login", new { returnUrl });

            // Verify OTP code
            var otpCode = Regex.Replace(this.Input.OtpCode, @"[^\d]", "");
            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
                otpCode,
                rememberMe,
                this.Input.RememberDevice);

            // Redirect to target page when logged in
            if (result.Succeeded) return this.LocalRedirect(returnUrl);

            // Show error otherwise
            this.ModelState.AddModelError(string.Empty, "P�ihl�en� se nezda�ilo");
            return this.Page();
        }

    }
}