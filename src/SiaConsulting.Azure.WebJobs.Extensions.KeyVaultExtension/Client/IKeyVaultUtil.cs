using Microsoft.Azure.KeyVault.Models;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Common.Models;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Models;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Client
{
    public interface IKeyVaultUtil
    {
        Task<SecretBundle> GetSecret(string secretName);
        Task<string> GetSecretAsString(string secretName);
        Task<byte[]> GetSecretAsByteArray(string secretName);
        Task<T> GetSecretAs<T>(string secretName);
        Task CreateSecret(CreateSecretData createSecretData);
        Task CreateSecret(string secretName, string secret);
        Task CreateSecret(string secretName, byte[] secret);
        Task CreateSecret<T>(string secretName, T secret);
        Task<KeyBundle> GetKey(string keyName);
        Task<KeyBundle> CreateKey(CreateKeyData createKeyData);
        Task<string> Encrypt(string keyName, string keyVersion, EncryptionAlgorithm encryptionAlgorithm, string value);
        Task<byte[]> Encrypt(string keyName, string keyVersion, EncryptionAlgorithm encryptionAlgorithm, byte[] value);
        Task<string> Decrypt(string keyName, string keyVersion, EncryptionAlgorithm encryptionAlgorithm, string base64EncryptedValue);
        Task<byte[]> Decrypt(string keyName, string keyVersion, EncryptionAlgorithm encryptionAlgorithm, byte[] value);

        Task<string> WrapKey(string keyName, string keyVersion, EncryptionAlgorithm encryptionAlgorithm, string value);
        Task<byte[]> WrapKey(string keyName, string keyVersion, EncryptionAlgorithm encryptionAlgorithm, byte[] value);
        Task<string> UnwrapKey(string keyName, string keyVersion, EncryptionAlgorithm encryptionAlgorithm, string base64EncryptedValue);
        Task<byte[]> UnwrapKey(string keyName, string keyVersion, EncryptionAlgorithm encryptionAlgorithm, byte[] value);
    }
}
