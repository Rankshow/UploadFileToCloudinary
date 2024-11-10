namespace UploadFileToCloudinary.Interface
{
    public interface ICloudinaryService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task<bool> DeleteFileAsync(string publicId);
    }

}
