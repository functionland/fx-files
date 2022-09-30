using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
public class ArtifactInvalidNameException : DomainLogicException
{
    public FsArtifact? FsArtifact { get; set; }

    public ArtifactInvalidNameException(string message) : base(message)
    {
    }

    public ArtifactInvalidNameException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public ArtifactInvalidNameException(LocalizedString message) : base(message)
    {
    }

    public ArtifactInvalidNameException(LocalizedString message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ArtifactInvalidNameException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ArtifactInvalidNameException(FsArtifact fsArtifact, string message) : this(message)
    {
        FsArtifact = fsArtifact;
    }

    public ArtifactInvalidNameException(FsArtifact fsArtifact, string message, Exception? innerException) : this(message, innerException)
    {
        FsArtifact = fsArtifact;
    }

    public ArtifactInvalidNameException(FsArtifact fsArtifact, LocalizedString message) : this(message)
    {
        FsArtifact = fsArtifact;
    }

    public ArtifactInvalidNameException(FsArtifact fsArtifact, LocalizedString message, Exception? innerException) : this(message, innerException)
    {
        FsArtifact = fsArtifact;
    }

    protected ArtifactInvalidNameException(FsArtifact fsArtifact, SerializationInfo info, StreamingContext context) : this(info, context)
    {
        FsArtifact = fsArtifact;
    }
}
