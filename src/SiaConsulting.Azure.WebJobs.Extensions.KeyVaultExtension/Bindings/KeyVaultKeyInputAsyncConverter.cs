using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Bindings
{
    public class KeyVaultKeyInputAsyncConverter : IAsyncConverter<KeyVaultKeyAttribute, KeyBundle>
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly IKeyVaultClient _keyVaultClient;

        public KeyVaultKeyInputAsyncConverter(Microsoft.Extensions.Logging.ILogger logger, IKeyVaultClient keyVaultClient)
        {
            _logger = logger;
            _keyVaultClient = keyVaultClient;
        }

        public async Task<KeyBundle> ConvertAsync(KeyVaultKeyAttribute config, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(config?.KeyVaultUrl))
            {
                var esException = new KeyVaultBindingException("KeyVaultUrl cant be empty");
                _logger.LogError(esException, esException.Message);
                throw esException;
            }
            return await _keyVaultClient.GetKeyAsync(config.KeyVaultUrl, config.KeyName, cancellationToken);
        }
    }
}
