using Microsoft.Azure.KeyVault.Models;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Extensions
{
    public static class DataExtensions
    {
        public static SecretAttributes CreateAttributes(this CreateSecretData createSecretData)
            => new SecretAttributes(createSecretData.IsEnabled, createSecretData.StartsAt, createSecretData.ExpiresAt);

        public static KeyAttributes CreateAttributes(this CreateKeyData createKeyData)
            => new KeyAttributes(createKeyData.IsEnabled, createKeyData.StartsAt, createKeyData.ExpiresAt);
    }
}
