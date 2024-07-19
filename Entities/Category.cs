using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MansorySupplyHub.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
