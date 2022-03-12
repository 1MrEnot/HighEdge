namespace Straonit.HighEdge.Core.SplitSecret;

using System.Runtime.Serialization;

public class NotEnoughSecretPartsException : Exception
{
    public NotEnoughSecretPartsException()
    {
    }

    protected NotEnoughSecretPartsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    protected NotEnoughSecretPartsException(int minumimPartAmount, int partAmount)
        : base($"Expected at least {minumimPartAmount} parts for secret merge, but received {partAmount} only")
    {
    }


    public NotEnoughSecretPartsException(string? message) : base(message)
    {
    }

    public NotEnoughSecretPartsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}