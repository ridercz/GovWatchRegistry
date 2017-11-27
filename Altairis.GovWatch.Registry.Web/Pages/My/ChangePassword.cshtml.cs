using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Altairis.GovWatch.Registry.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Altairis.GovWatch.Registry.Web.Pages.My {
    public class ChangePasswordModel : PageModel {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ChangePasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel {
            [Required, DataType(DataType.Password)]
            public string OldPassword { get; set; }

            [Required, DataType(DataType.Password), MinLength(12)]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Compare("NewPassword")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnPostAsync() {
            if (ModelState.IsValid) {
                // Get current user
                var user = await _userManager.GetUserAsync(this.User);

                // Try to change password
                var result = await _userManager.ChangePasswordAsync(
                    user,
                    this.Input.OldPassword,
                    this.Input.NewPassword);

                if (result.Succeeded) {
                    // OK, re-sign and redirect to homepage
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return this.RedirectToPage("ChangePasswordDone");
                }
                else {
                    // Failed - show why
                    foreach (var error in result.Errors) {
                        this.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }
            return this.Page();
        }
    }
}

