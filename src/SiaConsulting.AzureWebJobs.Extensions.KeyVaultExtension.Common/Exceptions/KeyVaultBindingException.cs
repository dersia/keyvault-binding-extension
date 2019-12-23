using System;
using System.Runtime.Serialization;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Common.Exceptions
{
    public class KeyVaultBindingException : Exception
    {
        public KeyVaultBindingException()
        {
        }

        public KeyVaultBindingException(string message) : base(message)
        {
        }

        public KeyVaultBindingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected KeyVaultBindingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
