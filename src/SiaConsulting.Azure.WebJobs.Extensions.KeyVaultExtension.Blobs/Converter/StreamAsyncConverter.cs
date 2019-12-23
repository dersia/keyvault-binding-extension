using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Converter
{
    public class StreamAsyncConverter : IAsyncConverter<ICloudBlob, Stream>
    {
        private readonly IKeyVaultClient _keyVaultClient;

        public StreamAsyncConverter(IKeyVaultClient keyVaultClient)
        {
            _keyVaultClient = keyVaultClient;
        }

        public async Task<Stream> ConvertAsync(ICloudBlob input, CancellationToken cancellationToken)
        {
            var stream = new MemoryStream();
            await input.DownloadToStreamAsync(stream, null, new BlobRequestOptions { EncryptionPolicy = GetEncryptionPolicyForDownload() }, null, cancellationToken);
            return stream;
        }

        private BlobEncryptionPolicy GetEncryptionPolicyForDownload()
        {
            var keyResolver = new KeyVaultKeyResolver(_keyVaultClient);
            var blobEncryptionPolicy = new BlobEncryptionPolicy(null, keyResolver);
            return blobEncryptionPolicy;
        }
    }
}
