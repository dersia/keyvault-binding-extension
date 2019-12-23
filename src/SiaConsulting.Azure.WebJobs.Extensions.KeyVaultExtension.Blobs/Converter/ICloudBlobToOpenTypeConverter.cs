using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Converter
{
    public class ICloudBlobToOpenTypeConverter<T> : IAsyncConverter<ICloudBlob, T>
    {
        private readonly IKeyVaultClient _keyVaultClient;

        public ICloudBlobToOpenTypeConverter(IKeyVaultClient keyVaultClient)
        {
            _keyVaultClient = keyVaultClient;
        }
        public async Task<T> ConvertAsync(ICloudBlob input, CancellationToken cancellationToken)
        {
            using var stream = new MemoryStream();
            await input.DownloadToStreamAsync(stream, null, new BlobRequestOptions { EncryptionPolicy = GetEncryptionPolicyForDownload() }, null, cancellationToken);
            return await System.Text.Json.JsonSerializer.DeserializeAsync<T>(stream);
        }

        private BlobEncryptionPolicy GetEncryptionPolicyForDownload()
        {
            var keyResolver = new KeyVaultKeyResolver(_keyVaultClient);
            var blobEncryptionPolicy = new BlobEncryptionPolicy(null, keyResolver);
            return blobEncryptionPolicy;
        }
    }
}
