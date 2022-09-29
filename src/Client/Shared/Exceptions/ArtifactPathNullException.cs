using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
public class ArtifactPathNullException : DomainLogicException
{
    public FsArtifact? FsArtifact { get; set; }

    public ArtifactPathNullException(string message) : base(message)
    {
    }

    public ArtifactPathNullException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public ArtifactPathNullException(LocalizedString message) : base(message)
    {
    }

    public ArtifactPathNullException(LocalizedString message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ArtifactPathNullException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ArtifactPathNullException(FsArtifact fsArtifact, string message) : this(message)
    {
        FsArtifact = fsArtifact;
    }

    public ArtifactPathNullException(FsArtifact fsArtifact, string message, Exception? innerException) : this(message, innerException)
    {
        FsArtifact = fsArtifact;
    }

    public ArtifactPathNullException(FsArtifact fsArtifact, LocalizedString message) : this(message)
    {
        FsArtifact = fsArtifact;
    }

    public ArtifactPathNullException(FsArtifact fsArtifact, LocalizedString message, Exception? innerException) : this(message, innerException)
    {
        FsArtifact = fsArtifact;
    }

    protected ArtifactPathNullException(FsArtifact fsArtifact, SerializationInfo info, StreamingContext context) : this(info, context)
    {
        FsArtifact = fsArtifact;
    }
}
