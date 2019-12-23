using Microsoft.Azure.KeyVault.Core;
using Microsoft.Azure.Storage.Blob;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Client
{
    public interface IKeyVaultBlobClient
    {
        Task<ICloudBlob> GetBlob(BlobPath blobPath, string keyVaultUrl, string keyName, CancellationToken cancellationToken);
        Task UploadBlobs(IList<EncryptedBlob> encryptedBlob, EncryptedBlobAttribute config, CancellationToken cancellationToken);
    }
}
