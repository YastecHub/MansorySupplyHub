using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;

namespace MansorySupplyHub.Models
{
    public class OrderVM
    {
        public OrderHeaderDto OrderHeader { get; set; }
        public IEnumerable<OrderDetailDto> OrderDetail { get; set; }
    }
}
