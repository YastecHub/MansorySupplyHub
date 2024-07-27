using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MansorySupplyHub.Models;

namespace MansorySupplyHub.Entities
{
    public class InquiryHeader : BaseEntity
    {
        public Guid ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime InquiryDate { get; set; }
        public int PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }


    }
}
