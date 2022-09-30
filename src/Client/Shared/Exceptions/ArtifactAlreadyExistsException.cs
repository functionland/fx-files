using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
public class ArtifactAlreadyExistsException : DomainLogicException
{
    public FsArtifact? FsArtifact { get; set; }

    public ArtifactAlreadyExistsException(string message) : base(message)
    {
    }

    public ArtifactAlreadyExistsException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public ArtifactAlreadyExistsException(LocalizedString message) : base(message)
    {
    }

    public ArtifactAlreadyExistsException(LocalizedString message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ArtifactAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ArtifactAlreadyExistsException(FsArtifact fsArtifact, string message) : this(message)
    {
        FsArtifact = fsArtifact;
    }

    public ArtifactAlreadyExistsException(FsArtifact fsArtifact, string message, Exception? innerException) : this(message, innerException)
    {
        FsArtifact = fsArtifact;
    }

    public ArtifactAlreadyExistsException(FsArtifact fsArtifact, LocalizedString message) : this(message)
    {
        FsArtifact = fsArtifact;
    }

    public ArtifactAlreadyExistsException(FsArtifact fsArtifact, LocalizedString message, Exception? innerException) : this(message, innerException)
    {
        FsArtifact = fsArtifact;
    }

    protected ArtifactAlreadyExistsException(FsArtifact fsArtifact, SerializationInfo info, StreamingContext context) : this(info, context)
    {
        FsArtifact = fsArtifact;
    }
}
