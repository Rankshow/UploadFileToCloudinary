namespace UploadFileToCloudinary.Interface
{
    public interface ICloudStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string directory = "");
        Task<bool> DeleteFileAsync(string fileIdentifier);
        Task<byte[]> DownloadFileAsync(string fileIdentifier);
    }
}
