using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
public class BloxPoolAlreadyExistsException : DomainLogicException
{
    public BloxPoolAlreadyExistsException(string message) : base(message)
    {
    }

    public BloxPoolAlreadyExistsException(LocalizedString message) : base(message)
    {
    }

    public BloxPoolAlreadyExistsException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public BloxPoolAlreadyExistsException(LocalizedString message, Exception? innerException) : base(message, innerException)
    {
    }

    protected BloxPoolAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}