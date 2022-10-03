using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
public class ArtifactTypeNullException : DomainLogicException
{
    public FsArtifact? FsArtifact { get; set; }

    public ArtifactTypeNullException(string message) : base(message)
    {
    }

    public ArtifactTypeNullException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public ArtifactTypeNullException(LocalizedString message) : base(message)
    {
    }

    public ArtifactTypeNullException(LocalizedString message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ArtifactTypeNullException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ArtifactTypeNullException(FsArtifact fsArtifact, string message) : this(message)
    {
        FsArtifact = fsArtifact;
    }

    public ArtifactTypeNullException(FsArtifact fsArtifact, string message, Exception? innerException) : this(message, innerException)
    {
        FsArtifact = fsArtifact;
    }

    public ArtifactTypeNullException(FsArtifact fsArtifact, LocalizedString message) : this(message)
    {
        FsArtifact = fsArtifact;
    }

    public ArtifactTypeNullException(FsArtifact fsArtifact, LocalizedString message, Exception? innerException) : this(message, innerException)
    {
        FsArtifact = fsArtifact;
    }

    protected ArtifactTypeNullException(FsArtifact fsArtifact, SerializationInfo info, StreamingContext context) : this(info, context)
    {
        FsArtifact = fsArtifact;
    }
}
