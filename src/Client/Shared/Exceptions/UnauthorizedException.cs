using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
public class UnauthorizedException : KnownException
{
    public UnauthorizedException()
        : base(nameof(AppStrings.UnauthorizedException))
    {
    }

    public UnauthorizedException(string message)
        : base(message)
    {
    }

    public UnauthorizedException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public UnauthorizedException(LocalizedString message)
        : base(message)
    {
    }

    public UnauthorizedException(LocalizedString message, Exception? innerException)
        : base(message, innerException)
    {
    }

    protected UnauthorizedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
