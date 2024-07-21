using System.ComponentModel.DataAnnotations;

namespace MansorySupplyHub.Dto
{
    public class UpdateApplicationTypeDto
    {
        [Required]
        public string Name { get; set; } 
    }
}
