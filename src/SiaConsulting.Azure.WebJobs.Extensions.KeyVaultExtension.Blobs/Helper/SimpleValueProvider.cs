using Microsoft.Azure.WebJobs.Host.Bindings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Helper
{
    public class SimpleValueProvider : IValueProvider
    {
        private readonly IKeyNameProvider _keyNameProvider;

        public SimpleValueProvider(IKeyNameProvider keyNameProvider) 
            => _keyNameProvider = keyNameProvider;

        public Task<object> GetValueAsync() => Task.FromResult<object>(_keyNameProvider);
        public string ToInvokeString() => _keyNameProvider.DefaultKey;

        public Type Type { get; } = typeof(IKeyNameProvider);
    }
}
