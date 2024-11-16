using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UploadFileToCloudinary.Interface;

namespace UploadFileToCloudinary.Controllers
{
    /// <summary>
    /// Controller for handling document-related operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        readonly ICloudinaryService _cloudinaryService;
        readonly IAzureBlobStorageService _blobStorageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentController"/> class.
        /// </summary>
        /// <param name="cloudinaryService">The service for managing documents in Cloudinary.</param>
        /// <param name="blobStorageService">The service for handling Azure Blob Storage operations.</param>
        public DocumentController(ICloudinaryService cloudinaryService, IAzureBlobStorageService blobStorageService)
        {
            _cloudinaryService = cloudinaryService;
            _blobStorageService = blobStorageService;
        }

        /// <summary>
        /// Endpoint to upload a file to Cloudinary.
        /// </summary>
        /// <param name="file">The file to be uploaded. It must be a non-null and non-empty multipart/form-data file.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the outcome of the operation:
        /// <list type="bullet">
        /// <item>
        /// <description>A <see cref="BadRequestResult"/> with an error message if the file is null or empty.</description>
        /// </item>
        /// <item>
        /// <description>An <see cref="OkObjectResult"/> with a success message and the public ID if the upload is successful.</description>
        /// </item>
        /// <item>
        /// <description>A <see cref="BadRequestResult"/> with an error message if the upload fails.</description>
        /// </item>
        /// </list>
        /// </returns>
        [Route("api/UploadToFileCloudinary")]
        [HttpPost]
        public async Task<IActionResult> UploadToFileCloudinary(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded." });
            }

            var publicId = await _cloudinaryService.UploadFileAsync(file);

            if (publicId != null)
            {
                return Ok(new { message = "File uploaded successfully.", publicId });
            }

            return BadRequest(new { message = "File upload failed." });
        }
        /// <summary>
        /// Deletes a file from Cloudinary using its public ID.
        /// </summary>
        /// <param name="publicId">The public ID of the file to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the outcome of the operation:
        /// <list type="bullet">
        /// <item>
        /// <description>An <see cref="OkObjectResult"/> with a success message if the file is deleted successfully.</description>
        /// </item>
        /// <item>
        /// <description>A <see cref="BadRequestObjectResult"/> with an error message if the file deletion fails.</description>
        /// </item>
        /// </list>
        /// </returns>
        // Endpoint to delete file
        [Route("api/DeleteFileFromCloudinary/{publicId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteFileFromCloudinary(string publicId)
        {
            var result = await _cloudinaryService.DeleteFileAsync(publicId);

            if (result)
            {
                return Ok(new { message = "File deleted successfully." });
            }
            else
            {
                return BadRequest(new { message = "File deletion failed." });
            }
        }

        /// <summary>
        /// Uploads a file to Azure Blob Storage.
        /// </summary>
        /// <param name="file">The file to be uploaded. It must be a non-null and non-empty multipart/form-data file.</param>
        /// <param name="directory">The directory in Azure Blob Storage where the file will be uploaded.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the outcome of the operation:
        /// <list type="bullet">
        /// <item>
        /// <description>A <see cref="BadRequestResult"/> if the file is null or empty.</description>
        /// </item>
        /// <item>
        /// <description>An <see cref="OkObjectResult"/> with a success message and the blob URL if the upload is successful.</description>
        /// </item>
        /// </list>
        /// </returns>
        [Route("api/uploadFileToAzure")]
        [Consumes("multipart/form-data")]
        [HttpPost]
        public async Task<IActionResult> UploadToAzureFile(IFormFile file, string directory)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is not provided.");

            var blobUrl = await _blobStorageService.UploadFileAsync(file, directory);
            return Ok(new { Message = "File uploaded successfully", BlobUrl = blobUrl });
        }

        /// <summary>
        /// Deletes a file from Azure Blob Storage using its blob name.
        /// </summary>
        /// <param name="blobName">The name of the blob to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the outcome of the operation:
        /// <list type="bullet">
        /// <item>
        /// <description>An <see cref="OkObjectResult"/> with a success message if the file is deleted successfully.</description>
        /// </item>
        /// <item>
        /// <description>A <see cref="NotFoundObjectResult"/> with an error message if the file is not found.</description>
        /// </item>
        /// </list>
        /// </returns>

        [Route("api/deleteFileFromAzure")]
        [HttpDelete]
        public async Task<IActionResult> DeleteFileFromAzure([FromQuery] string blobName)
        {
            bool isDeleted = await _blobStorageService.DeleteFileAsync(blobName);

            if (isDeleted)
                return Ok(new { Message = "File deleted successfully" });
            else
                return NotFound(new { Message = "File not found" });
        }

        /// <summary>
        /// Downloads a file from Azure Blob Storage.
        /// </summary>
        /// <param name="blobName">The name of the blob to be downloaded.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the following outcomes:
        /// <list type="bullet">
        /// <item>
        /// <description>A <see cref="NotFoundObjectResult"/> with an error message if the file is not found.</description>
        /// </item>
        /// <item>
        /// <description>A <see cref="FileResult"/> with the file data and metadata if the download is successful.</description>
        /// </item>
        /// </list>
        /// </returns>
        [Route("api/downloadFromAzure")]
        [HttpGet]
        public async Task<IActionResult> DownloadFileFromAzure([FromQuery] string blobName)
        {
            var fileData = await _blobStorageService.DownloadFileAsync(blobName);

            if (fileData == null)
                return NotFound(new { Message = "File not found" });

            return File(fileData, "application/octet-stream", blobName);
        }
    }
}
