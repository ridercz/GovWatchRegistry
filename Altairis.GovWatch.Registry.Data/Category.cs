using System.ComponentModel.DataAnnotations;

namespace Altairis.GovWatch.Registry.Data {
    public class Category {

        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string Comment { get; set; }

    }
}
