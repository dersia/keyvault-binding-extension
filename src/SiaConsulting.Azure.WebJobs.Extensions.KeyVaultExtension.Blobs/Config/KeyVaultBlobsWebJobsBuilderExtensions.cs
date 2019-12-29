using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.DependencyInjection;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.BindingProvider;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Config
{
    public static class KeyVaultBlobsWebJobsBuilderExtensions
    {
        public static IWebJobsBuilder AddKeyVaultBlobs(this IWebJobsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddSingleton<IKeyVaultClient, KeyVaultClient>(s => new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(new AzureServiceTokenProvider().KeyVaultTokenCallback)));
            builder.Services.AddSingleton<IKeyNameProvider, KeyNameProvider>();
            builder.Services.AddSingleton<IBindingProvider, KeyNameProviderBindingProvider>();
            builder.AddExtension<KeyVaultBlobsExtensionConfigProvider>();

            return builder;
        }
    }
}
