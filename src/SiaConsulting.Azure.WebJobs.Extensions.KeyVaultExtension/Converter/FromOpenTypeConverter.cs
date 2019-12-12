using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Converter
{
    public class FromOpenTypeConverter<T> : IConverter<SecretBundle, T>
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public FromOpenTypeConverter(Microsoft.Extensions.Logging.ILogger logger) 
            => _logger = logger;

        public T Convert(SecretBundle input)
            => JsonConvert.DeserializeObject<T>(input.Value);
    }
}
