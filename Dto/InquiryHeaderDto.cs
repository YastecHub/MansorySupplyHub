using MansorySupplyHub.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MansorySupplyHub.Dto
{
    public class InquiryHeaderDto
    {
        [Key]
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime InquiryDate { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
    }


    public class CreateInquiryHeaderDto
    {
        [Required]
        public string ApplicationUserId { get; set; }

        [Required]
        public DateTime InquiryDate { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Full name must be between 3 and 100 characters.", MinimumLength = 3)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class UpdateInquiryHeaderDto
    {
        [Required]
        public int Id { get; set; }  // Assuming you'll use the Id to identify which record to update

        [Required]
        public string ApplicationUserId { get; set; }

        [Required]
        public DateTime InquiryDate { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Full name must be between 3 and 100 characters.", MinimumLength = 3)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
