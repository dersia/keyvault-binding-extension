# WrapKey

## `KeyVaultWrapKey`

```json5
{
  type: "keyVaultWrapKey",
  name: "wrappedKey",
  direction: "in", //only in supported
  keyVaultUrl: "myKeyVaultUrl", //AppSettings name
  keyName: "MyKey", //Key name, AutoResolve-able
  keyVersion: "", //KeyVault key-version, nullable will be resolved to latest
  algorithm: "RSA1_5", // Values: RSA_OAEP, RSA1_5 - defaults to RSA1_5
  value: "{Query.myQueryData}", // Key Value to be wrapped, can be static string, or AutoResolved with binding expressions
  createKeyIfNotExistst: false, //Optional, defaults to false, creates a key, if key does not exists in KeyVault
  keyType: "RSA", // Optional, defaults to RSA , will be used if createKeyIfNotExistst set to true
  keyCurve: "P_256", // Optional, will be used if createKeyIfNotExistst set to true
  keySize: 1024 //Optional, will be used if createKeyIfNotExistst set to true
}
```

# How to use

The `keyVaultWrapKey` binding can be used to wrap a key using Azure KeyVault.

### DotNet

When using C# or F#, the binding supports multiple Types

| Type     | Description                                      | Example                                    |
| -------- | ------------------------------------------------ | ------------------------------------------ |
| `string` | UTF8 encoded Text representing the encoded bytes | [See example](#inbound-string-example)     |
| `byte[]` | encoded bytes                                    | [See example](#inbound-byte-array-example) |

#### Inbound string example

```c#
[FunctionName("MyKeyVaultFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [KeyVaultWrapKey("myKeyVaultUrl", "MyKeyToWrapKeyWith")]string wrappedKey,
    ILogger log)
{
    log.LogInformation(wrappedKey);
    return (ActionResult)new OkObjectResult($"WrappedKey: {wrappedKey}");
}
```

#### Inbound byte array example

```c#
[FunctionName("MyKeyVaultFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [KeyVaultWrapKey("myKeyVaultUrl", "MyKey")]byte[] wrappedKey,
    ILogger log)
{
    var wrappedKeyString = System.Text.Encoding.UTF8.GetString(wrappedKey);
    log.LogInformation(wrappedKeyString);
    return (ActionResult)new OkObjectResult($"WrappedKey: {wrappedKeyString}");
}
```

### JavaScript

When using JavaScript, the binding supports binary and string

#### Inbound javascript example

```javascript
module.exports = async function(context, req) {
  context.log(context.bindings.wrappedKey);
  return {
    body: "WrappedKey: " + context.bindings.wrappedKey
  };
};
```
