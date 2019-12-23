using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Models
{
    public class EncryptedBlob : IDisposable
    {
        private EncryptedBlob(BlobPath blobPath, Stream stream, BlobProperties? properties = null, IDictionary<string, string>? metadata = null, AccessCondition? blobAccessCondition = null, AccessCondition? propertiesAccessCondition = null, AccessCondition? metadataAccessCondition = null)
        {
            BlobPath = blobPath;
            Stream = stream;
            Properties = properties;
            Metadata = metadata;
            BlobAccessCondition = blobAccessCondition;
            PropertiesAccessCondition = propertiesAccessCondition;
            MetadataAccessCondition = metadataAccessCondition;
        }

        public BlobProperties? Properties { get; set; }
        public IDictionary<string, string>? Metadata { get; set; }
        public AccessCondition? BlobAccessCondition { get; set; }
        public AccessCondition? PropertiesAccessCondition { get; set; }
        public AccessCondition? MetadataAccessCondition { get; set; }
        public BlobPath BlobPath { get; set; }
        public Stream Stream { get; set; }

        public BlobType? BlobType { get; set; }

        public static EncryptedBlob FromStream(BlobPath blobPath, Stream stream, BlobProperties? properties = null, IDictionary<string, string>? metadata = null, AccessCondition? blobAccessCondition = null, AccessCondition? propertiesAccessCondition = null, AccessCondition? metadataAccessCondition = null)
            => new EncryptedBlob(blobPath, stream, properties, metadata, blobAccessCondition, propertiesAccessCondition, metadataAccessCondition);

        public static EncryptedBlob FromByteArray(BlobPath blobPath, byte[] bytes, BlobProperties? properties = null, IDictionary<string, string>? metadata = null, AccessCondition? blobAccessCondition = null, AccessCondition? propertiesAccessCondition = null, AccessCondition? metadataAccessCondition = null)
        {
            var stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(bytes, 0, bytes.Length);
            return new EncryptedBlob(blobPath, stream, properties, metadata, blobAccessCondition, propertiesAccessCondition, metadataAccessCondition);
        }

        public static EncryptedBlob FromString(BlobPath blobPath, string data, BlobProperties? properties = null, IDictionary<string, string>? metadata = null, AccessCondition? blobAccessCondition = null, AccessCondition? propertiesAccessCondition = null, AccessCondition? metadataAccessCondition = null)
        {
            var stream = new MemoryStream();
            var bytes = Encoding.UTF8.GetBytes(data);
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(bytes, 0, bytes.Length);
            return new EncryptedBlob(blobPath, stream, properties, metadata, blobAccessCondition, propertiesAccessCondition, metadataAccessCondition);
        }

        public static EncryptedBlob FromType<T>(BlobPath blobPath, T data, BlobProperties? properties = null, IDictionary<string, string>? metadata = null, AccessCondition? blobAccessCondition = null, AccessCondition? propertiesAccessCondition = null, AccessCondition? metadataAccessCondition = null) where T : class
        {
            var stream = new MemoryStream();
            var bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes<T>(data);
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(bytes, 0, bytes.Length);
            return new EncryptedBlob(blobPath, stream, properties, metadata, blobAccessCondition, propertiesAccessCondition, metadataAccessCondition);
        }

        ~EncryptedBlob()
        {
            Dispose();
        }

        public void Dispose()
        {
            Stream.Dispose();
        }
    }
}
