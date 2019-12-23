using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Common.Models;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [ConnectionProvider(typeof(StorageAccountAttribute))]
    [Binding]
    public class EncryptedBlobAttribute : Attribute
    {
        private FileAccess? _access;
        public EncryptedBlobAttribute(string keyVaultConnectionString, string blobConnectionString, string blobPath, string keyName)
        {
            BlobConnectionString = blobConnectionString;
            BlobPath = blobPath;
            KeyName = keyName;
            KeyVaultConnectionString = keyVaultConnectionString;
        }
            
        public EncryptedBlobAttribute(string keyVaultConnectionString, string blobConnectionString, string blobPath, string keyName, FileAccess access)
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
        public string BlobPath { get; }

        [AutoResolve]
        public string KeyName { get; }
        public FileAccess? Access
        {
            get { return _access; }
            set { _access = value; }
        }

        public bool CreateKeyIfNotExistst { get; set; } = false;
        public KeyType? KeyType { get; set; } = Common.Models.KeyType.RSA;
        public KeyCurves? KeyCurve { get; set; }
        public int? KeySize { get; set; }
    }
}
