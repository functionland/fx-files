using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
public class ArtifactAlreadyPinnedException : DomainLogicException
{
    public ArtifactAlreadyPinnedException(string message) : base(message)
    {
    }

    public ArtifactAlreadyPinnedException(LocalizedString message) : base(message)
    {
    }

    public ArtifactAlreadyPinnedException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public ArtifactAlreadyPinnedException(LocalizedString message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ArtifactAlreadyPinnedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
