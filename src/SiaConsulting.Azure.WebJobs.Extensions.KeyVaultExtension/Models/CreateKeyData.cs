using System;
using System.Collections.Generic;
using System.Linq;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Models
{
    public class CreateKeyData
    {
        public string KeyName { get; set; } = string.Empty;
        public KeyType KeyType { get; set; } = KeyType.RSA;
        public KeyCurves? KeyCurve { get; set; }
        public int? KeySize { get; set; }
        public bool? IsEnabled { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime? StartsAt { get; set; }
        public IDictionary<string, string>? Tags { get; set; }
    }
}
