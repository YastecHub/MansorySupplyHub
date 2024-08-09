using MansorySupplyHub.Dto;

namespace MansorySupplyHub.Models
{
    public class InquiryVM
    {
        public InquiryHeaderDto InquiryHeader { get; set; }
        public IEnumerable<InquiryDetailDto> InquiryDetail { get; set; }
    }
}
