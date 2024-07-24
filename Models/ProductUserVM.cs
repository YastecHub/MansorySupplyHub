using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;

namespace MansorySupplyHub.Models
{
    public class ProductUserVM
    {
        public ProductUserVM()
        {
            ProductList = new List<Product>();
        }

        public ApplicationUser ApplicationUser { get; set; }
        public IList<Product> ProductList { get; set; }
    }
}
