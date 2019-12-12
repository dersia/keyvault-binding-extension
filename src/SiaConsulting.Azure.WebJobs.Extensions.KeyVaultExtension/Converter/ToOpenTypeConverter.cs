using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Bindings;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Models;
using System.Collections.Generic;
using System.Linq;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Converter
{
    public class ToOpenTypeConverter<T> : IConverter<T, string>
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public ToOpenTypeConverter(Microsoft.Extensions.Logging.ILogger logger) 
            => _logger = logger;

        public string Convert(T input) 
            => JsonConvert.SerializeObject(input);
    }

    public class ToOpenTypeListConverter<T> : IConverter<IList<T>, IList<string>>
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public ToOpenTypeListConverter(Microsoft.Extensions.Logging.ILogger logger) 
            => _logger = logger;

        public IList<string> Convert(IList<T> input)
            => input.Select(i => JsonConvert.SerializeObject(input)).ToList();
    }
}
