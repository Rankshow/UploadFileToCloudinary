using Azure.Storage.Blobs;
using UploadFileToCloudinary.Interface;

namespace UploadFileToCloudinary.Servicel;

public class AzureBlobStorageService : IAzureBlobStorageService
{
    private readonly BlobContainerClient _blobContainerClient;

    public AzureBlobStorageService(IConfiguration configuration)
    {
        string connectionString = configuration["AzureBlobStorage:ConnectionString"]!;
        string containerName = configuration["AzureBlobStorage:ContainerName"]!;
        _blobContainerClient = new BlobContainerClient(connectionString, containerName);
    }

    public async Task<string> UploadFileAsync(IFormFile file, string directory)
    {
        string blobName = $"{directory}/{file.FileName}";
        BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);

        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, overwrite: true);
        }

        return blobClient.Uri.ToString();
    }

    public async Task<bool> DeleteFileAsync(string blobName)
    {
        BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);
        return await blobClient.DeleteIfExistsAsync();
    }

    public async Task<byte[]> DownloadFileAsync(string blobName)
    {
        BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);

        if (await blobClient.ExistsAsync())
        {
            using (var ms = new MemoryStream())
            {
                await blobClient.DownloadToAsync(ms);
                return ms.ToArray();
            }
        }

        return null;
    }
}
