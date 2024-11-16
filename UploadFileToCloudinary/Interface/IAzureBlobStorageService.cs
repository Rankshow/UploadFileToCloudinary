namespace UploadFileToCloudinary.Interface;

public interface IAzureBlobStorageService
{
    Task<string> UploadFileAsync(IFormFile file, string directory);
    Task<bool> DeleteFileAsync(string blobName);
    Task<byte[]> DownloadFileAsync(string blobName);
}
