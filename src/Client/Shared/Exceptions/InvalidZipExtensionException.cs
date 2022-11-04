using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
public class InvalidZipExtensionException:KnownException
{
    public InvalidZipExtensionException():this("Invalid zip file extension error")
    {

    }
    public InvalidZipExtensionException(string message)
          : base(message)
    {
    }

    public InvalidZipExtensionException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public InvalidZipExtensionException(LocalizedString message)
        : base(message)
    {
    }

    public InvalidZipExtensionException(LocalizedString message, Exception? innerException)
        : base(message, innerException)
    {
    }

    protected InvalidZipExtensionException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
