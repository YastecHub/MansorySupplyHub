using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MansorySupplyHub.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; } 

        [NotMapped]
        [Required]
        public string StreetAddress { get; set; }
        [NotMapped]
        [Required]
        public string City { get; set; }
        [NotMapped]
        [Required]
        public string State { get; set; }
        [NotMapped]
        [Required]
        public String PostalCode { get; set; }
    }
}

