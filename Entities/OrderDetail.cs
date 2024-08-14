using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MansorySupplyHub.Dto;

namespace MansorySupplyHub.Entities
{
    public class OrderDetail : BaseEntity
    {
        public int OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public OrderHeader OrderHeader { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int Sqft { get; set; }
        public double PricePerSqFt { get; set; }
    }
}
