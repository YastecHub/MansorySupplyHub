using System.ComponentModel.DataAnnotations;

namespace MansorySupplyHub.Dto
{
    public class ApplicationTypeDto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
