using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.Azure.WebJobs.Host.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Converter
{
    public static class CommonConverter
    {
        public static ExtensionConfigContext AddAllSecretsConverters(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger) 
            => context
                .AddStringToSecretConverter(logger)
                .AddByteArrayToConverter(logger)
                .AddJObjectToSecretConverter(logger)
                .AddDynamicToSecretConverter(logger)
                .AddExpandoObjectToSecretConverter(logger)
                .AddSecretToJObjectConverter(logger)
                .AddSecretToStringConverter(logger)
                .AddSecretToByteArrayConverter(logger);

        public static ExtensionConfigContext AddAllKeysConverters(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger) 
            => context
                .AddKeyToJsonWebKeyConverter(logger)
                .AddKeyToJObjectConverter(logger)
                .AddJObjectToKeyConverter(logger)
                .AddDynamicToKeyConverter(logger)
                .AddExpandoObjectToKeyConverter(logger)
                .AddJArrayToKeyListConverter(logger);

        public static ExtensionConfigContext AddAllCommonConverters(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger) 
            => context
                .AddStringToByteArrayConverter(logger)
                .AddByteArrayToStringConverter(logger)
                .AddJObecjtToStringConverter(logger)
                .AddStringToJObjectConverter(logger);

        private static ExtensionConfigContext AddStringToSecretConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<string, CreateSecretData>((value, attr) =>
            {
                var config = (KeyVaultSecretAttribute)attr;
                return new CreateSecretData { SecretName = config.SecretName, Value = value, ContentType = config.ContentType, IsEnabled = config.IsEnabled, ExpiresAt = config.ExpiresAt, StartsAt = config.StartsAt };
            });

        private static ExtensionConfigContext AddByteArrayToConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<byte[], CreateSecretData>((value, attr) =>
            {
                var config = (KeyVaultSecretAttribute)attr;
                return new CreateSecretData { SecretName = config.SecretName, Value = Convert.ToBase64String(value), ContentType = config.ContentType, IsEnabled = config.IsEnabled, ExpiresAt = config.ExpiresAt, StartsAt = config.StartsAt };
            });

        public static ExtensionConfigContext AddJObjectToSecretConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<JObject?, CreateSecretData?>(payload => JsonConvert.DeserializeObject<CreateSecretData>(payload?.ToString()));

        public static ExtensionConfigContext AddDynamicToSecretConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<dynamic?, CreateSecretData?>(payload => JsonConvert.DeserializeObject<CreateSecretData>((JsonConvert.SerializeObject(payload))));

        public static ExtensionConfigContext AddExpandoObjectToSecretConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<ExpandoObject?, CreateSecretData?>(payload => JsonConvert.DeserializeObject<CreateSecretData>((JsonConvert.SerializeObject(payload))));

        public static ExtensionConfigContext AddSecretToJObjectConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<SecretBundle?, JObject?>(JObject.FromObject);

        private static ExtensionConfigContext AddSecretToStringConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<SecretBundle?, string?>(value => value?.Value);

        private static ExtensionConfigContext AddSecretToByteArrayConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<SecretBundle?, byte[]?>(value => Convert.FromBase64String(value?.Value));

        public static ExtensionConfigContext AddKeyToJsonWebKeyConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<KeyBundle?, JsonWebKey?>(value => value?.Key);

        public static ExtensionConfigContext AddKeyToJObjectConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<KeyBundle?, JObject?>(JObject.FromObject);

        public static ExtensionConfigContext AddJObjectToKeyConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<JObject?, CreateKeyData?>(value => JsonConvert.DeserializeObject<CreateKeyData?>(value?.ToString()));

        public static ExtensionConfigContext AddDynamicToKeyConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<dynamic?, CreateKeyData?>(value => JsonConvert.DeserializeObject<CreateKeyData?>(value?.ToString()));

        public static ExtensionConfigContext AddExpandoObjectToKeyConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<ExpandoObject?, CreateKeyData?>(value => JsonConvert.DeserializeObject<CreateKeyData?>(value?.ToString()));

        public static ExtensionConfigContext AddJArrayToKeyListConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<JArray?, IList<CreateKeyData?>>(value => JsonConvert.DeserializeObject<IList<CreateKeyData?>>(value?.ToString()));

        public static ExtensionConfigContext AddStringToByteArrayConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<string, byte[]>(Encoding.UTF8.GetBytes);

        public static ExtensionConfigContext AddByteArrayToStringConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<byte[], string>(Encoding.UTF8.GetString);

        public static ExtensionConfigContext AddJObecjtToStringConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<JObject?, string?>(v => v?.ToString());

        public static ExtensionConfigContext AddStringToJObjectConverter(this ExtensionConfigContext context, Microsoft.Extensions.Logging.ILogger logger)
            => context.AddConverter<string?, JObject?>(JObject.Parse);


    }
}
