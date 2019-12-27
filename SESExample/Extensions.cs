using EventStore.ClientAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.KeyVault.Models;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Client;
using SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SESExample 
{
    public static class SESExtensions
    {
        public static string? GetValueSafe(this JwtSecurityToken? token, string property)
            => token is { } t && t.Payload is { } p && p.ContainsKey(property) ? p[property].ToString() : null;

        public static async Task<string?> EncryptToBase64String(this IKeyVaultUtil keyVaultUtil, KeyBundle? key, string? data)
            => key is null || data is null ? null : Convert.ToBase64String(await keyVaultUtil.Encrypt(key.KeyIdentifier.Name, key.KeyIdentifier.Version, EncryptionAlgorithm.RSA1_5, Encoding.UTF8.GetBytes(data)));

        public static EventData ToEventData(this EventBase @event)
            => new EventData(Guid.NewGuid(), @event?.GetType().Name, true, Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(@event, @event?.GetType())), null);

        public static Uri? GetRedirectUri(this AuthState? state)
            => !string.IsNullOrWhiteSpace(state?.Redirect) && Uri.TryCreate(state?.Redirect, UriKind.Absolute, out var uri) ? uri : null;

        public static string ToSafeString(this JwtSecurityToken? token)
            => token?.ToString() ?? "null";

        public static string ToSafeString(this Guid? guid)
            => guid?.ToString() ?? "null";

        public static string ToSafeString(this string? str)
            => str ?? "null";

        public static string GetOid(this HttpRequest req)
        {
            var claim = req.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            return claim.Value;
        }
        
        public static async Task<CustomerRegistered?> ToCustomerRegisteredEvent(this JwtSecurityToken? token, IKeyVaultUtil keyVaultUtil, KeyBundle key)
        {
            if (token is null || token.Payload is null || !token.Payload.ContainsKey("oid")) return null;
            var id = token?.GetValueSafe("oid");
            var givenname = token?.GetValueSafe("given_name");
            var surname = token?.GetValueSafe("family_name");
            var city = token?.GetValueSafe("city");
            var country = token?.GetValueSafe("country");
            var name = token?.GetValueSafe("name");
            var jobTitle = token?.GetValueSafe("jobTitle");
            var postalCode = token?.GetValueSafe("postalCode");
            var state = token?.GetValueSafe("state");
            var streetAddress = token?.GetValueSafe("streetAddress");
            var emails = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<string>?>(token?.GetValueSafe("emails"));
            IList<string>? encryptedEmails = null;
            if (emails is { })
            {
                encryptedEmails = new List<string>();
                foreach (var email in emails)
                {
                    var encryptedMail = await keyVaultUtil.EncryptToBase64String(key, email);
                    if (encryptedMail is null) continue;
                    encryptedEmails.Add(encryptedMail);
                }
                if (encryptedEmails.Count <= 0)
                    encryptedEmails = null;
            }

            return new CustomerRegistered(await keyVaultUtil.EncryptToBase64String(key, id))
            {
                Givenname = await keyVaultUtil.EncryptToBase64String(key, givenname),
                Surname = await keyVaultUtil.EncryptToBase64String(key, surname),
                City = await keyVaultUtil.EncryptToBase64String(key, city),
                Country = await keyVaultUtil.EncryptToBase64String(key, country),
                Name = await keyVaultUtil.EncryptToBase64String(key, name),
                JobTitle = await keyVaultUtil.EncryptToBase64String(key, jobTitle),
                PostalCode = await keyVaultUtil.EncryptToBase64String(key, postalCode),
                State = await keyVaultUtil.EncryptToBase64String(key, state),
                StreetAddress = await keyVaultUtil.EncryptToBase64String(key, streetAddress),
                EmailAddresses = encryptedEmails
            };
        }

        public static (State? state, JwtSecurityToken? token, Guid? nonce, string? oid) UnpackQuery(this IQueryCollection query)
        {
            AuthState? unpackedState = null;
            if (query.ContainsKey("state"))
            {
                unpackedState = System.Text.Json.JsonSerializer.Deserialize<State>(Convert.FromBase64String(query["state"]));
            }

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken? token = null;
            Guid? nonce = null;
            string? oid = null;
            if (query.ContainsKey("id_token"))
            {
                var idToken = query["id_token"];
                if (jwtTokenHandler.CanReadToken(idToken))
                {
                    token = jwtTokenHandler.ReadJwtToken(idToken);
                    Guid.TryParse(token?.Payload?["nonce"]?.ToString(), out var parsedNonce);
                    if (parsedNonce != Guid.Empty) nonce = parsedNonce;
                    oid = token?.Payload?["oid"]?.ToString();
                }
            }
            return (unpackedState, token, nonce, oid);
        }
    } 

}