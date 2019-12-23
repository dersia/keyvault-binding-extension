# UnwrapKey

## `KeyVaultUnwrapKey`

```json5
{
  type: "keyVaultUnwrapKey",
  name: "unwrappedKey",
  direction: "in", //only in supported
  keyVaultUrl: "myKeyVaultUrl", //AppSettings name
  keyName: "MyKey", //Key name, AutoResolve-able
  keyVersion: "", //KeyVault key-version, nullable will be resolved to latest
  algorithm: "RSA1_5", // Values: RSA_OAEP, RSA1_5 - defaults to RSA1_5
  value: "{Query.myQueryData}", // Key Value to be unwrapped, can be static string, or AutoResolved with binding expressions
  createKeyIfNotExistst: false, //Optional, defaults to false, creates a key, if key does not exists in KeyVault
  keyType: "RSA", // Optional, defaults to RSA , will be used if createKeyIfNotExistst set to true
  keyCurve: "P_256", // Optional, will be used if createKeyIfNotExistst set to true
  keySize: 1024 //Optional, will be used if createKeyIfNotExistst set to true
}
```

# How to use

The `keyVaultUnwrapKey` binding can be used to unwrap keys using Azure KeyVault.

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
    [KeyVaultUnwrapKey("myKeyVaultUrl", "MyKeyToUnwrapKeyWith")]string unwrappedKey,
    ILogger log)
{
    log.LogInformation(unwrappedKey);
    return (ActionResult)new OkObjectResult($"UnwrappedKey: {unwrappedKey}");
}
```

#### Inbound byte array example

```c#
[FunctionName("MyKeyVaultFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [KeyVaultUnwrapKey("myKeyVaultUrl", "MyKey")]byte[] unwrappedKey,
    ILogger log)
{
    var unwrappedKeyString = System.Text.Encoding.UTF8.GetString(unwrappedKey);
    log.LogInformation(unwrappedKeyString);
    return (ActionResult)new OkObjectResult($"UnwrappedKey: {unwrappedKeyString}");
}
```

### JavaScript

When using JavaScript, the binding supports binary and string

#### Inbound javascript example

```javascript
module.exports = async function(context, req) {
  context.log(context.bindings.unwrappedKey);
  return {
    body: "UnwrappedKey: " + context.bindings.unwrappedKey
  };
};
```
