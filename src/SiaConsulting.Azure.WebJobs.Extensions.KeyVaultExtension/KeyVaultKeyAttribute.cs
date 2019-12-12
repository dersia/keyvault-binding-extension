using Microsoft.Azure.WebJobs.Description;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Models;
using System;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public class KeyVaultKeyAttribute : Attribute
    {
        [AppSetting]
        public string KeyVaultUrl { get; set; } = string.Empty;
        [AutoResolve]
        public string KeyName { get; set; } = string.Empty;
        public KeyType KeyType { get; set; } = KeyType.RSA;
        public bool? IsEnabled { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime? StartsAt { get; set; }
        public KeyCurves? KeyCurve { get; set; }
        public int? KeySize { get; set; }
    }
}
