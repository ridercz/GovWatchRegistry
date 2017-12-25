using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Altairis.GovWatch.Registry.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Altairis.GovWatch.Registry.Web.Pages.Admin.Users {
    public class CreateModel : PageModel {
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(UserManager<ApplicationUser> userManager) {
            this._userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IEnumerable<SelectListItem> RoleNames => new List<SelectListItem> {
            new SelectListItem { Text = ApplicationRole.MonitorRoleName},
            new SelectListItem { Text = ApplicationRole.OperatorRoleName},
            new SelectListItem { Text = ApplicationRole.AdministratorRoleName}
        };

        public class InputModel {

            [Required, MaxLength(100)]
            public string UserName { get; set; }

            [Required, MaxLength(100)]
            public string DisplayName { get; set; }

            [MaxLength(100), EmailAddress]
            public string Email { get; set; }

            public string RoleName { get; set; }

            [Required]
            public string NewPassword { get; set; }

        }

        public void OnGet() {
            this.Input = new InputModel {
                NewPassword = SecurityHelper.GenerateRandomPassword(20)
            };
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!this.ModelState.IsValid) return this.Page();

            var newUser = new ApplicationUser {
                UserName = this.Input.UserName,
                DisplayName = this.Input.DisplayName,
                Email = this.Input.Email,
                ApiKey = SecurityHelper.GenerateRandomPassword(64)
            };

            var result = await _userManager.CreateAsync(newUser, this.Input.NewPassword);
            if (!this.IsIdentitySuccess(result)) return Page();

            return this.RedirectToPage("Index");
        }

    }
}