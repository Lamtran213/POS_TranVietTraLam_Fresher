using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS_TranVietTraLam_Fresher_BLL.DTO.StorageDTO
{
    public class UploadFileRequest
    {
        [FromForm(Name = "file")]
        public IFormFile File { get; set; } = default!;
    }
}
