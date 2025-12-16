using POS_TranVietTraLam_Fresher_BLL.Defines;
using Supabase;
using Microsoft.Extensions.Logging;

namespace POS_TranVietTraLam_Fresher_BLL.Implements
{
    public class SupabaseStorageService : IStorageService
    {
        private readonly Client _supabase;
        private readonly ILogger<SupabaseStorageService> _logger;

        private const string BucketName = "POS.Lamtran213";

        public SupabaseStorageService(
            Client supabase,
            ILogger<SupabaseStorageService> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        public async Task<string> UploadFileAsync(string fileName, Stream fileStream)
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("File stream is empty");

            fileName = string.IsNullOrWhiteSpace(fileName)
                ? Guid.NewGuid().ToString()
                : fileName;

            var remotePath = $"product/{fileName}";

            byte[] fileBytes;
            await using (var ms = new MemoryStream())
            {
                await fileStream.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }

            await _supabase.Storage
                .From(BucketName)
                .Upload(
                    fileBytes,
                    remotePath,
                    onProgress: (_, progress) =>
                        _logger.LogInformation($"Uploading {fileName}: {progress}%")
                );

            var publicUrl = _supabase.Storage
                .From(BucketName)
                .GetPublicUrl(remotePath);

            return publicUrl;
        }
    }
}
