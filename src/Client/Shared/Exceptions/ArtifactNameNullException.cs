using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
public class ArtifactNameNullException : DomainLogicException
{
    public FsArtifact? FsArtifact { get; set; }

    public ArtifactNameNullException(string message) : base(message)
    {
    }

    public ArtifactNameNullException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public ArtifactNameNullException(LocalizedString message) : base(message)
    {
    }

    public ArtifactNameNullException(LocalizedString message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ArtifactNameNullException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ArtifactNameNullException(FsArtifact fsArtifact, string message) : this(message)
    {
        FsArtifact = fsArtifact;
    }

    public ArtifactNameNullException(FsArtifact fsArtifact, string message, Exception? innerException) : this(message, innerException)
    {
        FsArtifact = fsArtifact;
    }

    public ArtifactNameNullException(FsArtifact fsArtifact, LocalizedString message) : this(message)
    {
        FsArtifact = fsArtifact;
    }

    public ArtifactNameNullException(FsArtifact fsArtifact, LocalizedString message, Exception? innerException) : this(message, innerException)
    {
        FsArtifact = fsArtifact;
    }

    protected ArtifactNameNullException(FsArtifact fsArtifact, SerializationInfo info, StreamingContext context) : this(info, context)
    {
        FsArtifact = fsArtifact;
    }
}
