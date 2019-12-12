# Keys management

## `KeyVaultKey`

```json5
{
  type: "keyVaultKey",
  name: "keyVaultKey",
  direction: "in", //in and out supported
  keyVaultUrl: "myKeyVaultUrl", //AppSettings name
  keyName: "MyKey", //Key name, AutoResolve-able
  keyType: "RSA", // Values: EC, EC_HSM, RSA, RSA_HSM, oct - defaults to RSA
  keyCurves: "P_256", //Optional, Values: P_256, P_384, P_521, SECP256K1
  keySize: 1024, //Optional
  isEnabled: true, //Optional, true
  expiresAt: "", //Optional, null=never
  startsAt: "" //Optional, null=now
}
```

# How to use

The `KeyVaultKey` binding can be used to [`retrive` (`in`-binding)][in-binding] or [`create` (`out`-binding)][out-binding] keys.

## In-Binding

### DotNet

When using C# or F#, the binding supports multiple Types for input bindings

| Type         | Description                                                                       | Example                                   |
| ------------ | --------------------------------------------------------------------------------- | ----------------------------------------- |
| `KeyBundle`  | [`KeyBundle` is a KeyVault-SDK type][1] which can be used to get Metadata of Keys | [See example][inbound keybundle example]  |
| `JsonWebKey` | [`JsonWebKey` is a KeyVault-SDK type][2]                                          | [See example][inbound jsonwebkey example] |

#### Inbound KeyBundle example

```c#
[FunctionName("MyKeyVaultFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [KeyVaultKey("myKeyVaultUrl", "MyKey")]KeyBundle keyVaultKey,
    ILogger log)
{
    var key = Newtonsoft.Json.JsonConvert.SerializeObject(keyVaultKey.Key);
    log.LogInformation(key);
    return (ActionResult)new OkObjectResult($"Key: {key}");
}
```

#### Inbound JsonWebKey example

```c#
[FunctionName("MyKeyVaultFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [KeyVaultKey("myKeyVaultUrl", "MyKey")]JsonWebKey keyVaultKey,
    ILogger log)
{
    var key = Newtonsoft.Json.JsonConvert.SerializeObject(keyVaultKey);
    log.LogInformation(key);
    return (ActionResult)new OkObjectResult($"Key: {key}");
}
```

[1]: https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.keyvault.models.keybundle?view=azure-dotnet-preview
[2]: https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.keyvault.webkey.jsonwebkey?view=azure-dotnet-legacy&viewFallbackFrom=azure-dotnet-preview

### JavaScript

When using JavaScript, the binding only supports KeyBundle

#### Inbound javascript example

```javascript
module.exports = async function(context, req) {
  context.log(context.bindings.keyVaultKey);
  return {
    body: "Key: " + context.bindings.keyVaultKey
  };
};
```

## Out-Binding

### DotNet

When using C# or F#, the binding supports only `CreateKeyData` using the `IAsyncCollector<>` interface

| Type            | Description                                                                                                 | Example                                       |
| --------------- | ----------------------------------------------------------------------------------------------------------- | --------------------------------------------- |
| `CreateKeyData` | [`CreateKeyData` is a Type added by the Binding Extensions][3] which can be used to set Metadata of the Key | [See example][outbound createkeydata example] |

#### Outbound CreateKeyData example

```c#
[FunctionName("MyKeyVaultFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [KeyVaultKey("myKeyVaultUrl", "MyKey")]IAsyncCollector<CreateKeyData> keyVaultKey,
    ILogger log)
{
    var key = new CreateKeyData
    {
        KeyName = "Key1", // string
        KeyType = KeyType.RSA, // KeyType enum
        KeyCurve = null, // nullable, KeyCurves enum
        KeySize = null, // nullable int
        IsEnabled = null, // nullable bool, defaults to true
        ExpiresAt = null, // nullable DateTime, null means never
        StartsAt = null, // nullable DateTime, null means there is no starting date, so it is starting now
        Tags = null // nullable IDictionary<string, string>
    };
    await keyVaultKey.AddAync(key);
    var json = Newtonsoft.Json.JsonConvert.SerializeObject(key);
    log.LogInformation(json);
    return (ActionResult)new OkObjectResult($"Key: {json}");
}
```

[3]: ../blob/master/src/SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension/Models/CreateKeyData.cs

### JavaScript

When using JavaScript, the binding supports only `CreateKeyData arrays`

#### Outbound javascript CreateKeyData example

```javascript
module.exports = async function(context, req) {
  let key = {
    keyName: "Key1", // string
    keyType: "RSA", // string
    keyCurve: null, // nullable KeyCurves
    keySize: null, // nullable number
    isEnabled: null, // nullable bool, defaults to true
    expiresAt: null, // nullable DateTime, null means never
    startsAt: null // nullable DateTime, null means there is no starting date, so it is starting now
  };
  context.bindings.keyVaultKey.push(key);
  context.log(key);
  return {
    body: "Key: " + key
  };
};
```
