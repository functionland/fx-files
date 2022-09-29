using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
public class ArtifactDoseNotExistsException : DomainLogicException
{
    public FsArtifact? FsArtifact { get; set; }

    public ArtifactDoseNotExistsException(string message) : base(message)
    {
    }

    public ArtifactDoseNotExistsException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public ArtifactDoseNotExistsException(LocalizedString message) : base(message)
    {
    }

    public ArtifactDoseNotExistsException(LocalizedString message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ArtifactDoseNotExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ArtifactDoseNotExistsException(FsArtifact fsArtifact, string message) : this(message)
    {
        FsArtifact = fsArtifact;
    }

    public ArtifactDoseNotExistsException(FsArtifact fsArtifact, string message, Exception? innerException) : this(message, innerException)
    {
        FsArtifact = fsArtifact;
    }

    public ArtifactDoseNotExistsException(FsArtifact fsArtifact, LocalizedString message) : this(message)
    {
        FsArtifact = fsArtifact;
    }

    public ArtifactDoseNotExistsException(FsArtifact fsArtifact, LocalizedString message, Exception? innerException) : this(message, innerException)
    {
        FsArtifact = fsArtifact;
    }

    protected ArtifactDoseNotExistsException(FsArtifact fsArtifact, SerializationInfo info, StreamingContext context) : this(info, context)
    {
        FsArtifact = fsArtifact;
    }
}
