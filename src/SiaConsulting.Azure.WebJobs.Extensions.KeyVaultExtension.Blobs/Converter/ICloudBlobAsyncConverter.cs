using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Client;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Helper;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Converter
{
    public class ICloudBlobAsyncConverter : IAsyncConverter<EncryptedBlobAttribute, ICloudBlob>
    {
        private readonly IKeyVaultClient _keyVaultClient;
        private readonly IKeyNameProvider _keyNameProvider;

        public ICloudBlobAsyncConverter(IKeyVaultClient keyVaultClient, IKeyNameProvider keyNameProvider)
        {
            _keyVaultClient = keyVaultClient;
            _keyNameProvider = keyNameProvider;
        }

        public async Task<ICloudBlob> ConvertAsync(EncryptedBlobAttribute input, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(_keyNameProvider.DefaultKey) && input.KeyName is string kn && !string.IsNullOrWhiteSpace(kn))
            {
                _keyNameProvider.DefaultKey = input.KeyName;
            }
            var cloudBlobClient = CloudStorageAccount.Parse(input.BlobConnectionString).CreateCloudBlobClient();
            var keyVaultBlobClient = new KeyVaultBlobClient(cloudBlobClient, _keyVaultClient, _keyNameProvider);
            var blobPath = BlobPath.ParseAndValidate(input.BlobPath) ?? throw new ArgumentException(nameof(EncryptedBlobAttribute.BlobPath));
            return await keyVaultBlobClient.GetBlob(blobPath, cancellationToken);
        }
    }
}
