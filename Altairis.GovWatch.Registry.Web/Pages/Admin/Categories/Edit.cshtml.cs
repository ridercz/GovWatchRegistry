using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Altairis.GovWatch.Registry.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Altairis.GovWatch.Registry.Web.Pages.Admin.Categories {
    public class EditModel : PageModel {
        private readonly RegistryDbContext _dc;

        public EditModel(RegistryDbContext dc) {
            this._dc = dc;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel {
            [Required, MaxLength(100)]
            public string Name { get; set; }

            [DataType(DataType.MultilineText)]
            public string Comment { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int categoryId) {
            var c = await _dc.Categories.FindAsync(categoryId);
            if (c == null) return this.NotFound();

            this.Input = new InputModel {
                Name = c.Name,
                Comment = c.Comment
            };

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(int categoryId) {
            if (!this.ModelState.IsValid) return this.Page();

            var c = new Category {
                Id = categoryId,
                Name = this.Input.Name,
                Comment = this.Input.Comment
            };
            _dc.Categories.Update(c);
            await _dc.SaveChangesAsync();

            return this.RedirectToPage("Index");
        }
    }
}