using Microsoft.AspNetCore.Mvc;
using POS_TranVietTraLam_Fresher_BLL.Defines;
using POS_TranVietTraLam_Fresher_BLL.DTO.StorageDTO;

namespace POS_TranVietTraLam_Fresher_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;
        public StorageController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] UploadFileRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest(new { message = "No file uploaded" });

            await using var stream = request.File.OpenReadStream();
            var url = await _storageService.UploadFileAsync(
                request.File.FileName,
                stream
            );

            return Ok(new
            {
                message = "Upload successful",
                fileName = request.File.FileName,
                url
            });
        }

    }
}
