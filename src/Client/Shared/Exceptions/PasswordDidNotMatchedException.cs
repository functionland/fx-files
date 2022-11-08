using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
public class PasswordDidNotMatchedException : DomainLogicException
{
    public PasswordDidNotMatchedException(string message) : base(message)
    {
    }

    public PasswordDidNotMatchedException(LocalizedString message) : base(message)
    {
    }

    public PasswordDidNotMatchedException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public PasswordDidNotMatchedException(LocalizedString message, Exception? innerException) : base(message, innerException)
    {
    }

    protected PasswordDidNotMatchedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
