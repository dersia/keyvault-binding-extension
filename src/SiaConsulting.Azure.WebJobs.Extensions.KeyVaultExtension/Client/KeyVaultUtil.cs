using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Newtonsoft.Json;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Extensions;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Common.Extensions;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Models;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Common.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Client
{
    public class KeyVaultUtil : IKeyVaultUtil
    {
        private readonly KeyVaultAttribute _config;
        private readonly IKeyVaultClient _keyVaultClient;

        public KeyVaultUtil(KeyVaultAttribute config, IKeyVaultClient keyVaultClient)
        {
            _config = config;
            _keyVaultClient = keyVaultClient;
        }

        public Task<SecretBundle> GetSecret(string secretName) 
            => _keyVaultClient.GetSecretAsync(_config.KeyVaultUrl, secretName);
        public async Task<string> GetSecretAsString(string secretName) 
            => (await GetSecret(secretName)).Value;
        public async Task<byte[]> GetSecretAsByteArray(string secretName) 
            => Convert.FromBase64String((await GetSecret(secretName)).Value);
        public async Task<T> GetSecretAs<T>(string secretName)
            => JsonConvert.DeserializeObject<T>((await GetSecret(secretName)).Value);
        public Task CreateSecret(CreateSecretData createSecretData)
            => _keyVaultClient.SetSecretAsync(_config.KeyVaultUrl, createSecretData.SecretName, createSecretData.Value, createSecretData.Tags, createSecretData.ContentType, createSecretData.CreateAttributes());
        public Task CreateSecret(string secretName, string secret) 
            => CreateSecret(new CreateSecretData { SecretName = secretName, Value = secret });
        public Task CreateSecret(string secretName, byte[] secret) 
            => CreateSecret(new CreateSecretData { SecretName = secretName, Value = Convert.ToBase64String(secret) });
        public Task CreateSecret<T>(string secretName, T secret) 
            => CreateSecret(new CreateSecretData { SecretName = secretName, Value = JsonConvert.SerializeObject(secret) });
        public Task<KeyBundle> GetKey(string keyName)
            => _keyVaultClient.GetKeyAsync(_config.KeyVaultUrl, keyName);
        public Task<KeyBundle> CreateKey(CreateKeyData createKeyData)
            => _keyVaultClient.CreateKeyAsync(_config.KeyVaultUrl, createKeyData.KeyName, createKeyData.KeyType.MapKeyType(), createKeyData.KeySize, null, createKeyData.CreateAttributes(), createKeyData.Tags, createKeyData.KeyCurve.MapKeyCurve());
        public async Task<string> Encrypt(string keyName, string keyVersion, EncryptionAlgorithm encryptionAlgorithm, string value)
            => Convert.ToBase64String(await Encrypt(keyName, keyVersion, encryptionAlgorithm, Encoding.UTF8.GetBytes(value)));
        public async Task<byte[]> Encrypt(string keyName, string keyVersion, EncryptionAlgorithm encryptionAlgorithm, byte[] value)
            => (await _keyVaultClient.EncryptAsync(_config.KeyVaultUrl, keyName, keyVersion, encryptionAlgorithm.MapAlogrithm(), value)).Result;
        public async Task<string> Decrypt(string keyName, string keyVersion, EncryptionAlgorithm encryptionAlgorithm, string base64EncryptedValue) 
            => Encoding.UTF8.GetString(await Decrypt(keyName, keyVersion, encryptionAlgorithm, Convert.FromBase64String(base64EncryptedValue)));
        public async Task<byte[]> Decrypt(string keyName, string keyVersion, EncryptionAlgorithm encryptionAlgorithm, byte[] value) 
            => (await _keyVaultClient.DecryptAsync(_config.KeyVaultUrl, keyName, keyVersion, encryptionAlgorithm.MapAlogrithm(), value)).Result;

        public async Task<string> WrapKey(string keyName, string keyVersion, EncryptionAlgorithm encryptionAlgorithm, string value)
            => Convert.ToBase64String(await WrapKey(keyName, keyVersion, encryptionAlgorithm, Encoding.UTF8.GetBytes(value)));
        public async Task<byte[]> WrapKey(string keyName, string keyVersion, EncryptionAlgorithm encryptionAlgorithm, byte[] value)
            => (await _keyVaultClient.WrapKeyAsync(_config.KeyVaultUrl, keyName, keyVersion, encryptionAlgorithm.MapAlogrithm(), value)).Result;
        public async Task<string> UnwrapKey(string keyName, string keyVersion, EncryptionAlgorithm encryptionAlgorithm, string base64EncryptedValue)
            => Encoding.UTF8.GetString(await UnwrapKey(keyName, keyVersion, encryptionAlgorithm, Convert.FromBase64String(base64EncryptedValue)));
        public async Task<byte[]> UnwrapKey(string keyName, string keyVersion, EncryptionAlgorithm encryptionAlgorithm, byte[] value)
            => (await _keyVaultClient.UnwrapKeyAsync(_config.KeyVaultUrl, keyName, keyVersion, encryptionAlgorithm.MapAlogrithm(), value)).Result;
    }
}
