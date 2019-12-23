using System;
using System.Collections.Generic;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Models
{
    public class CreateSecretData
    {
        public string SecretName { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public bool? IsEnabled { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime? StartsAt { get; set; }
        public IDictionary<string, string>? Tags { get; set; }
    }
}
