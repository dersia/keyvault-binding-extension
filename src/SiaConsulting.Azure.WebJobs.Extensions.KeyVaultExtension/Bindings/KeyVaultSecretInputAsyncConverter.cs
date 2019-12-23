using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Common.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Bindings
{
    public class KeyVaultSecretInputAsyncConverter : IAsyncConverter<KeyVaultSecretAttribute, SecretBundle>
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly IKeyVaultClient _keyVaultClient;

        public KeyVaultSecretInputAsyncConverter(Microsoft.Extensions.Logging.ILogger logger, IKeyVaultClient keyVaultClient)
        {
            _logger = logger;
            _keyVaultClient = keyVaultClient;
        }

        public async Task<SecretBundle> ConvertAsync(KeyVaultSecretAttribute config, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(config?.KeyVaultUrl))
            {
                var esException = new KeyVaultBindingException("KeyVaultUrl cant be empty");
                _logger.LogError(esException, esException.Message);
                throw esException;
            }
            return await _keyVaultClient.GetSecretAsync(config.KeyVaultUrl, config.SecretName, cancellationToken);
        }
    }
}
