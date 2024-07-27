using MansorySupplyHub.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MansorySupplyHub.Dto
{
    public class InquiryHeaderDto
    {
        [Key]
        public int Id { get; set; }
        public Guid ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime InquiryDate { get; set; }
        [Required]
        public int PhoneNumber { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }

    }
}
