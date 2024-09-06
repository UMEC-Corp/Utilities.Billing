using System.Runtime.Serialization;

namespace Utilities.Billing.StellarWallets;

[Serializable]
internal class StellarTransactionFailException : Exception
{

    public StellarTransactionFailException()
    {
    }

    public StellarTransactionFailException(string? message) : base(message)
    {
    }

    public StellarTransactionFailException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected StellarTransactionFailException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
