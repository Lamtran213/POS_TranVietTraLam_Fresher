using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_BLL.DTO.CategoryDTO
{
    public class AddCategoryRequest
    {
        [StringLength(40)]
        public string? CategoryName { get; set; }

        public string Description { get; set; } = null!;
    }
}
