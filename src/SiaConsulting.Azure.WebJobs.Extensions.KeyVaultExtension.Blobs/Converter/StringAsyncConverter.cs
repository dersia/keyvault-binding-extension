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
    public class StringAsyncConverter : IAsyncConverter<ICloudBlob, string>
    {
        private readonly IKeyVaultClient _keyVaultClient;

        public StringAsyncConverter(IKeyVaultClient keyVaultClient)
        {
            _keyVaultClient = keyVaultClient;
        }
        public async Task<string> ConvertAsync(ICloudBlob input, CancellationToken cancellationToken)
        {
            using var stream = new MemoryStream();
            await input.DownloadToStreamAsync(stream, null, new BlobRequestOptions { EncryptionPolicy = GetEncryptionPolicyForDownload() }, null, cancellationToken);
            stream.Seek(0, SeekOrigin.Begin);
            using var sr = new StreamReader(stream);
            return await sr.ReadToEndAsync();
        }

        private BlobEncryptionPolicy GetEncryptionPolicyForDownload()
        {
            var keyResolver = new KeyVaultKeyResolver(_keyVaultClient);
            var blobEncryptionPolicy = new BlobEncryptionPolicy(null, keyResolver);
            return blobEncryptionPolicy;
        }
    }
}
