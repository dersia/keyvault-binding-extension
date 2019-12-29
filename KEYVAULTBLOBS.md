This is a binding extension for Azure Functions

> # Because Security Matters

This binding extension enables [Azure Functions][2] to use [Azure KeyVault][1] to encrypt and decrypt [Azure Blob Storage][3].

The extension supports output bindings and input bindings.

Important: the output binding uses transactions, this means, that the added blobs will only be flushed, if the function completes successfully

# Changelog

## Version 0.0.3 - Fixes & IKeyNameProvider

- Attribute changed to Allow `null` for BlobPath and KeyName
- Allow Binding for IKeyNameProvider to set KeyName per BlobPath

## Version 0.0.2.1 - Minor Dependency fix

- Fixed dependency bug

## Version 0.0.2 - Encryption and Decryption

- Added `EncryptedBlob` Binding

## Version 0.0.1 - Initial project start

How to install

1. Create an Function App in the portal
2. Create a new Function within your function app
3. Get your functions url and you masterkey
4. use Postman or Curl to post the following to the extensions endpoint of your function app. If you functions url is `https://MyEncryptedBlobFunction.azurewebsites.net/api/HttpTrigger1?code=ABC` then your extensions endpoint is `https://MyEncryptedBlobFunction.azurewebsites.net/admin/host/extensions?code=ABC`
   ```json
   {
     "Id": "SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs",
     "Version": "0.0.2.1"
   }
   ```
5. check with the returned jobid, if the job to be completed / the extension is installed `https://MyEncryptedBlobFunction.azurewebsites.net/admin/host/extensions/jobs/<JOBID>?code=ABC`
6. setup your function.json with all the needed parameters
7. start using the funtion

How to unsintall

There is a problem with uninstalling extensions right now, so the easiest way is to delete the functions app create a new one.
If you still want to uninstall the extension, this is how to do it

1. Stop the function app
2. Use Azure-Portal or Azure Storage Explorer to connect to the storage account file shares of your function app
3. Delete `SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.dll` from `site/wwwroot/bin`
4. Edit `extensions.json` in `site/wwwroot/bin` and remove the `SiaConsulting.AzureWebJobs.Extensions.KeyVaultExtension.Blobs`-extension from the array
5. Edit `extensions.deps.json` in `site/wwwroot/bin` and remove any occurance of `SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs`
6. Edit `extensions.csproj` in `site/wwwroot` and remove the `PackageReference` for `SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs`
7. Start your function app

# Running in Azure

This Binding Extension uses MSI to authenticate against [`Azure KeyVault`][1] when running in Azure
Please make sure that the MSI has all needed Access Rights

# Running local

This Binding Extension needs to authenticate against [`Azure KeyVault`][1] when running local.
You need to create a new Service Principle and give it rights to use KeyVault:

## Create new SP using Azure CLI:

```bash
az ad sp create-for-rbac --sdk-auth
Creating a role assignment under the scope of "/subscriptions/<your-subscription-id>"
  Retrying role assignment creation: 1/36
{
  "clientId": "<created-client-id>",
  "clientSecret": "<created-client-secret>",
  "subscriptionId": "<your-subscription-id>",
  "tenantId": "<your-tenant-id>",
  "activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
  "resourceManagerEndpointUrl": "https://management.azure.com/",
  "activeDirectoryGraphResourceId": "https://graph.windows.net/",
  "sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
  "galleryEndpointUrl": "https://gallery.azure.com/",
  "managementEndpointUrl": "https://management.core.windows.net/"
}
```

Take your `clientId`, `clientSecret` and `tenantId` to create a Environment Variable to authenticate:

#### using bash

```bash
export AzureServicesAuthConnectionString="RunAs=App;AppId=<clientId>;TenantId=<tenantId>;AppKey=<clientSecret>"
```

#### using windows powershell/command line

