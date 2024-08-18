using MansorySupplyHub.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MansorySupplyHub.Dto
{
    public class ProductDto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        [Range(1, int.MaxValue)]
        public double Price { get; set; }
        public double TotalPrice { get; set; }

        public string Image { get; set; }

        [Display(Name = "CategoryType")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [Display(Name = "Application Type")]
        public int ApplicationTypeId { get; set; }
        [ForeignKey("ApplicationTypeId")]
        public virtual ApplicationType ApplicationType { get; set; }


        [NotMapped]
        [Range(1, 100000)]
        public int TempSqft { get; set; }
    }
}
