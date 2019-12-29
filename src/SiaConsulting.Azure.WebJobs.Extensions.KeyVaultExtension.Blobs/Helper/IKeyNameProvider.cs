using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Helper
{
    public interface IKeyNameProvider
    {
        string DefaultKey { get; set; }
        Task<string> GetKeyByPath(string path);
        Task<string> GetKeyByPath(BlobPath path);
        Task AddPathKeyPair(string path, string key);
        Task AddPathKeyPair(BlobPath path, string key);
        Task AddProvider(Func<string, Task<string?>> pathKeyProvider);
        Task AddProvider(Func<string, string?> pathKeyProvider);
    }
}
