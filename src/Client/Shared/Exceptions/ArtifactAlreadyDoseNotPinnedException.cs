using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
public class ArtifactAlreadyDoseNotPinnedException : DomainLogicException
{
    public ArtifactAlreadyDoseNotPinnedException(string message) : base(message)
    {
    }

    public ArtifactAlreadyDoseNotPinnedException(LocalizedString message) : base(message)
    {
    }

    public ArtifactAlreadyDoseNotPinnedException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public ArtifactAlreadyDoseNotPinnedException(LocalizedString message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ArtifactAlreadyDoseNotPinnedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
