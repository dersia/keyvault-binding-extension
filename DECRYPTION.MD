# Decryption

## `KeyVaultDecrypt`

```json5
{
  type: "keyVaultDecrypt",
  name: "decryptedValue",
  direction: "in", //only in supported
  keyVaultUrl: "myKeyVaultUrl", //AppSettings name
  keyName: "MyKey", //Key name, AutoResolve-able
  keyVersion: "", //KeyVault key-version, nullable will be resolved to latest
  algorithm: "RSA1_5", // Values: RSA_OAEP, RSA1_5 - defaults to RSA1_5
  value: "{Query.myQueryData}", // Value to be decrypted, can be static string, or AutoResolved with binding expressions
  createKeyIfNotExistst: false, //Optional, defaults to false, creates a key, if key does not exists in KeyVault
  keyType: "RSA", // Optional, defaults to RSA , will be used if createKeyIfNotExistst set to true
  keyCurve: "P_256", // Optional, will be used if createKeyIfNotExistst set to true
  keySize: 1024 //Optional, will be used if createKeyIfNotExistst set to true
}
```

# How to use

The `keyVaultDecrypt` binding can be used to decrypt data using Azure KeyVault.

> ## **Note**:
>
> The ENCRYPT/DECRYPT operation only supports a single block of data, the size of which is dependent on the target key and the encryption algorithm to be used.
>
> For encryption/decryption of Azure Storage Blobs see the [KeyVaultBlobsBinding](./KEYVAULTBLOBS.MD)
>
> Encryption/Decryption of larger data blocks will be supported in a future version of this binding.

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
    [KeyVaultDecrypt("myKeyVaultUrl", "MyKey")]string decrypterdValue,
    ILogger log)
{
    log.LogInformation(decrypterdValue);
    return (ActionResult)new OkObjectResult($"DecryptedValue: {decrypterdValue}");
}
```

#### Inbound byte array example

```c#
[FunctionName("MyKeyVaultFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [KeyVaultDecrypt("myKeyVaultUrl", "MyKey")]byte[] decrypterdValue,
    ILogger log)
{
    var decryptedString = System.Text.Encoding.UTF8.GetString(decrypterdValue);
    log.LogInformation(decryptedString);
    return (ActionResult)new OkObjectResult($"DecryptedValue: {decryptedString}");
}
```

### JavaScript

When using JavaScript, the binding supports binary and string

#### Inbound javascript example

```javascript
module.exports = async function(context, req) {
  context.log(context.bindings.decrypterdValue);
  return {
    body: "DecryptedValue: " + context.bindings.decrypterdValue
  };
};
```
