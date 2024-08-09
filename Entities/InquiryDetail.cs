using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MansorySupplyHub.Entities
{
    public class InquiryDetail : BaseEntity
    {
        [Required]
        public int InquiryHeaderId { get; set; }
        [ForeignKey("InquiryHeaderId")]
        public InquiryHeader InquiryHeader { get; set; }

        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
