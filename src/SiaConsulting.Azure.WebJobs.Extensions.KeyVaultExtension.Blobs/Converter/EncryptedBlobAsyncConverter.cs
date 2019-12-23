using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Converter
{
    public static class EncryptedBlobAsyncConverter
    {
        public static EncryptedBlob Convert(byte[] input, Attribute config)
            => EncryptedBlob.FromByteArray(BlobPath.ParseAndValidate(((EncryptedBlobAttribute)config).BlobPath), input);
        public static EncryptedBlob Convert(Stream input, Attribute config)
            => EncryptedBlob.FromStream(BlobPath.ParseAndValidate(((EncryptedBlobAttribute)config).BlobPath), input);
        public static EncryptedBlob Convert(string input, Attribute config)
            => EncryptedBlob.FromString(BlobPath.ParseAndValidate(((EncryptedBlobAttribute)config).BlobPath), input);
    }
}
