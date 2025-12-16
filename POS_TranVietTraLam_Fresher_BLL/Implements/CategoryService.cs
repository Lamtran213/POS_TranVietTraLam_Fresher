using POS_TranVietTraLam_Fresher_BLL.Defines;
using POS_TranVietTraLam_Fresher_BLL.DTO.CategoryDTO;
using POS_TranVietTraLam_Fresher_DAL.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_BLL.Implements
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AddCategoryResponse> AddCategoryAsync(AddCategoryRequest addCategoryRequest)
        {
            var category = new POS_TranVietTraLam_Fresher_Entities.Entity.Category
            {
                CategoryName = addCategoryRequest.CategoryName,
                Description = addCategoryRequest.Description
            };
            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.Save();
            var response = new AddCategoryResponse
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description
            };
            return response;
        }
    }
}
