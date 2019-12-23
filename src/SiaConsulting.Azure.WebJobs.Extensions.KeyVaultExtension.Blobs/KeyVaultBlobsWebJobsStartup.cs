using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Config;

[assembly: WebJobsStartup(typeof(KeyVaultBlobsWebJobsStartup))]
namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs
{
    public class KeyVaultBlobsWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
            => builder.AddKeyVaultBlobs();
    }
}
