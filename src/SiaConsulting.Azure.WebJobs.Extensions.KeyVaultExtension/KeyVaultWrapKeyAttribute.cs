using Microsoft.Azure.WebJobs.Description;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Common.Models;
using System;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension
{
    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter)]
    [Binding]
    public class KeyVaultWrapKeyAttribute : Attribute
    {
        [AppSetting]
        public string KeyVaultUrl { get; set; } = string.Empty;
        [AutoResolve]
        public string KeyName { get; set; } = string.Empty;
        [AutoResolve]
        public string? KeyVersion { get; set; } = null;
        public EncryptionAlgorithm Algorithm { get; set; } = EncryptionAlgorithm.RSA1_5;
        [AutoResolve]
        public string Value { get; set; } = string.Empty;
        public bool CreateKeyIfNotExistst { get; set; } = false;
        public KeyType? KeyType { get; set; } = Common.Models.KeyType.RSA;
        public KeyCurves? KeyCurve { get; set; }
        public int? KeySize { get; set; }
    }
}
