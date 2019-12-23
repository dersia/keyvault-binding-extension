using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Newtonsoft.Json.Linq;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Converter
{
    public static class CommonConverters
    {
        public static ExtensionConfigContext AddConverters(this ExtensionConfigContext context, IKeyVaultClient keyVaultClient, Microsoft.Extensions.Logging.ILogger logger)
            => context
            .AddBlobToByteArrayConverter(keyVaultClient, logger)
            .AddBlobToStreamConverter(keyVaultClient, logger)
            .AddBlobToStringConverter(keyVaultClient, logger)
            .AddStringToEncryptedBlobConverter(logger)
            .AddStreamToEncryptedBlobConverter(logger)
            .AddByteArrayToEncryptedBlobConverter(logger)
            .AddOpenTypeToEncryptedBlobConverter(logger)
            .AddStringToJObjectConverter(logger)
            .AddStringToExpandoObjectConverter(logger)
            .AddStringToDynamicConverter(logger)
            .AddJObjectToStringConverter(logger)
            .AddExpandoObjectToStringConverter(logger)
            .AddDynamicToStringConverter(logger);

        private static ExtensionConfigContext AddBlobToStreamConverter(this ExtensionConfigContext context, IKeyVaultClient keyVaultClient, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<ICloudBlob, Stream>(new StreamAsyncConverter(keyVaultClient));
        private static ExtensionConfigContext AddBlobToByteArrayConverter(this ExtensionConfigContext context, IKeyVaultClient keyVaultClient, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<ICloudBlob, byte[]>(new ByteArrayAsyncConverter(keyVaultClient));
        private static ExtensionConfigContext AddBlobToStringConverter(this ExtensionConfigContext context, IKeyVaultClient keyVaultClient, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<ICloudBlob, string>(new StringAsyncConverter(keyVaultClient));
        private static ExtensionConfigContext AddOpenTypeToEncryptedBlobConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddOpenConverter<OpenType, EncryptedBlob>(OpenTypeToEncryptedBlobConverter<OpenType>.ConvertFromType);

        //JObject
        private static ExtensionConfigContext AddStringToJObjectConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<string?, JObject?>(JObject.Parse);
        private static ExtensionConfigContext AddStringToExpandoObjectConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<string?, ExpandoObject?>(s => System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(s));
        private static ExtensionConfigContext AddStringToDynamicConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
                    => context.AddConverter<string?, dynamic?>(s => System.Text.Json.JsonSerializer.Deserialize<dynamic>(s));
        private static ExtensionConfigContext AddJObjectToStringConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<JObject?, string?>(s => s?.ToString());
        private static ExtensionConfigContext AddExpandoObjectToStringConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<ExpandoObject?, string?>(s => System.Text.Json.JsonSerializer.Serialize(s));
        private static ExtensionConfigContext AddDynamicToStringConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
                    => context.AddConverter<dynamic?, string?>(s => System.Text.Json.JsonSerializer.Serialize(s));

        // In Binding only
        private static ExtensionConfigContext AddStringToEncryptedBlobConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
    => context.AddConverter<string, EncryptedBlob>(EncryptedBlobAsyncConverter.Convert);
        private static ExtensionConfigContext AddStreamToEncryptedBlobConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<Stream, EncryptedBlob>(EncryptedBlobAsyncConverter.Convert);
        private static ExtensionConfigContext AddByteArrayToEncryptedBlobConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<byte[], EncryptedBlob>(EncryptedBlobAsyncConverter.Convert);
    }
}
