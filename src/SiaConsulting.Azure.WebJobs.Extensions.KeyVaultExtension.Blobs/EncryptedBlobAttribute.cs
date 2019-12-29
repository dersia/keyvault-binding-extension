using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using System;
using System.IO;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Common.Models;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [ConnectionProvider(typeof(StorageAccountAttribute))]
    [Binding]
    public class EncryptedBlobAttribute : Attribute
    {
        private FileAccess? _access;
        public EncryptedBlobAttribute(string keyVaultConnectionString, string blobConnectionString)
        {
            BlobConnectionString = blobConnectionString;
            BlobPath = null;
            KeyName = null;
            KeyVaultConnectionString = keyVaultConnectionString;
            Access = null;
        }

        public EncryptedBlobAttribute(string keyVaultConnectionString, string blobConnectionString, string keyName)
        {
            BlobConnectionString = blobConnectionString;
            BlobPath = null;
            KeyName = keyName;
            KeyVaultConnectionString = keyVaultConnectionString;
            Access = null;
        }

        public EncryptedBlobAttribute(string keyVaultConnectionString, string blobConnectionString, string keyName, string blobPath)
        {
            BlobConnectionString = blobConnectionString;
            BlobPath = blobPath;
            KeyName = keyName;
            KeyVaultConnectionString = keyVaultConnectionString;
            Access = null;
        }

        public EncryptedBlobAttribute(string keyVaultConnectionString, string blobConnectionString, FileAccess access)
        {
            BlobConnectionString = blobConnectionString;
            BlobPath = null;
            KeyName = null;
            KeyVaultConnectionString = keyVaultConnectionString;
            Access = access;
        }

        public EncryptedBlobAttribute(string keyVaultConnectionString, string blobConnectionString, FileAccess access, string keyName)
        {
            BlobConnectionString = blobConnectionString;
            BlobPath = null;
            KeyName = keyName;
            KeyVaultConnectionString = keyVaultConnectionString;
            Access = access;
        }

        public EncryptedBlobAttribute(string keyVaultConnectionString, string blobConnectionString, FileAccess access, string keyName, string blobPath)
        {
            BlobConnectionString = blobConnectionString;
            BlobPath = blobPath;
            KeyName = keyName;
            KeyVaultConnectionString = keyVaultConnectionString;
            Access = access;
        }

        [AppSetting]
        public string KeyVaultConnectionString { get; set; }
        [AppSetting]
        public string BlobConnectionString { get; set; }

        [AutoResolve]
        public string? BlobPath { get; }

        [AutoResolve]
        public string? KeyName { get; }
        public FileAccess? Access
        {
            get => _access;
            set => _access = value;
        }

        public bool CreateKeyIfNotExistst { get; set; } = false;
        public KeyType? KeyType { get; set; } = Common.Models.KeyType.RSA;
        public KeyCurves? KeyCurve { get; set; }
        public int? KeySize { get; set; }
    }
}
