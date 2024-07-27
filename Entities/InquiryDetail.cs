using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MansorySupplyHub.Entities
{
    public class InquiryDetail : BaseEntity
    {
        public Guid InquiryHeaderId { get; set; }
        public InquiryHeader InquiryHeader { get; set; }
        public int ProductId { get; set; } 
        public Product Product { get; set; }
    }
}
