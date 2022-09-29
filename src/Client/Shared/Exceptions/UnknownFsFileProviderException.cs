using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
public class UnknownFsFileProviderException : KnownException
{
    public UnknownFsFileProviderException(string message)
        : base(message)
    {
    }

    public UnknownFsFileProviderException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public UnknownFsFileProviderException(LocalizedString message)
        : base(message)
    {
    }

    public UnknownFsFileProviderException(LocalizedString message, Exception? innerException)
        : base(message, innerException)
    {
    }

    protected UnknownFsFileProviderException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
