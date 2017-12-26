using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Altairis.GovWatch.Registry.Data;
using Altairis.GovWatch.Registry.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Altairis.GovWatch.Registry.Web.Pages.Admin.Sites {
    public class CreateModel : PageModel {
        private readonly RegistryDbContext _dc;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDateProvider _dateProvider;

        public CreateModel(RegistryDbContext dc, UserManager<ApplicationUser> userManager, IDateProvider dateProvider) {
            this._dc = dc;
            this._userManager = userManager;
            this._dateProvider = dateProvider;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel {
            [Required, MaxLength(100)]
            public string Name { get; set; }

            public string AltNames { get; set; }

            public int? CategoryId { get; set; }

            [Required, MaxLength(1000)]
            public string Url { get; set; }

            public string Comment { get; set; }

            [Required]
            public WebSiteStatus Status { get; set; } = WebSiteStatus.Active;

            [Required]
            public WebSiteScope Scope { get; set; } = WebSiteScope.Unknown;

            [Range(1, 10)]
            public int? Importance { get; set; }

            [DataType(DataType.EmailAddress), MaxLength(100)]
            public string OwnerEmailAddress { get; set; }

            [DataType(DataType.PhoneNumber), MaxLength(100)]
            public string OwnerPhoneNumber { get; set; }

            [DataType(DataType.MultilineText)]
            public string OwnerComment { get; set; }

        }

        public IEnumerable<SelectListItem> CategoryList => _dc.Categories
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem {
                Text = x.Name,
                Value = x.Id.ToString()
            });

        public async Task<IActionResult> OnPostAsync() {
            if (!this.ModelState.IsValid) return this.Page();

            var currentUserId = int.Parse(_userManager.GetUserId(this.User));
            var currentTime = _dateProvider.Now;

            var newSite = new WebSite {
                AltNames = this.Input.AltNames,
                CategoryId = this.Input.CategoryId == 0 ? null : this.Input.CategoryId,
                Comment = this.Input.Comment,
                CreatedById = currentUserId,
                DateCreated = currentTime,
                DateUpdated = currentTime,
                Importance = this.Input.Importance,
                Name = this.Input.Name,
                OwnerComment = this.Input.OwnerComment,
                OwnerEmailAddress = this.Input.OwnerEmailAddress,
                OwnerPhoneNumber = this.Input.OwnerPhoneNumber,
                Scope = this.Input.Scope,
                Status = this.Input.Status,
                UpdatedById = currentUserId,
                Url = this.Input.Url
            };

            _dc.WebSites.Add(newSite);
            await _dc.SaveChangesAsync();

            return this.RedirectToPage("Index");
        }

    }
}