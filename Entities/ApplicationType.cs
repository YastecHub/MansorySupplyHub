using System.ComponentModel.DataAnnotations;

namespace MansorySupplyHub.Entities
{
    public class ApplicationType : BaseEntity
    {
        [Required]
        public string Name { get; set; }
    }
}
