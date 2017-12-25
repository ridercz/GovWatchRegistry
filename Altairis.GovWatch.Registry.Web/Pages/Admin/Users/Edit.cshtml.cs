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
    public class EditModel : PageModel {
        readonly UserManager<ApplicationUser> _userManager;

        public EditModel(UserManager<ApplicationUser> userManager) {
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

        }

        public async Task<IActionResult> OnGet(int userId) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return this.NotFound();

            this.Input = new InputModel {
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                Email = user.Email
            };

            if (await _userManager.IsInRoleAsync(user, ApplicationRole.MonitorRoleName)) this.Input.RoleName = ApplicationRole.MonitorRoleName;
            if (await _userManager.IsInRoleAsync(user, ApplicationRole.OperatorRoleName)) this.Input.RoleName = ApplicationRole.OperatorRoleName;
            if (await _userManager.IsInRoleAsync(user, ApplicationRole.AdministratorRoleName)) this.Input.RoleName = ApplicationRole.AdministratorRoleName;

            return this.Page();
        }

        public async Task<IActionResult> OnPost(int userId) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return this.NotFound();

            // Update general user properties
            user.UserName = this.Input.UserName;
            user.DisplayName = this.Input.DisplayName;
            user.Email = this.Input.Email;
            var result = await _userManager.UpdateAsync(user);
            if (!this.IsIdentitySuccess(result)) return Page();

            // Update user roles
            var oldRoles = await _userManager.GetRolesAsync(user);
            result = await _userManager.RemoveFromRolesAsync(user, oldRoles);
            if (!this.IsIdentitySuccess(result)) return Page();
            result = await _userManager.AddToRoleAsync(user, this.Input.RoleName);
            if (!this.IsIdentitySuccess(result)) return Page();

            return this.RedirectToPage("Index");
        }

    }
}