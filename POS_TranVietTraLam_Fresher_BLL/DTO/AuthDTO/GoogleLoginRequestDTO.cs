using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_BLL.DTO.AuthDTO
{
    public class GoogleLoginRequestDTO
    {
        [Required]
        public string Uid { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
