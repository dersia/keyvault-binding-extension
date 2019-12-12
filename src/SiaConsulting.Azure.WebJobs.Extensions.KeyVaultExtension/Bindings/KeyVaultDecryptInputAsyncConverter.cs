using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Exceptions;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Bindings
{
    public class KeyVaultDecryptInputAsyncConverter : IAsyncConverter<KeyVaultDecryptAttribute, byte[]>
    {
        private readonly INameResolver _nameResolver;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly IKeyVaultClient _keyVaultClient;

        public KeyVaultDecryptInputAsyncConverter(INameResolver nameResolver, Microsoft.Extensions.Logging.ILogger logger, IKeyVaultClient keyVaultClient)
        {
            _nameResolver = nameResolver;
            _logger = logger;
            _keyVaultClient = keyVaultClient;
        }

        public async Task<byte[]> ConvertAsync(KeyVaultDecryptAttribute config, CancellationToken cancellationToken)
        {
            if (config is null || string.IsNullOrWhiteSpace(config?.KeyVaultUrl))
            {
                var esException = new KeyVaultBindingException("KeyVaultUrl cant be empty");
                _logger.LogError(esException, esException.Message);
                throw esException;
            }
            if (config.CreateKeyIfNotExistst is bool c && c && !string.IsNullOrWhiteSpace(config.KeyName) && config.KeyType is { })
            {
                var key = await GetKey(config, cancellationToken);
                if (key is null)
                {
                    key = await _keyVaultClient.CreateKeyAsync(config.KeyVaultUrl, config.KeyName, config.KeyType.MapKeyType(), config.KeySize, null, null, null, config.KeyCurve.MapKeyCurve(), cancellationToken);
                }
                if (key?.KeyIdentifier is { } ki)
                {
                    config.KeyVersion = ki.Version;
                }
            }
            if(config.KeyVersion is null)
            {
                var key = await GetKey(config, cancellationToken);
                if(key?.KeyIdentifier is null)
                {
                    throw new KeyVaultBindingException("Could not get KeyVersion");
                }
                config.KeyVersion = key.KeyIdentifier.Version;
            }
            var result = await _keyVaultClient.DecryptAsync(config.KeyVaultUrl,config.KeyName, config.KeyVersion, config.Algorithm.MapAlogrithm(), Convert.FromBase64String(_nameResolver.Resolve(config.Value) ?? config.Value), cancellationToken);
            return result.Result;
        }

        private async Task<KeyBundle?> GetKey(KeyVaultDecryptAttribute config, CancellationToken cancellationToken)
        {
            KeyBundle? key = null;
            try
            {
                key = await _keyVaultClient.GetKeyAsync(config.KeyVaultUrl, config.KeyName, cancellationToken);
            }
            catch (Microsoft.Azure.KeyVault.Models.KeyVaultErrorException kve) when (kve.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogTrace("key does not exists");
            }

            return key;
        }
    }
}
