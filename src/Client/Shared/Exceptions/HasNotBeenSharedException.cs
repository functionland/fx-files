using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
public class HasNotBeenSharedException : DomainLogicException
{
    public HasNotBeenSharedException(string message) : base(message)
    {
    }

    public HasNotBeenSharedException(LocalizedString message) : base(message)
    {
    }

    public HasNotBeenSharedException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public HasNotBeenSharedException(LocalizedString message, Exception? innerException) : base(message, innerException)
    {
    }

    protected HasNotBeenSharedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
