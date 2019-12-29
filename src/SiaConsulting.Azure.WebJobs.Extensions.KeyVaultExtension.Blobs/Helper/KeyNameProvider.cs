using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Helper
{
    public class KeyNameProvider : IKeyNameProvider
    {
        private readonly IDictionary<string, string> _pathKeyParis = new Dictionary<string, string>();
        private readonly List<Func<string, Task<string?>>> _providers = new List<Func<string, Task<string?>>>();
        public string DefaultKey { get; set; } = string.Empty;

        public async Task<string> GetKeyByPath(string path)
        {
            if (_pathKeyParis.TryGetValue(path, out var key))
            {
                return key;
            }
            if(_providers.Count > 0)
            {
                foreach(var provider in _providers)
                {
                    var providerKey = await provider(path);
                    if(providerKey is { } pk && !string.IsNullOrWhiteSpace(pk))
                    {
                        return providerKey;
                    }
                }
            }
            return DefaultKey;
        }
        public Task<string> GetKeyByPath(BlobPath path)
            => GetKeyByPath(path.ToString());
        public Task AddPathKeyPair(string path, string key)
        {
            if (_pathKeyParis.ContainsKey("path"))
            {
                _pathKeyParis[path] = key;
            } 
            else
            {
                _pathKeyParis.Add(path, key);
            }
            return Task.CompletedTask;
        }
        public Task AddPathKeyPair(BlobPath path, string key)
            => AddPathKeyPair(path.ToString(), key);
        public Task AddProvider(Func<string, Task<string?>> pathKeyProvider)
        {
            _providers.Add(pathKeyProvider);
            return Task.CompletedTask;
        }
        public Task AddProvider(Func<string, string?> pathKeyProvider)
            => AddProvider(path => Task.FromResult(pathKeyProvider(path)));
    }
}
