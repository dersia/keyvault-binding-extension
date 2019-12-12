using Microsoft.Azure.WebJobs.Description;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Converter;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Models;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.KeyVault;
using System.Diagnostics;
using Microsoft.Azure.WebJobs.Host.Bindings;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Client;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Config
{
    [Extension("KeyVault")]
    public class KeyVaultConfigProvider : IExtensionConfigProvider
    {
        private readonly INameResolver _nameResolver;
        private readonly IKeyVaultClient? _keyVaultClient;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public KeyVaultConfigProvider(INameResolver nameResolver, ILoggerFactory loggerFactory, IKeyVaultClient keyVaultClient)
        {
            _nameResolver = nameResolver;
            _keyVaultClient = keyVaultClient;
            _logger = loggerFactory.CreateLogger("KeyVault");
        }

        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (_keyVaultClient == null)
            {
                throw new ArgumentNullException("_keyVaultClient");
            }

            context.AddAllSecretsConverters(_logger);
            context.AddAllKeysConverters(_logger);
            context.AddAllCommonConverters(_logger);

            var keyVaultSecretAttributeRule = context.AddBindingRule<KeyVaultSecretAttribute>();
            var keyVaultKeyAttributeRule = context.AddBindingRule<KeyVaultKeyAttribute>();
            var keyVaultEncryptAttributeRule = context.AddBindingRule<KeyVaultEncryptAttribute>();
            var keyVaultDecryptAttributeRule = context.AddBindingRule<KeyVaultDecryptAttribute>();
            var keyVaultAttributeRule = context.AddBindingRule<KeyVaultAttribute>();
            keyVaultSecretAttributeRule.BindToInput<SecretBundle>(new KeyVaultSecretInputAsyncConverter(_logger, _keyVaultClient));
            keyVaultKeyAttributeRule.BindToInput<KeyBundle>(new KeyVaultKeyInputAsyncConverter(_logger, _keyVaultClient));
            keyVaultEncryptAttributeRule.BindToInput<byte[]>(new KeyVaultEncryptInputAsyncConverter(_nameResolver,_logger, _keyVaultClient));
            keyVaultDecryptAttributeRule.BindToInput<byte[]>(new KeyVaultDecryptInputAsyncConverter(_nameResolver,_logger, _keyVaultClient));
            keyVaultSecretAttributeRule.BindToCollector<CreateSecretData>(config => new SecretDataAsyncCollector(config, _logger, _keyVaultClient));
            keyVaultKeyAttributeRule.BindToCollector<CreateKeyData>(config => new KeyDataAsyncCollector(config, _logger, _keyVaultClient));
            keyVaultAttributeRule.BindToInput<IKeyVaultUtil>(new KeyVaultInputAsyncConverter(_logger, _keyVaultClient));
        }
    }
}
