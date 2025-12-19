using Microsoft.AspNetCore.Mvc;
using POS_TranVietTraLam_Fresher_BLL.Constants;
using POS_TranVietTraLam_Fresher_BLL.Defines;
using POS_TranVietTraLam_Fresher_BLL.DTO.CategoryDTO;
using POS_TranVietTraLam_Fresher_BLL.DTO.CommonDTO;

namespace POS_TranVietTraLam_Fresher_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("add")]
        public async Task<ApiResponse<AddCategoryResponse>> AddCategory([FromBody] AddCategoryRequest addCategoryRequest)
        {
            try
            {
                var response = await _categoryService.AddCategoryAsync(addCategoryRequest);
                if (response != null)
                {
                    return new ApiResponse<AddCategoryResponse>
                    {
                        Success = true,
                        Message = CategoryMessage.CategoryAddedSuccess,
                        Data = response,
                        Timestamp = DateTime.UtcNow
                    };
                }
                else
                {
                    return new ApiResponse<AddCategoryResponse>
                    {
                        Success = false,
                        Message = CategoryMessage.CategoryAddFailed,
                        Data = null,
                        Timestamp = DateTime.UtcNow
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<AddCategoryResponse>
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
