using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
public class CanNotModifyOrDeleteDriveException : DomainLogicException
{
    public FsArtifact? FsArtifact { get; set; }

    public CanNotModifyOrDeleteDriveException(string message) : base(message)
    {
    }

    public CanNotModifyOrDeleteDriveException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public CanNotModifyOrDeleteDriveException(LocalizedString message) : base(message)
    {
    }

    public CanNotModifyOrDeleteDriveException(LocalizedString message, Exception? innerException) : base(message, innerException)
    {
    }

    protected CanNotModifyOrDeleteDriveException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public CanNotModifyOrDeleteDriveException(FsArtifact fsArtifact, string message) : this(message)
    {
        FsArtifact = fsArtifact;
    }

    public CanNotModifyOrDeleteDriveException(FsArtifact fsArtifact, string message, Exception? innerException) : this(message, innerException)
    {
        FsArtifact = fsArtifact;
    }

    public CanNotModifyOrDeleteDriveException(FsArtifact fsArtifact, LocalizedString message) : this(message)
    {
        FsArtifact = fsArtifact;
    }

    public CanNotModifyOrDeleteDriveException(FsArtifact fsArtifact, LocalizedString message, Exception? innerException) : this(message, innerException)
    {
        FsArtifact = fsArtifact;
    }

    protected CanNotModifyOrDeleteDriveException(FsArtifact fsArtifact, SerializationInfo info, StreamingContext context) : this(info, context)
    {
        FsArtifact = fsArtifact;
    }
}
