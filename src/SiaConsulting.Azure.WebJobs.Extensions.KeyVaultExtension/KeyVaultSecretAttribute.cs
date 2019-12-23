using Microsoft.Azure.WebJobs.Description;
using System;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension
{
    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter)]
    [Binding]
    public class KeyVaultSecretAttribute : Attribute
    {
        [AppSetting]
        public string KeyVaultUrl { get; set; } = string.Empty;
        [AutoResolve]
        public string SecretName { get; set; } = string.Empty;
        [AutoResolve]
        public string? ContentType { get; set; }
        public bool? IsEnabled { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime? StartsAt { get; set; }
    }
}
