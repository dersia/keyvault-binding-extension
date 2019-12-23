using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Client;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Converter
{
    public class EncryptedBlobAsyncCollector : IAsyncCollector<EncryptedBlob>
    {
        private readonly IList<EncryptedBlob> _encryptedBlobs = new List<EncryptedBlob>();
        private readonly EncryptedBlobAttribute _config;
        private readonly IKeyVaultBlobClient _keyVaultBlobClient;

        public EncryptedBlobAsyncCollector(EncryptedBlobAttribute config, IKeyVaultClient keyVaultClient)
        {
            _config = config;
            var cloudBlobClient = CloudStorageAccount.Parse(config.BlobConnectionString).CreateCloudBlobClient();
            _keyVaultBlobClient = new KeyVaultBlobClient(cloudBlobClient, keyVaultClient);
        }

        public Task AddAsync(EncryptedBlob encryptedBlob, CancellationToken cancellationToken = default)
        {
            _encryptedBlobs.Add(encryptedBlob);
            return Task.CompletedTask;
        }

        public Task FlushAsync(CancellationToken cancellationToken = default)
            =>_keyVaultBlobClient.UploadBlobs(_encryptedBlobs, _config, cancellationToken);
    }
}
