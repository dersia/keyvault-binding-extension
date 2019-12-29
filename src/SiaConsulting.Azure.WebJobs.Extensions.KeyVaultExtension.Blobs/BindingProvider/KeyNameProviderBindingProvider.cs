using Microsoft.Azure.WebJobs.Host.Bindings;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.BindingProvider
{
    internal class KeyNameProviderBindingProvider : IBindingProvider
    {
        private static readonly Task<IBinding?> _nullBinding = Task.FromResult<IBinding?>(null);
        private readonly IKeyNameProvider _keyNameProvider;

        public KeyNameProviderBindingProvider(IKeyNameProvider keyNameProvider) 
            => _keyNameProvider = keyNameProvider;

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Parameter.ParameterType != typeof(IKeyNameProvider))
            {
                return _nullBinding;
            }

            return Task.FromResult<IBinding>(new KeyNameProviderBinding(context.Parameter, _keyNameProvider));
        }
    }
}
