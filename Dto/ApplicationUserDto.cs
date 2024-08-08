using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MansorySupplyHub.Dto
{
    public class ApplicationUserDto
    {
        public string Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }


        [NotMapped]
        [Required]
        public string StreetAddress { get; set; }
        [NotMapped]
        [Required]
        public string City { get; set; }
        [Required]
        [NotMapped]
        public string State { get; set; }
        [Required]
        [NotMapped]
        public String PostalCode { get; set; }
    }
}
