using System.ComponentModel.DataAnnotations;

namespace MansorySupplyHub.Dto
{
    public class CreateApplicationTypeDto
    {
        [Required]
        public string Name { get; set; } 
    }
}
