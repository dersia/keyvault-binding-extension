using Microsoft.Azure.Storage.Blob;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Extensions
{
    public static class BlobExtensions
    {
        public static ICloudBlob GetBlobReferenceFromType(this CloudBlobContainer container, BlobPath blobPath, BlobType? blobType)
            => blobType switch
            {
                BlobType.PageBlob => container.GetPageBlobReference(blobPath.BlobName),
                BlobType.AppendBlob => container.GetAppendBlobReference(blobPath.BlobName),
                _ => container.GetBlockBlobReference(blobPath.BlobName),
            };

        public static EncryptedBlob CreateNew(this EncryptedBlob encryptedBlob, BlobType blobType)
        {
            encryptedBlob.BlobType = blobType;
            return encryptedBlob;
        }

        public static void MergeProperties(this BlobProperties originalProperties, BlobProperties propertiesToMerge)
        {
            foreach (var prop in propertiesToMerge.GetType().GetProperties())
            {
                originalProperties.GetType().GetProperty(prop.Name).SetValue(originalProperties, prop.GetValue(propertiesToMerge));
            }
        }

        public static void MergeMetadata(this IDictionary<string, string> originalMetadata, IDictionary<string, string> metadataToMerge)
        {
            foreach (var data in originalMetadata.Keys.ToList())
            {
                if (!metadataToMerge.ContainsKey(data))
                    continue;
                originalMetadata[data] = metadataToMerge[data];
            }
        }
    }
}
