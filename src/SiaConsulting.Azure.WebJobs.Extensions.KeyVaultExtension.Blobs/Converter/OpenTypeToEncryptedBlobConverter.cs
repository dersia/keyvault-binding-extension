using Microsoft.Azure.WebJobs;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Converter
{
    public static class OpenTypeToEncryptedBlobConverter<T> where T : class
    {
        public static FuncAsyncConverter ConvertFromType
            => (obj, attr, context)
            => Task.FromResult<object>(EncryptedBlob.FromType<T>(BlobPath.ParseAndValidate(((EncryptedBlobAttribute)attr).BlobPath), (T)obj));
    }
}