```powershell
setx AzureServicesAuthConnectionString "RunAs=App;AppId=<clientId>;TenantId=<tenantId>;AppKey=<clientSecret>"
```

After that you need to restart, VS, VSCode or any running version of the `Functions-Core-Tools`

# Access Policy needed for MSI/SP on Azure KeyVault

| Binding Function | Mode                      | KeyVault Policy                |
| ---------------- | ------------------------- | ------------------------------ |
| EncryptedBlob    | CreateKeyIfExists = false | Key Permission Encrypt         |
| EncryptedBlob    | CreateKeyIfExists = true  | Key Permission Encrypt, Create |

# Features and examples:

# BlobEncryption / BlobDecryption

## `EncryptedBlob`

```json5
{
  type: "encryptedBlob",
  name: "encryptedBlobFile",
  direction: "in", //only in supported for encrypting
  keyVaultConnectionString: "myKeyVaultUrl", //AppSettings name
  blobConnectionString: "myStorageConnectionString", //AppSettings name
  keyName: "MyKey", //Key name, AutoResolve-able
  blobPath: "container/blob-path", //optional when using IAsyncCollector
  createKeyIfNotExistst: false, //Optional, defaults to false, creates a key, if key does not exists in KeyVault
  keyType: "RSA", // Optional, defaults to RSA , will be used if createKeyIfNotExistst set to true
  keyCurve: "P_256", // Optional, will be used if createKeyIfNotExistst set to true
  keySize: 1024 //Optional, will be used if createKeyIfNotExistst set to true
}
```

# How to use

The `encryptedBlob` binding can be used to encrypt or decrypt a blob using Azure KeyVault.

### DotNet

When using C# or F#, the binding supports multiple Types

