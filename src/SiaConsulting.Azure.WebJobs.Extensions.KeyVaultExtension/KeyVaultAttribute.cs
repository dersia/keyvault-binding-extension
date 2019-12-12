using Microsoft.Azure.WebJobs.Description;
using System;
using System.Collections.Generic;
using System.Text;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public class KeyVaultAttribute : Attribute
    {
        [AppSetting]
        public string KeyVaultUrl { get; set; } = string.Empty;
    }
}
