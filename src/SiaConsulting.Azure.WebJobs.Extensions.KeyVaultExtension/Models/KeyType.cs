using System;
using System.Collections.Generic;
using System.Text;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Models
{
    public enum KeyType
    {
        EC,
        EC_HSM,
        RSA,
        RSA_HSM,
        oct
    }
}
