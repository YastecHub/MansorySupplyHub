using MansorySupplyHub.Dto;
using MansorySupplyHub.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MansorySupplyHub.Implementation.Interface
{
    public interface IProductService
    {
        Task<ResponseModel<ProductDto>> CreateProduct(CreateProductDto request);
        Task<ResponseModel<ProductDto>> EditProduct(UpdateProductDto request, int id);
        Task<ResponseModel<bool>> DeleteProduct(int id);
        Task<ResponseModel<List<ProductDto>>> GetAllProducts();
        Task<ResponseModel<ProductDto>> GetProductDetails(int id);
        Task<ResponseModel<ProductDto>> GetProductForUpsert(int? id);
        Task<ResponseModel<ProductDto>> UpsertProduct(ProductDto productDto, IFormFileCollection files, string webRootPath);
        Task<IEnumerable<SelectListItem>> GetCategorySelectList();
        Task<IEnumerable<SelectListItem>> GetApplicationTypeSelectList();
    }
}
