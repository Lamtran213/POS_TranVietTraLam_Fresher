using POS_TranVietTraLam_Fresher_BLL.DTO.ProductDTO;
using POS_TranVietTraLam_Fresher_BLL.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_BLL.Defines
{
    public interface IProductService
    {
        Task<AddProductResponse> AddProductAsync(AddProductRequest addProductRequest);
        Task<PagedResult<GetProductResponse>> GetAllProductsAsync(int pageIndex, int pageSize);
    }
}
