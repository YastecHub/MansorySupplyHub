using MansorySupplyHub.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MansorySupplyHub.Models
{
    public class ProductUpsertViewModel
    {
        public ProductDto Product { get; set; }
        public IEnumerable<SelectListItem> CategorySelectList { get; set; }
        public IEnumerable<SelectListItem> ApplicationTypeSelectList { get; set; }
    }
}
