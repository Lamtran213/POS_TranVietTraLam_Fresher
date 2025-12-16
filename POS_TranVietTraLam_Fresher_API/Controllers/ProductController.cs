using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS_TranVietTraLam_Fresher_BLL.Defines;
using POS_TranVietTraLam_Fresher_BLL.DTO.CommonDTO;
using POS_TranVietTraLam_Fresher_BLL.DTO.ProductDTO;
using POS_TranVietTraLam_Fresher_BLL.Pagination;

namespace POS_TranVietTraLam_Fresher_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ApiResponse<AddProductResponse>> AddProduct([FromBody] AddProductRequest addProductRequest)
        {
            try
            {
                var response = await _productService.AddProductAsync(addProductRequest);
                return new ApiResponse<AddProductResponse>
                {
                    Success = true,
                    Message = "Product added successfully.",
                    Data = response,
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AddProductResponse>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                };
            }
        }

        [HttpGet]
        public async Task<ApiResponse<PagedResult<GetProductResponse>>> GetAllProducts(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var products = await _productService
                    .GetAllProductsAsync(pageIndex, pageSize);

                return new ApiResponse<PagedResult<GetProductResponse>>
                {
                    Success = true,
                    Message = "Products retrieved successfully.",
                    Data = products,
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<GetProductResponse>>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                };
            }
        }
    }
}
