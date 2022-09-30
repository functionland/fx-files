using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

public class StreamNullException : DomainLogicException
{
    public StreamNullException(string message) : base(message)
    {
    }

    public StreamNullException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public StreamNullException(LocalizedString message) : base(message)
    {
    }

    public StreamNullException(LocalizedString message, Exception? innerException) : base(message, innerException)
    {
    }

    protected StreamNullException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
