using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Security.Principal;
using UploadFileToCloudinary.Interface;

namespace UploadFileToCloudinary.Service
{
    public class CloudinaryService : ICloudinaryService
    {
            private readonly Cloudinary _cloudinary;


        public CloudinaryService(Cloudinary cloudinary) // Take a Cloudinary instance, not strings
        {
            _cloudinary = cloudinary;
        }

        // Upload file to Cloudinary
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is not valid.");
            }

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = Path.GetFileNameWithoutExtension(file.FileName),
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception($"File upload failed: {uploadResult.Error.Message}");
                }

                return uploadResult.Url?.ToString() ?? throw new Exception("File upload failed: No URL returned.");
            }
            // Delete file from Cloudinary
        }
            public async Task<bool> DeleteFileAsync(string publicId)
            {
                //publicId = GetPublicIdFromUri(publicId);
                var deletionParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deletionParams);


                return result.StatusCode == System.Net.HttpStatusCode.OK;
            }

        public static string GetPublicIdFromUri(string cloudinaryUri)
        {
            try
            {
                cloudinaryUri = Uri.UnescapeDataString(cloudinaryUri);
                // Parse the URI to get the path
                Uri uri = new Uri(cloudinaryUri);
                string path = uri.AbsolutePath;

                // Find the "/upload/" index and extract the public ID up to the file extension
                int uploadIndex = path.IndexOf("/upload/") + "/upload/".Length;
                int extensionIndex = path.LastIndexOf('.');
                if (uploadIndex < 0 || extensionIndex < 0 || extensionIndex <= uploadIndex)
                    throw new InvalidOperationException("Invalid Cloudinary URL format.");

                // Extract the publicId
                string publicId = path.Substring(uploadIndex, extensionIndex - uploadIndex);
                return publicId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting public ID: {ex.Message}");
                return null;
            }
        }
    }
}
