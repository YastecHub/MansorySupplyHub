using MansorySupplyHub.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MansorySupplyHub.Models.ViewModel
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> CategorySelectList { get; set; }
    }
}
