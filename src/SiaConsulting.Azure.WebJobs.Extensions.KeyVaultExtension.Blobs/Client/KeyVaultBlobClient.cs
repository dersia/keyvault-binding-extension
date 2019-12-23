using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Core;
using Microsoft.Azure.Storage.Blob;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Extensions;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Client
{
    public class KeyVaultBlobClient : IKeyVaultBlobClient
    {
        private readonly CloudBlobClient _blobClient;
        private readonly IKeyVaultClient _keyVaultClient;

        public KeyVaultBlobClient(CloudBlobClient blobClient, IKeyVaultClient keyVaultClient)
        {
            _blobClient = blobClient;
            _keyVaultClient = keyVaultClient;
        }

        private CloudBlobContainer GetContainer(BlobPath blobPath) => _blobClient.GetContainerReference(blobPath.ContainerName);
        public async Task<ICloudBlob> GetBlob(BlobPath blobPath, string keyVaultUrl, string keyName, CancellationToken cancellationToken)
        {
            var container = await GetOrCreateContainer(blobPath, cancellationToken);
            return await container.GetBlobReferenceFromServerAsync(blobPath.BlobName, null, new BlobRequestOptions { EncryptionPolicy = GetEncryptionPolicyForDownload() }, null, cancellationToken);
        }

        public async Task UploadBlobs(IList<EncryptedBlob> encryptedBlobs, EncryptedBlobAttribute config, CancellationToken cancellationToken)
        {
            foreach(var blob in encryptedBlobs)
            {
                await UploadBlob(blob, config, cancellationToken);
            }
        }

        public async Task UploadBlob(EncryptedBlob encryptedBlob, EncryptedBlobAttribute config, CancellationToken cancellationToken)
        {
            var container = await GetOrCreateContainer(encryptedBlob.BlobPath, cancellationToken);
            ICloudBlob blob;
            if (await container.GetBlobReference(encryptedBlob.BlobPath.BlobName).ExistsAsync())
            {
                blob = await container.GetBlobReferenceFromServerAsync(encryptedBlob.BlobPath.BlobName);
            } 
            else
            {
                blob = container.GetBlobReferenceFromType(encryptedBlob.BlobPath, encryptedBlob.BlobType);
            }
            var blobEncryptionPolicy = await GetEncryptionPolicyForUpload(config, cancellationToken);
            await blob.UploadFromStreamAsync(encryptedBlob.Stream, encryptedBlob.BlobAccessCondition, new BlobRequestOptions { EncryptionPolicy = blobEncryptionPolicy }, null, cancellationToken);
            if(encryptedBlob.Properties != null)
            {
                blob.Properties.MergeProperties(encryptedBlob.Properties);
                await blob.SetPropertiesAsync(encryptedBlob.PropertiesAccessCondition, null, null, cancellationToken);
            }
            if (encryptedBlob.Metadata != null)
            {
                blob.Metadata.MergeMetadata(encryptedBlob.Metadata);
                await blob.SetMetadataAsync(encryptedBlob.MetadataAccessCondition, null, null, cancellationToken);
            }
        }

        private async Task<BlobEncryptionPolicy> GetEncryptionPolicyForUpload(EncryptedBlobAttribute config, CancellationToken cancellationToken)
        {
            var kid = await _keyVaultClient.GetKidByName(config, cancellationToken) ?? throw new ArgumentException(nameof(EncryptedBlobAttribute.KeyName));
            var keyResolver = new KeyVaultKeyResolver(_keyVaultClient);
            var key = await keyResolver.ResolveKeyAsync(kid, cancellationToken);
            var blobEncryptionPolicy = new BlobEncryptionPolicy(key, null);
            return blobEncryptionPolicy;
        }

        private BlobEncryptionPolicy GetEncryptionPolicyForDownload()
        {
            var keyResolver = new KeyVaultKeyResolver(_keyVaultClient);
            var blobEncryptionPolicy = new BlobEncryptionPolicy(null, keyResolver);
            return blobEncryptionPolicy;
        }

        private async Task<CloudBlobContainer> GetOrCreateContainer(BlobPath blobPath, CancellationToken cancellationToken)
        {
            var container = GetContainer(blobPath);
            // Call ExistsAsync before attempting to create. This reduces the number of 
            // 40x calls that App Insights may be tracking automatically
            if (!await container.ExistsAsync())
            {
                await container.CreateIfNotExistsAsync(cancellationToken);
            }
            return container;
        }
    }
}
