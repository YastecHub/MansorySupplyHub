namespace MansorySupplyHub.Dto
{
    public class ProductUserDto
    {
        public ApplicationUserDto ApplicationUser { get; set; }
        public IList<ProductDto> ProductList { get; set; }
      
        public ProductUserDto()
        {
            ProductList = new List<ProductDto>();
        }
    }
}
