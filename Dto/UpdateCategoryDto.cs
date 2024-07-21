using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MansorySupplyHub.Dto
{
    public class UpdateCategoryDto
    {
        [Required]
        public string Name { get; set; }

        [DisplayName("Display Order")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Display Order Must Be Greater tham 0")]
        public int DisplayOrder { get; set; }

    }

}
