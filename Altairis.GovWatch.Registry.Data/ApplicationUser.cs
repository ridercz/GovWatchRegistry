using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Altairis.GovWatch.Registry.Data {
    public class ApplicationUser : IdentityUser<int> {

        [Required, MaxLength(100)]
        public string DisplayName { get; set; }

        [MaxLength(64)]
        public string ApiKey { get; set; }

    }
}
