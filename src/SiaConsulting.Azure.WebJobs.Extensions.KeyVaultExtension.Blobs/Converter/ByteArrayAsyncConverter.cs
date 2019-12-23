using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Converter
{
    public class ByteArrayAsyncConverter : IAsyncConverter<ICloudBlob, byte[]>
    {
        private readonly IKeyVaultClient _keyVaultClient;

        public ByteArrayAsyncConverter(IKeyVaultClient keyVaultClient)
        {
            _keyVaultClient = keyVaultClient;
        }
        public async Task<byte[]> ConvertAsync(ICloudBlob input, CancellationToken cancellationToken)
        {
            using var stream = new MemoryStream();
            await input.DownloadToStreamAsync(stream, null, new BlobRequestOptions { EncryptionPolicy = GetEncryptionPolicyForDownload() }, null, cancellationToken);
            return stream.ToArray();
        }

        private BlobEncryptionPolicy GetEncryptionPolicyForDownload()
        {
            var keyResolver = new KeyVaultKeyResolver(_keyVaultClient);
            var blobEncryptionPolicy = new BlobEncryptionPolicy(null, keyResolver);
            return blobEncryptionPolicy;
        }
    }
}
