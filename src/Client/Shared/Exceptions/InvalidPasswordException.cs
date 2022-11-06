using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

public class InvalidPasswordException : DomainLogicException
{
    public InvalidPasswordException(string message) : base(message)
    {
    }

    public InvalidPasswordException(LocalizedString message) : base(message)
    {
    }

    public InvalidPasswordException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public InvalidPasswordException(LocalizedString message, Exception? innerException) : base(message, innerException)
    {
    }

    protected InvalidPasswordException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}