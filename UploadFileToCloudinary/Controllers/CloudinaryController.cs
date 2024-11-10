using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UploadFileToCloudinary.Interface;

namespace UploadFileToCloudinary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CloudinaryController : ControllerBase
    {
        readonly ICloudinaryService _cloudinaryService;

        public CloudinaryController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        // Endpoint to upload file
        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
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

        // Endpoint to delete file
        [HttpDelete("DeleteFile/{publicId}")]
        public async Task<IActionResult> DeleteFile(string publicId)
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
    }
}
