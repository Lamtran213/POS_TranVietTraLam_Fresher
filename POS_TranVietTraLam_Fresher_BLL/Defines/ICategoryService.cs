using POS_TranVietTraLam_Fresher_BLL.DTO.CategoryDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_BLL.Defines
{
    public interface ICategoryService
    {
        Task<AddCategoryResponse> AddCategoryAsync(AddCategoryRequest addCategoryRequest);
    }
}
