using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MansorySupplyHub.Entities;
namespace MansorySupplyHub.Dto

{
    public class OrderDetailDto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public OrderHeaderDto OrderHeader { get; set; }


        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public ProductDto Product { get; set; }

        public int Sqft { get; set; }
        public double PricePerSqFt { get; set; }
    }


    public class CreateOrderDetailDto
    {
        [Required]
        public int OrderHeaderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Sqft { get; set; }

        [Required]
        public double PricePerSqFt { get; set; }
    }


    public class UpdateOrderDetailDto
    {
        [Required]
        public int OrderHeaderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Sqft { get; set; }

        [Required]
        public double PricePerSqFt { get; set; }
    }
}
