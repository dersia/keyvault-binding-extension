# Secrets management

## `KeyVaultSecret`

```json5
{
  type: "keyVaultSecret",
  name: "keyVaultSecret",
  direction: "in", //in and out supported
  keyVaultUrl: "myKeyVaultUrl", //AppSettings name
  secretName: "MySecret", //Secret name, AutoResolve-able
  contentType: "mySuperSecretValue", //Optional
  isEnabled: true, //Optional, true
  expiresAt: "", //Optional, null=never
  startsAt: "" //Optional, null=now
}
```

# How to use

The `KeyVaultSecret` binding can be used to [`retrive` (`in`-binding)][in-binding] or [`store` (`out`-binding)][out-binding] secrets.

## In-Binding

### DotNet

When using C# or F#, the binding supports multiple Types for input bindings

| Type           | Description                                                                                      | Example                                      |
| -------------- | ------------------------------------------------------------------------------------------------ | -------------------------------------------- |
| `SecretBundle` | [`SecretBundle` is a KeyVault-SDK type][1] which can be used to get Metadata of the Secret       | [See example](#inbound-secretbundle-example) |
| `string`       | Returns the secret as string                                                                     | [See example](#inbound-string-example)       |
| `byte[]`       | Returns the secret as byte array, byte arrays are stored as `base64` encoded strings in KeyVault | [See example](#inbound-byte-array-example)   |
| `T`            | Returns the secret as an open type                                                               | [See example](#inbound-open-type-example)    |

#### Inbound SecretBundle example

```c#
[FunctionName("MyKeyVaultFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [KeyVaultSecret("myKeyVaultUrl", "MySecret")]SecretBundle keyVaultSecret,
    ILogger log)
{
    log.LogInformation(keyVaultSecret.Value);
    return (ActionResult)new OkObjectResult($"Secret: {keyVaultSecret.Value}");
}
```

#### Inbound string example

```c#
[FunctionName("MyKeyVaultFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [KeyVaultSecret("myKeyVaultUrl", "MySecret")]string keyVaultSecret,
    ILogger log)
{
    log.LogInformation(keyVaultSecret);
    return (ActionResult)new OkObjectResult($"Secret: {keyVaultSecret}");
}
```

#### Inbound byte array example

```c#
[FunctionName("MyKeyVaultFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [KeyVaultSecret("myKeyVaultUrl", "MySecret")]byte[] keyVaultSecret,
    ILogger log)
{
    var base64Secret = System.Convert.ToBase64String(keyVaultSecret);
    log.LogInformation(base64Secret);
    return (ActionResult)new OkObjectResult($"Secret: {base64Secret}");
}
```

#### Inbound open type example

```c#
[FunctionName("MyKeyVaultFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [KeyVaultSecret("myKeyVaultUrl", "MySecret")]MySecretClass keyVaultSecret,
    ILogger log)
{
    var json = Newtonsoft.Json.JsonConvert.SerializeObject(keyVaultSecret);
    log.LogInformation(json);
    return (ActionResult)new OkObjectResult($"Secret: {json}");
}

class MySecretClass
{
    public string MyValue { get; set; }
    public bool MyBool { get; set; }
}
```

[1]: https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.keyvault.models.secretbundle?view=azure-dotnet-preview

### JavaScript

When using JavaScript, the binding only supports string

#### Inbound javascript example

```javascript
module.exports = async function(context, req) {
  context.log(context.bindings.keyVaultSecret);
  return {
    body: "Secret: " + context.bindings.keyVaultSecret
  };
};
```

## Out-Binding

### DotNet

When using C# or F#, the binding supports multiple Types for output bindings using the `IAsyncCollector<>` interface

| Type               | Description                                                                                                       | Example                                           |
| ------------------ | ----------------------------------------------------------------------------------------------------------------- | ------------------------------------------------- |
| `CreateSecretData` | [`CreateSecretData` is a Type added by the Binding Extensions][2] which can be used to set Metadata of the Secret | [See example](#outbound-createsecretdata-example) |
| `string`           | Stores the secrets as string                                                                                      | [See example](#outbound-string-example)           |
| `byte[]`           | Stores the secret as byte array, byte arrays are stored as `base64` encoded strings in KeyVault                   | [See example](#outbound-byte-array-example)       |
| `T`                | Stores the secret an open type as string                                                                          | [See example](#outbound-open-type-example)        |

#### Outbound CreateSecretData example

```c#
[FunctionName("MyKeyVaultFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [KeyVaultSecret("myKeyVaultUrl", "MySecret")]IAsyncCollector<CreateSecretData> keyVaultSecret,
    ILogger log)
{
    var secret = new CreateSecretData
    {
        SecretName = "Secret1", // string
        Value = "MySecretValue", // string
        ContentType = null, // nullable string
        IsEnabled = null, // nullable bool, defaults to true
        ExpiresAt = null, // nullable DateTime, null means never
        StartsAt = null, // nullable DateTime, null means there is no starting date, so it is starting now
        Tags = null // nullable IDictionary<string, string>
    };
    await keyVaultSecret.AddAync(secret);
    var json = Newtonsoft.Json.JsonConvert.SerializeObject(secret);
    log.LogInformation(json);
    return (ActionResult)new OkObjectResult($"Secret: {json}");
}
```

#### Outbound string example

```c#
[FunctionName("MyKeyVaultFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [KeyVaultSecret("myKeyVaultUrl", "MySecret")]IAsyncCollector<string> keyVaultSecret,
    ILogger log)
{
    string secret = "MyValue";
    await keyVaultSecret.AddAync(secret);
    log.LogInformation(secret);
    return (ActionResult)new OkObjectResult($"Secret: {secret}");
}
```

#### Outbound byte array example

```c#
[FunctionName("MyKeyVaultFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [KeyVaultSecret("myKeyVaultUrl", "MySecret")]IAsyncCollector<byte[]> keyVaultSecret,
    ILogger log)
{
    var secret = Encoding.UTF8.GetBytes("MyValue");
    await keyVaultSecret.AddAsync(secret);
    var base64Secret = System.Convert.ToBase64String(secret);
    log.LogInformation(base64Secret);
    return (ActionResult)new OkObjectResult($"Secret: {base64Secret}");
}
```

#### Outbound open type example

```c#
[FunctionName("MyKeyVaultFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [KeyVaultSecret("myKeyVaultUrl", "MySecret")]IAsyncCollector<MySecretClass> keyVaultSecret,
    ILogger log)
{
    var secret = new MySecretClass
    {
        MyValue = "MyValue",
        MyBool = true
    };
    await keyVaultSecret.AddAsync(secret);
    var json = Newtonsoft.Json.JsonConvert.SerializeObject(secret);
    log.LogInformation(json);
    return (ActionResult)new OkObjectResult($"Secret: {json}");
}

class MySecretClass
{
    public string MyValue { get; set; }
    public bool MyBool { get; set; }
}
```

[2]: ../blob/master/src/SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension/Models/CreateSecretData.cs

### JavaScript

When using JavaScript, the binding supports `string arrays` and `CreateSecretData arrays`

#### Outbound javascript string example

```javascript
module.exports = async function(context, req) {
  let secret = "MySecret";
  context.bindings.keyVaultSecret.push(secret);
  context.log(secret);
  return {
    body: "Secret: " + secret
  };
};
```

#### Outbound javascript CreateSecretData example

```javascript
module.exports = async function(context, req) {
  let secret = {
    secretName: "Secret1", // string
    value: "MySecretValue", // string
    contentType: null, // nullable string
    isEnabled: null, // nullable bool, defaults to true
    expiresAt: null, // nullable DateTime, null means never
    startsAt: null // nullable DateTime, null means there is no starting date, so it is starting now
  };
  context.bindings.keyVaultSecret.push(secret);
  context.log(secret);
  return {
    body: "Secret: " + secret
  };
};
```
