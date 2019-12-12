using Microsoft.Azure.KeyVault.Models;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Exceptions;
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

        public static string? MapAlogrithm(this EncryptionAlgorithm? algorithm)
            => algorithm switch
            {
                null => null,
                { } algo => MapAlogrithm(algo)
            };

        public static string MapAlogrithm(this EncryptionAlgorithm algorithm)
            => algorithm switch
            {
                EncryptionAlgorithm.RSA1_5 => "RSA1_5",
                EncryptionAlgorithm.RSA_OAEP => "RSA-OAEP",
                _ => throw new KeyVaultBindingException("Algorithm not allowd")
            };

        public static string? MapKeyType(this KeyType? keyType)
            => keyType switch
            {
                null => null,
                { } kt => MapKeyType(kt)
            };

        public static string MapKeyType(this KeyType keyType)
            => keyType switch
            {
                KeyType.EC => "EC",
                KeyType.EC_HSM => "EC-HSM",
                KeyType.oct => "oct",
                KeyType.RSA => "RSA",
                KeyType.RSA_HSM => "RSA-HSM",
                _ => throw new KeyVaultBindingException("KeyType not allowd")
            };

        public static string? MapKeyCurve(this KeyCurves? keyCurve)
            => keyCurve switch
            {
                null => null,
                { } kc => MapKeyCurve(kc)
            };

        public static string MapKeyCurve(this KeyCurves keyCurve)
            => keyCurve switch
            {
                KeyCurves.P_256 => "P-256",
                KeyCurves.P_384 => "P-384",
                KeyCurves.P_521 => "P-521",
                KeyCurves.SECP256K1 => "SECP256K1",
                _ => throw new KeyVaultBindingException("KeyCurve not allowd")
            };
    }
}
