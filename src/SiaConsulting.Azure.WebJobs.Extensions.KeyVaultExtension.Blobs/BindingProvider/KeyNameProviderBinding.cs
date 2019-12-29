using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Helper;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.BindingProvider
{
    public class KeyNameProviderBinding : IBinding
    {
        private readonly ParameterInfo _parameterInfo;
        private readonly IKeyNameProvider _keyNameProvider;

        public KeyNameProviderBinding(ParameterInfo parameterInfo, IKeyNameProvider keyNameProvider)
        {
            _parameterInfo = parameterInfo;
            _keyNameProvider = keyNameProvider;
        }

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var valueProvider = new SimpleValueProvider(_keyNameProvider);
            return Task.FromResult<IValueProvider>(valueProvider);
        }
        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var valueProvider = new SimpleValueProvider(_keyNameProvider);
            return Task.FromResult<IValueProvider>(valueProvider);
        }
        public ParameterDescriptor ToParameterDescriptor()
            => new ParameterDescriptor
            {
                Name = _parameterInfo.Name,
                DisplayHints = new ParameterDisplayHints
                {
                    Description = "IKeyNameProvider"
                }
            };

        public bool FromAttribute { get; } = false;
    }
}
