using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Client;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Converter
{
    public class ICloudBlobAsyncConverter : IAsyncConverter<EncryptedBlobAttribute, ICloudBlob>
    {
        private readonly IKeyVaultClient _keyVaultClient;

        public ICloudBlobAsyncConverter(IKeyVaultClient keyVaultClient)
        {
            _keyVaultClient = keyVaultClient;
        }

        public async Task<ICloudBlob> ConvertAsync(EncryptedBlobAttribute input, CancellationToken cancellationToken)
        {
            var cloudBlobClient = CloudStorageAccount.Parse(input.BlobConnectionString).CreateCloudBlobClient();
            var keyVaultBlobClient = new KeyVaultBlobClient(cloudBlobClient, _keyVaultClient);
            var blobPath = BlobPath.ParseAndValidate(input.BlobPath) ?? throw new ArgumentException(nameof(EncryptedBlobAttribute.BlobPath));
            return await keyVaultBlobClient.GetBlob(blobPath, input.KeyVaultConnectionString, input.KeyName, cancellationToken);
        }
    }
}
