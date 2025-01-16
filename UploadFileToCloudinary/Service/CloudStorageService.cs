using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using UploadFileToCloudinary.Interface;

namespace UploadFileToCloudinary.Service
{
    public class CloudStorageService : ICloudStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudStorageService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary ?? throw new ArgumentNullException(nameof(cloudinary));
        }

        /// <summary>
        /// Uploads a file to Cloudinary.
        /// </summary>
        public async Task<string> UploadFileAsync(IFormFile file, string directory = "")
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is not valid or is empty.", nameof(file));
            }

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = string.IsNullOrWhiteSpace(directory)
                        ? Path.GetFileNameWithoutExtension(file.FileName)
                        : $"{directory}/{Path.GetFileNameWithoutExtension(file.FileName)}",
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception($"File upload failed: {uploadResult.Error.Message}");
                }

                return uploadResult.Url?.ToString() ?? throw new Exception("File upload failed: No URL returned.");
            }
        }

        /// <summary>
        /// Deletes a file from Cloudinary.
        /// </summary>
        public async Task<bool> DeleteFileAsync(string fileIdentifier)
        {
            if (string.IsNullOrWhiteSpace(fileIdentifier))
            {
                throw new ArgumentException("File identifier cannot be null or empty.", nameof(fileIdentifier));
            }

            var deletionParams = new DeletionParams(fileIdentifier);
            var result = await _cloudinary.DestroyAsync(deletionParams);

            return result.StatusCode == System.Net.HttpStatusCode.OK;
        }

        /// <summary>
        /// Downloads a file from Cloudinary as a byte array.
        /// </summary>
        public async Task<byte[]> DownloadFileAsync(string fileIdentifier)
        {
            if (string.IsNullOrWhiteSpace(fileIdentifier))
            {
                throw new ArgumentException("File identifier cannot be null or empty.", nameof(fileIdentifier));
            }

            var url = _cloudinary.Api.UrlImgUp.BuildUrl(fileIdentifier);

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to download file: {response.ReasonPhrase}");
            }

            return await response.Content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// Extracts the public ID from a Cloudinary URL.
        /// </summary>
        public static string GetPublicIdFromUri(string cloudinaryUri)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cloudinaryUri))
                {
                    throw new ArgumentException("Cloudinary URI cannot be null or empty.", nameof(cloudinaryUri));
                }

                cloudinaryUri = Uri.UnescapeDataString(cloudinaryUri);
                Uri uri = new Uri(cloudinaryUri);
                string path = uri.AbsolutePath;

                int uploadIndex = path.IndexOf("/upload/") + "/upload/".Length;
                int extensionIndex = path.LastIndexOf('.');
                if (uploadIndex < 0 || extensionIndex < 0 || extensionIndex <= uploadIndex)
                    throw new InvalidOperationException("Invalid Cloudinary URL format.");

                return path.Substring(uploadIndex, extensionIndex - uploadIndex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting public ID: {ex.Message}");
                return null;
            }
        }
    }
}
