using MansorySupplyHub.Entities;

namespace MansorySupplyHub.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }

    }
}
