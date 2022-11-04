using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
public class UnableAccessToStorageException : KnownException
{
    public UnableAccessToStorageException(string message)
        : base(message)
    {
    }

    public UnableAccessToStorageException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public UnableAccessToStorageException(LocalizedString message)
        : base(message)
    {
    }

    public UnableAccessToStorageException(LocalizedString message, Exception? innerException)
        : base(message, innerException)
    {
    }

    protected UnableAccessToStorageException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
