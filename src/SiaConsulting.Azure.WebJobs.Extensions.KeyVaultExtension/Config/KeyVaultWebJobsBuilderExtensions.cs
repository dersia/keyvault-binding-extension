using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Config
{
    public static class KeyVaultWebJobsBuilderExtensions
    {
        public static IWebJobsBuilder AddKeyVault(this IWebJobsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddSingleton<IKeyVaultClient, KeyVaultClient>(s => new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(new AzureServiceTokenProvider().KeyVaultTokenCallback)));
            builder.AddExtension<KeyVaultConfigProvider>();

            return builder;
        }
    }
}