| Direction | Type                             | Description                                       | Example                                     |
| --------- | -------------------------------- | ------------------------------------------------- | ------------------------------------------- |
| `Out`     | `IAsyncCollector<EncryptedBlob>` | Type to create Blobs with metadata and properties | [See example](#outbound-collector-example)  |
| `Out`     | `out byte[]`                     | creates an encrypted blob using blob-path         | [See example](#outbound-byte-array-example) |
| `Out`     | `out string`                     | creates an encrypted blob using blob-path         | [See example](#outbound-string-example)     |
| `Out`     | `out T`                          | creates an encrypted blob using blob-path         | [See example](#outbound-t-example)          |
| `In`      | `Stream`                         | fetches and decrypts a blob using blob-path       | [See example](#inbound-stream-example)      |
| `In`      | `byte[]`                         | fetches and decrypts a blob using blob-path       | [See example](#inbound-byte-array-example)  |
| `In`      | `string`                         | fetches and decrypts a blob using blob-path       | [See example](#inbound-string-example)      |
| `In`      | `T`                              | fetches and decrypts a blob using blob-path       | [See example](#inbound-t-example)           |

#### Outbound collector example

```c#
#r "SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs"
#r "Microsoft.Azure.Storage.Blob"
#r "../bin/Microsoft.Azure.Storage.Common.dll"
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Models;
using Microsoft.Azure.Storage.Blob;

[FunctionName("MyEncryptedBlobFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req,
    [EncryptedBlob("myKeyVaultUrl", "myStorageConnectionString", null, "MyKey", FileAccess.Write)]IAsyncCollector<EncryptedBlob> encrypterdBlobs,
    ILogger log)
{
    await encrypterdBlobs.AddAsync(EncryptedBlob.FromStream(new BlobPath("container", "blob-path"), req.Body, new BlobProperties { ContentType = req.ContentType }));
    return (ActionResult)new OkResult();
}
```

#### Outbound byte array example

```c#
#r "SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs"
#r "Microsoft.Azure.Storage.Blob"
#r "../bin/Microsoft.Azure.Storage.Common.dll"
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Models;
using Microsoft.Azure.Storage.Blob;

[FunctionName("MyEncryptedBlobFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req,
    ILogger log,
    [EncryptedBlob("myKeyVaultUrl", "myStorageConnectionString", "container/blob-path", "MyKey", FileAccess.Write)] out byte[] encryptedBlob)
{
    var encryptedBlob = req.Body.ToArray();
    return (ActionResult)new OkResult();
}
```

#### Outbound string example

```c#
#r "SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs"
#r "Microsoft.Azure.Storage.Blob"
#r "../bin/Microsoft.Azure.Storage.Common.dll"
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Models;
using Microsoft.Azure.Storage.Blob;

[FunctionName("MyEncryptedBlobFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req,
    ILogger log,
    [EncryptedBlob("myKeyVaultUrl", "myStorageConnectionString", "container/blob-path", "MyKey", FileAccess.Write)] out string encryptedBlob)
{
    var bytes = req.Body.ToArray();
    encryptedBlob = System.Text.Encoding.UTF8.GetString(bytes);
    return (ActionResult)new OkResult();
}
```

#### Outbound T example

```c#
#r "SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs"
#r "Microsoft.Azure.Storage.Blob"
#r "../bin/Microsoft.Azure.Storage.Common.dll"
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Models;
using Microsoft.Azure.Storage.Blob;

[FunctionName("MyEncryptedBlobFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req,
    ILogger log,
    [EncryptedBlob("myKeyVaultUrl", "myStorageConnectionString", "container/blob-path", "MyKey", FileAccess.Write)] out Test encryptedBlob)
{
    var encryptedBlob = req.Body.ToArray();
    var bytes = req.Body.ToArray();
    var testObject = new Test();
    testObject.Data = System.Text.Encoding.UTF8.GetString(bytes);
    encryptedBlob = textObject;
    return (ActionResult)new OkResult();
}

public class Test
{
    public string Data { get; set; }
}
```

#### Inbound stream example

```c#
using System.IO;

[FunctionName("MyEncryptedBlobFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [EncryptedBlob("myKeyVaultUrl", "myStorageConnectionString", "container/blob-path", "MyKey", FileAccess.Read)]Stream encrypterdBlobs,
    ILogger log)
{
    return new FileStreamResult(encrypterdBlobs, "application/octet-stream");
}
```

#### Inbound byte array example

```c#
[FunctionName("MyEncryptedBlobFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [EncryptedBlob("myKeyVaultUrl", "myStorageConnectionString", "container/blob-path", "MyKey", FileAccess.Read)] byte[] encryptedBlob,
    ILogger log)
{
    return new FileContentResult(encrypterdBlobs, "application/octet-stream");
}
```

#### Outbound string example

```c#
[FunctionName("MyEncryptedBlobFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [EncryptedBlob("myKeyVaultUrl", "myStorageConnectionString", "container/blob-path", "MyKey", FileAccess.Read)] string encryptedBlob,
    ILogger log)
{
    return new OkObjectResult(encryptedBlob);
}
```

#### Outbound T example

```c#
[FunctionName("MyEncryptedBlobFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [EncryptedBlob("myKeyVaultUrl", "myStorageConnectionString", "container/blob-path", "MyKey", FileAccess.Read)] Test encryptedBlob,
    ILogger log)
{
    return new OkObjectResult(encryptedBlob);
}

public class Test
{
    public string Data { get; set; }
}
```

### JavaScript

When using JavaScript, the binding supports binary and string

#### Outbound javascript example

```javascript
module.exports = async function(context, req) {
  context.bindings.encrypterdValue = req.body;
  return {
    body: "ok"
  };
};
```

#### Inbound javascript example

```javascript
module.exports = async function(context, req) {
  return {
    body: context.bindings.encrypterdValue
  };
};
```

[1]: https://azure.microsoft.com/en-us/services/key-vault
[2]: https://azure.microsoft.com/en-us/services/functions/
[3]: https://azure.microsoft.com/en-us/services/storage/blobs
