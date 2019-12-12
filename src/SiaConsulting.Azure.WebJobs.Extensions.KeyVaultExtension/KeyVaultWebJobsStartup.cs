using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Config;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Azure.WebJobs;

[assembly: WebJobsStartup(typeof(KeyVaultWebJobsStartup))]
namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension
{
    public class KeyVaultWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder) 
            => builder.AddKeyVault();
    }
}
