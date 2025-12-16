namespace POS_TranVietTraLam_Fresher_BLL.Defines
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(string fileName, Stream fileStream);
    }
}
