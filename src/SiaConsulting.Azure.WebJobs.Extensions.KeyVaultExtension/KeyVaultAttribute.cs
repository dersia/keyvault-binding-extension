using Microsoft.Azure.WebJobs.Description;
using System;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public class KeyVaultAttribute : Attribute
    {
        public KeyVaultAttribute(string keyVaultUrl) 
            => KeyVaultUrl = keyVaultUrl;

        [AppSetting]
        public string KeyVaultUrl { get; set; }
    }
}
