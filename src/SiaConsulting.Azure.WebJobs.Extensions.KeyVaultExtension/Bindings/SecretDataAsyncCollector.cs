using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Common.Exceptions;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Bindings
{
    public class SecretDataAsyncCollector : IAsyncCollector<CreateSecretData>, IDisposable
    {
        private readonly ConcurrentQueue<Task> _secretsToAdd = new ConcurrentQueue<Task>();
        private readonly KeyVaultSecretAttribute _config;
        private readonly ILogger _logger;
        private readonly IKeyVaultClient _keyVaultClient;

        public SecretDataAsyncCollector(KeyVaultSecretAttribute config, Microsoft.Extensions.Logging.ILogger logger, IKeyVaultClient keyVaultClient)
        {
            _config = config;
            _logger = logger;
            _keyVaultClient = keyVaultClient;
        }

        public Task AddAsync(CreateSecretData data, CancellationToken cancellationToken = default)
        {
            try
            {
                if(_keyVaultClient == null)
                {
                    throw new KeyVaultBindingException("Client not initialized");
                }
                if (data is null || string.IsNullOrWhiteSpace(data.Value))
                {
                    return Task.CompletedTask;
                }
                if (string.IsNullOrWhiteSpace(data.SecretName))
                {
                    data.SecretName = _config.SecretName;
                }
                data.IsEnabled ??= _config.IsEnabled;
                data.ContentType ??= _config.ContentType;
                data.ExpiresAt ??= _config.ExpiresAt;
                data.StartsAt ??= _config.StartsAt;
                SecretAttributes? secretAttributes = null;
                if (data.IsEnabled is { } || data.ExpiresAt is { } || data.StartsAt is { })
                {
                    secretAttributes = new SecretAttributes(data.IsEnabled, data.StartsAt, data.ExpiresAt);
                }
                _secretsToAdd.Enqueue(_keyVaultClient.SetSecretAsync(_config.KeyVaultUrl, data.SecretName, data.Value, data.Tags, data.ContentType, secretAttributes, cancellationToken));
            }
            catch(Exception esException)
            {
                _logger.LogError(esException, esException.Message);
                throw;
            }
            return Task.CompletedTask;
        }

        public async Task FlushAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_secretsToAdd == null || _secretsToAdd.Count < 1)
                {
                    return;
                }
                await Task.WhenAll(_secretsToAdd);
            }
            catch(Exception esException)
            {
                _logger.LogError(esException, esException.Message);
                throw;
            }
            finally
            {
                Dispose();
            }
        }

        public void Dispose()
        {            
        }
    }
}
