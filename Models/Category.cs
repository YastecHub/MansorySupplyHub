using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MansorySupplyHub.Models
{
    public class Category
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string Name { get; set; }

        [DisplayName("Display Order")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Display Order Must Be Greater tham 0")]
        public int DisplayOrder { get; set; }
    }
}
