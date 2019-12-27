# Secure EventSourcing Example

My dear friend [Andreas](https://twitter.com/_andreasgrimm) asked me in [a twitter thread](https://twitter.com/_andreasgrimm/status/1210507885509332993) how to do proper Secure EventSourcing using Azure Functions, this binding and ServiceBus or EventGrid as a sink.

In this example we will go through a scenario where a customer signs up on our website and this results in a `CustomerRegistered`-Event and is stored in [EventStore](https://eventstore.org/).

In this case we are using [EventStore](https://eventstore.org/) as our sink, but it is interreplaceable with any sink you want and that is supported as output-binding.

Before we start let's talk about how the user flow is working:

On out Website we have some decent UI that provides the Customer a login Screen. On this screen the Customer can either Login or Signup (Register). The whole login/signup functionality is done by an Auth-Provider like Azure B2C or Auth0.
As soon as the user is signed up the auth-provider is redirecting back to our Function, which in a EventSourcing/CQRS-Scenario is our CommandHanlder for the `RegisterCustomer`-Command. Our CommandHandler will create a specific RSA-Key in KeyVault for the new User and Encrypt the data provided from the auth-provider via claims and store those in [EventStore](https://eventstore.org/) before it redirects back to out fancy website.

Website --> Auth-provider --> 
RegisterCustomer-CommandHandler --> Website

Unfortunatley for this scenario we have to use a dotnet function, we can talk about a javascript version at a later point, but this needs a bit more work.

Our function is going to be a HttpTriggered-Function and will have a [KeyVault-Input-Binding](https://github.com/dersia/keyvault-binding-extension) and a [EventStore-Output-Binding](https://github.com/dersia/eventstoreextension).

Here is the Full Code
```c#
[FunctionName("RegisterCustomer")]
public static async Task<IActionResult> RegisterCustomerAccount(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
    [EventStoreStreams(ConnectionStringSetting = "myEventStoreConnection", StreamName = "customer")] IAsyncCollector<EventData> events,
    [KeyVault(KeyVaultUrl = "myKeyVaultConnection")]IKeyVaultUtil keyVaultUtil,
    Microsoft.Extensions.Logging.ILogger log)
{
    var (unpackedState, token, nonce, oid) = req.Query.UnpackQuery(); //Unpack id_token, oid is the userid from the auth-provider     
    if(oid is null || string.IsNullOrWhiteSpace(oid) || unpackedState is null || token is null || nonce is null)
    {
        return new BadRequestObjectResult($"token = {token.ToSafeString()},\r\nnonce = {nonce.ToSafeString()},\r\noid = {oid.ToSafeString()}");
    }

    var key = await keyVaultUtil.GetOrCreateKey(oid); //Get or Creates a RSA-Key in KeyVault for the oid
    var customerRegistered = await token.ToCustomerRegisteredEvent(keyVaultUtil, key); // unpacks claims and encrypts PII and creates the Secure Event
    if (customerRegistered is { } cr)
    {
        await events.AddAsync(cr.ToEventData());
        if (unpackedState.GetRedirectUri() is { } uri)
        {
            return (ActionResult)new RedirectResult(uri.ToString());
        }
    }
    return (ActionResult)new OkObjectResult($"code => {req.Query["code"]}\r\n\r\nState => Redirect: {unpackedState?.Redirect}\r\nid_token={req.Query["idToken"]}");
}
```
> |Supporting files|
> |--------|
> |[CustomerRegisteredEvent.cs](./SESExample/CustomerRegisteredEvent.cs)|
> |[Extensions.cs](./SESExample/Extensions.cs)|
> |[State.cs](./SESExample/State.cs)|

The OAuth parameter are set to:
|Name|Value|
|----|-----|
|response_type|id_token|
|response_mode|query|
|scope|openid offline_access|

When the Auth-Provider redirects back to our CommandHandler-Function, it provides all information in as query-paramters because we asked for it (`response_mode=query`).

After unpacking the query parameter, we use the binding to Get or Create a RSA-Key using the user_id provided by the Auth-Provider.

We then use that Key to Encrypt the Event-Values!!!
> ## Note
> We Only encrypt values, not keys!
> This is important, because this let's us aggregate and project over events even if the values are still encrypted. There for we only need to decrypt, once our porjection is completed.

After our event is created we store our Event in our sink, in this case [EventStore](https://eventstore.org/).

In the state-object we can find the Redirect-URI to redirect back to our fancy Website.

I hope this gives you a clue about how to do Secure EventSourcing using Azure Functions and this Extension.

If there are question, please just open an issue.

