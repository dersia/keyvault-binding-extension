using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Common.Exceptions;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Common.Extensions;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Bindings
{
    public class KeyDataAsyncCollector : IAsyncCollector<CreateKeyData>, IDisposable
    {
        private readonly ConcurrentQueue<Task> _secretsToAdd = new ConcurrentQueue<Task>();
        private readonly KeyVaultKeyAttribute _config;
        private readonly ILogger _logger;
        private readonly IKeyVaultClient _keyVaultClient;

        public KeyDataAsyncCollector(KeyVaultKeyAttribute config, Microsoft.Extensions.Logging.ILogger logger, IKeyVaultClient keyVaultClient)
        {
            _config = config;
            _logger = logger;
            _keyVaultClient = keyVaultClient;
        }

        public Task AddAsync(CreateKeyData data, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_keyVaultClient == null)
                {
                    throw new KeyVaultBindingException("Client not initialized");
                }
                if (data is null)
                {
                    return Task.CompletedTask;
                }
                KeyAttributes? keyAttributes = null;
                if (data.IsEnabled is { } || data.ExpiresAt is { } || data.StartsAt is { })
                {
                    keyAttributes = new KeyAttributes(data.IsEnabled, data.StartsAt, data.ExpiresAt);
                }
                _secretsToAdd.Enqueue(_keyVaultClient.CreateKeyAsync(_config.KeyVaultUrl, data.KeyName, data.KeyType.MapKeyType(), data.KeySize, null, keyAttributes, data.Tags, data.KeyCurve.MapKeyCurve(), cancellationToken));
            }
            catch (Exception esException)
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
            catch (Exception esException)
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
