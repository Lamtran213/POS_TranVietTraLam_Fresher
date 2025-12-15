using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_BLL.DTO.CommonDTO
{
    public class EmailSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string ToEmail { get; set; } = string.Empty;
    }
}
