using MansorySupplyHub.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MansorySupplyHub.Dto
{
    public class InquiryDetailDto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InquiryHeaderId { get; set; }
        [ForeignKey("InquiryHeaderId")]
        public InquiryHeader InquiryHeader { get; set; }

        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public ProductDto Product { get; set; }

    }

    public class CreateInquiryDetailDto
    {
        [Required]
        public int InquiryHeaderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public double Sqft { get; set; }


    }

    public class UpdateInquiryDetailDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int InquiryHeaderId { get; set; }

        [Required]
        public int ProductId { get; set; }
    }
}
