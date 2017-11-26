using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Altairis.GovWatch.Registry.Data {
    public class WebSite {

        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string AltNames { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [ForeignKey("Category")]
        public int? CategoryId { get; set; }

        [Required, MaxLength(1000)]
        public string Url { get; set; }

        public string Comment { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [ForeignKey("CreatedById")]
        public ApplicationUser CreatedBy { get; set; }

        [Required, ForeignKey("CreatedBy")]
        public int CreatedById { get; set; }

        [Required]
        public DateTime DateUpdated { get; set; }

        [ForeignKey("UpdatedById")]
        public ApplicationUser UpdatedBy { get; set; }

        [Required, ForeignKey("UpdatedBy")]
        public int UpdatedById { get; set; }

        [Required]
        public WebSiteStatus  Status { get; set; }

        [Required]
        public WebSiteScope Scope { get; set; }

        [Range(1,10)]
        public int? Importance { get; set; }

        [DataType(DataType.EmailAddress), MaxLength(100)]
        public string OwnerEmailAddress { get; set; }

        [DataType(DataType.PhoneNumber), MaxLength(100)]
        public string OwnerPhoneNumber { get; set; }

        public string OwnerComment { get; set; }

    }
}
