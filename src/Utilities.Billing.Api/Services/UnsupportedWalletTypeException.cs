using System.Runtime.Serialization;

namespace Utilities.Billing.Api.Services
{
    [Serializable]
    internal class UnsupportedWalletTypeException : Exception
    {
        public UnsupportedWalletTypeException()
        {
        }

        public UnsupportedWalletTypeException(string? message) : base(message)
        {
        }

        public UnsupportedWalletTypeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnsupportedWalletTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}