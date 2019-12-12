This is a binding extension for Azure Functions

> # Because Security Matters

This binding extension enables [`Azure Functions`][2] to use [`Azure KeyVault`][1] to [_store and retrive secrets_, _create and retrive keys_ and _encrypt and decrypt values_](https://docs.microsoft.com/en-us/azure/key-vault/about-keys-secrets-and-certificates).

For the future there are more capabilities planned like _singin_ and _certificates_.

The extension supports output bindings and input bindings.

Important: the output binding uses transactions, this means, that the added secrets and keys will only be flushed, if the function completes successfully

# Changelog

## Version 0.0.3 - Downport to netstandard 2.0

- Downported to netstandard 2.0 to support Functions V2

## Version 0.0.2 - Encryption and Decryption

- Added `encryption` and `decryption` support

## Version 0.0.1 - Initial project start

- Added `secret` management
- Added `key` management

How to install

1. Create an Function App in the portal
2. Create a new Function within your function app
3. Get your functions url and you masterkey
4. use Postman or Curl to post the following to the extensions endpoint of your function app. If you functions url is `https://MyKeyVaultFunction.azurewebsites.net/api/HttpTrigger1?code=ABC` then your extensions endpoint is `https://MyKeyVaultFunction.azurewebsites.net/admin/host/extensions?code=ABC`
   ```json
   {
     "Id": "SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension",
     "Version": "0.0.3"
   }
   ```
5. check with the returned jobid, if the job to be completed / the extension is installed `https://MyKeyVaultFunction.azurewebsites.net/admin/host/extensions/jobs/<JOBID>?code=ABC`
6. setup your function.json with all the needed parameters
7. start using the funtion

How to unsintall

There is a problem with uninstalling extensions right now, so the easiest way is to delete the functions app create a new one.
If you still want to uninstall the extension, this is how to do it

1. Stop the function app
2. Use Azure-Portal or Azure Storage Explorer to connect to the storage account file shares of your function app
3. Delete `SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.dll` from `site/wwwroot/bin`
4. Edit `extensions.json` in `site/wwwroot/bin` and remove the `SiaConsulting.AzureWebJobs.Extensions.KeyVaultExtension`-extension from the array
5. Edit `extensions.deps.json` in `site/wwwroot/bin` and remove any occurance of `SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension`
6. Edit `extensions.csproj` in `site/wwwroot` and remove the `PackageReference` for `SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension`
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

| Binding Function  | Mode                      | KeyVault Policy                |
| ----------------- | ------------------------- | ------------------------------ |
| KeyVaultSecret    | In-Binding                | Secret Permission Get          |
| KeyVaultSecret    | Out-Binding               | Secret Permission Set          |
| KeyVaultKeys      | In-Binding                | Key Permission Get             |
| KeyVaultKeys      | Out-Binding               | Key Permission Create          |
| KeyVaultEncrption | CreateKeyIfExists = false | Key Permission Encrypt         |
| KeyVaultEncrption | CreateKeyIfExists = true  | Key Permission Encrypt, Create |
| KeyVaultDecrption | CreateKeyIfExists = false | Key Permission Decrypt         |
| KeyVaultDecrption | CreateKeyIfExists = true  | Key Permission Decrypt, Create |

# Features and examples:

- [Secrets](./SECRETS.MD)
- [Keys](./KEYS.MD)
- [Encryption](./ENCRYPTION.MD)
- [Decryption](./DECRYPTION.MD)
- Signing
- Certificates

[1]: https://azure.microsoft.com/en-us/services/key-vault
[2]: https://azure.microsoft.com/en-us/services/functions/
