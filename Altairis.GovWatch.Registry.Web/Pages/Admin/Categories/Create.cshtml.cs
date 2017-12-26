using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Altairis.GovWatch.Registry.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Altairis.GovWatch.Registry.Web.Pages.Admin.Categories {
    public class CreateModel : PageModel {
        private readonly RegistryDbContext _dc;

        public CreateModel(RegistryDbContext dc) {
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

        public async Task<IActionResult> OnPostAsync() {
            if (!this.ModelState.IsValid) return this.Page();

            var newCategory = new Category {
                Name = this.Input.Name,
                Comment = this.Input.Comment
            };
            _dc.Categories.Add(newCategory);
            await _dc.SaveChangesAsync();

            return this.RedirectToPage("Index");
        }

    }
}