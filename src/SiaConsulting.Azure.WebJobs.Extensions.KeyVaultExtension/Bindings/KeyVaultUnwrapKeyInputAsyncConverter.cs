using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Common.Exceptions;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Common.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Bindings
{
    public class KeyVaultUnwrapKeyInputAsyncConverter : IAsyncConverter<KeyVaultUnwrapKeyAttribute, byte[]>
    {
        private readonly INameResolver _nameResolver;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly IKeyVaultClient _keyVaultClient;

        public KeyVaultUnwrapKeyInputAsyncConverter(INameResolver nameResolver, Microsoft.Extensions.Logging.ILogger logger, IKeyVaultClient keyVaultClient)
        {
            _nameResolver = nameResolver;
            _logger = logger;
            _keyVaultClient = keyVaultClient;
        }

        public async Task<byte[]> ConvertAsync(KeyVaultUnwrapKeyAttribute config, CancellationToken cancellationToken)
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
            var result = await _keyVaultClient.UnwrapKeyAsync(config.KeyVaultUrl,config.KeyName, config.KeyVersion, config.Algorithm.MapAlogrithm(), Convert.FromBase64String(_nameResolver.Resolve(config.Value) ?? config.Value), cancellationToken);
            return result.Result;
        }

        private async Task<KeyBundle?> GetKey(KeyVaultUnwrapKeyAttribute config, CancellationToken cancellationToken)
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
