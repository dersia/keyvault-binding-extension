﻿using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Helper;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Extensions
{
    public static class KeyVaultExtensions
    {
        public static async Task<string?> GetKidByName(this IKeyVaultClient keyVaultClient, EncryptedBlobAttribute config, string keyName, CancellationToken cancellationToken)
        {
            KeyBundle? key = null;
            try
            {
                key = await keyVaultClient.GetKeyAsync(config.KeyVaultConnectionString, keyName, cancellationToken);
            }
            catch (Microsoft.Azure.KeyVault.Models.KeyVaultErrorException kve) when (kve.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
            }
            if (key is null && config.CreateKeyIfNotExistst is bool c && c && !string.IsNullOrWhiteSpace(keyName) && config.KeyType is { })
            {
                key = await keyVaultClient.CreateKeyAsync(config.KeyVaultConnectionString, keyName, config.KeyType.MapKeyType(), config.KeySize, null, null, null, config.KeyCurve.MapKeyCurve(), cancellationToken);
            }
            return key?.KeyIdentifier?.BaseIdentifier;
        }
    }
}
