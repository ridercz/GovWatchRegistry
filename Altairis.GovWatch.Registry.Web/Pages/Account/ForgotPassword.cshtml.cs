using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Altairis.Services.Mailing;
using Altairis.GovWatch.Registry.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Altairis.GovWatch.Registry.Web.Pages.Account {
    public class ForgotPasswordModel : PageModel {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMailerService _mailerService;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IMailerService mailerService) {
            this._userManager = userManager;
            this._mailerService = mailerService;
        }

        [BindProperty, Required]
        public string UserName { get; set; }

        public async Task<IActionResult> OnPostAsync() {
            if (!this.ModelState.IsValid) return this.Page();

            // Try to find user by name
            var user = await this._userManager.FindByNameAsync(this.UserName);

            // Redirect to done page if user does not exist to block account enumeration
            if (user == null) return this.RedirectToPage("ForgotPasswordDone");

            // Get password reset token
            var token = await this._userManager.GeneratePasswordResetTokenAsync(user);

            // Get password reset URL
            var url = this.Url.Page("/Account/ResetPassword",
                pageHandler: null,
                values: new { userId = user.Id, token = token },
                protocol: this.Request.Scheme);

            // Send password reset mail
            var msg = new MailMessageDto {
                Subject = "Reset hesla",
                BodyText = "Heslo si m��ete zm�nit na n�sleduj�c� adrese:\r\n" + url
            };
            msg.To.Add(new MailAddressDto(user.Email));
            await this._mailerService.SendMessageAsync(msg);

            return this.RedirectToPage("ForgotPasswordDone");
        }

    }
}