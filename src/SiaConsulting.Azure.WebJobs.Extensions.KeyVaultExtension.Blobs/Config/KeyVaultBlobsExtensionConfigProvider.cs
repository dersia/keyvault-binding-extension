using Microsoft.Azure.KeyVault.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Models;
using System.IO;
using Microsoft.Azure.Storage.Blob;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Client;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Converter;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs.Host.Bindings;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Helper;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Config
{
    public class KeyVaultBlobsExtensionConfigProvider : IExtensionConfigProvider
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly IKeyVaultClient _keyVaultClient;
        private readonly IKeyNameProvider _keyNameProvider;

        public KeyVaultBlobsExtensionConfigProvider(ILoggerFactory loggerFactory, IKeyVaultClient keyVaultClient, IKeyNameProvider keyNameProvider)
        {
            _logger = loggerFactory.CreateLogger("KeyVault");
            _keyVaultClient = keyVaultClient;
            _keyNameProvider = keyNameProvider;
        }
        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var encryptedBlobAttributeRule = context.AddBindingRule<EncryptedBlobAttribute>();
            encryptedBlobAttributeRule.When("Access", FileAccess.Read).BindToInput<ICloudBlob>(new ICloudBlobAsyncConverter(_keyVaultClient, _keyNameProvider));
            encryptedBlobAttributeRule.When("Access", FileAccess.ReadWrite).BindToInput<ICloudBlob>(new ICloudBlobAsyncConverter(_keyVaultClient, _keyNameProvider));
            encryptedBlobAttributeRule.BindToCollector<EncryptedBlob>(config => new EncryptedBlobAsyncCollector(config, _keyVaultClient, _keyNameProvider));
            encryptedBlobAttributeRule.AddOpenConverter<ICloudBlob, OpenType>(typeof(ICloudBlobToOpenTypeConverter<>), _keyVaultClient, _logger);
            context.AddConverters(_keyVaultClient, _logger);
        }
    }
}
