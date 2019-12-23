using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
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
            builder.AddExtension<KeyVaultBlobsExtensionConfigProvider>();

            return builder;
        }
    }
}
