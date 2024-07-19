using System.ComponentModel.DataAnnotations;

namespace MansorySupplyHub.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
