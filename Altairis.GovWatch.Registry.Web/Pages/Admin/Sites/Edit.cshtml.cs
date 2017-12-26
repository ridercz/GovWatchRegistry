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
using Microsoft.EntityFrameworkCore;

namespace Altairis.GovWatch.Registry.Web.Pages.Admin.Sites {
    public class EditModel : PageModel {
        private readonly RegistryDbContext _dc;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDateProvider _dateProvider;

        public EditModel(RegistryDbContext dc, UserManager<ApplicationUser> userManager, IDateProvider dateProvider) {
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

        public string CreatedBy { get; set; }

        public DateTime DateCreated { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime DateUpdated { get; set; }

        public IEnumerable<SelectListItem> CategoryList => _dc.Categories
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem {
                Text = x.Name,
                Value = x.Id.ToString()
            });

        public async Task<IActionResult> OnGetAsync(Guid siteId) {
            var site = await _dc.WebSites.Include(x => x.CreatedBy).Include(x => x.UpdatedBy).SingleOrDefaultAsync(x => x.Id == siteId);
            if (site == null) return this.NotFound();

            this.Input = new InputModel {
                AltNames = site.AltNames,
                CategoryId = site.CategoryId,
                Comment = site.Comment,
                Importance = site.Importance,
                Name = site.Name,
                OwnerComment = site.OwnerComment,
                OwnerEmailAddress = site.OwnerEmailAddress,
                OwnerPhoneNumber = site.OwnerPhoneNumber,
                Scope = site.Scope,
                Status = site.Status,
                Url = site.Url
            };

            this.CreatedBy = $"{site.CreatedBy.UserName} ({site.CreatedBy.DisplayName})";
            this.UpdatedBy = $"{site.UpdatedBy.UserName} ({site.UpdatedBy.DisplayName})";
            this.DateCreated = site.DateCreated;
            this.DateUpdated = site.DateUpdated;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid siteId) {
            if (!this.ModelState.IsValid) return this.Page();

            var site = await _dc.WebSites.FindAsync(siteId);
            if (site == null) return this.NotFound();

            var currentUserId = int.Parse(_userManager.GetUserId(this.User));

            site.AltNames = this.Input.AltNames;
            site.CategoryId = this.Input.CategoryId == 0 ? null : this.Input.CategoryId;
            site.Comment = this.Input.Comment;
            site.DateUpdated = _dateProvider.Now;
            site.Importance = this.Input.Importance;
            site.Name = this.Input.Name;
            site.OwnerComment = this.Input.OwnerComment;
            site.OwnerEmailAddress = this.Input.OwnerEmailAddress;
            site.OwnerPhoneNumber = this.Input.OwnerPhoneNumber;
            site.Scope = this.Input.Scope;
            site.Status = this.Input.Status;
            site.UpdatedById = currentUserId;
            site.Url = this.Input.Url;

            _dc.WebSites.Update(site);
            await _dc.SaveChangesAsync();

            return this.RedirectToPage("Index");
        }

    }

}