using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MansorySupplyHub.Models
{
    public class OrderListVM
    {
        //public IEnumerable<OrderHeader> OrderHList { get; set; }
        //public IEnumerable<SelectListItem> StatusList { get; set; }

        public List<OrderHeaderDto> OrderHList { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public string Status { get; set; }
    }
}
