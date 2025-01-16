using Azure.Core.Extensions;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.Azure;

namespace UploadFileToCloudinary.Extensions
{
    internal static class AzureClientFactoryBuilderExtensions
    {
        /// <summary>
        /// Adds a BlobServiceClient to the Azure Client Factory.
        /// Allows using either a service URI (for MSI) or a connection string.
        /// </summary>
        public static IAzureClientBuilder<BlobServiceClient, BlobClientOptions> AddBlobServiceClient(
            this AzureClientFactoryBuilder builder,
            string serviceUriOrConnectionString,
            bool preferMsi)
        {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri? serviceUri))
            {
                return builder.AddBlobServiceClient(serviceUri);
            }
            else
            {
                return builder.AddBlobServiceClient(serviceUriOrConnectionString);
            }
        }

        /// <summary>
        /// Adds a QueueServiceClient to the Azure Client Factory.
        /// Allows using either a service URI (for MSI) or a connection string.
        /// </summary>
        public static IAzureClientBuilder<QueueServiceClient, QueueClientOptions> AddQueueServiceClient(
            this AzureClientFactoryBuilder builder,
            string serviceUriOrConnectionString,
            bool preferMsi)
        {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri? serviceUri))
            {
                return builder.AddQueueServiceClient(serviceUri);
            }
            else
            {
                return builder.AddQueueServiceClient(serviceUriOrConnectionString);
            }
        }
    }
}
