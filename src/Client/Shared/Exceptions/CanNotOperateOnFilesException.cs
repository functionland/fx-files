namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
public class CanNotOperateOnFilesException : DomainLogicException
{
    public List<FsArtifact> FsArtifacts { get; set; }

    public CanNotOperateOnFilesException(string message, List<FsArtifact> fsArtifacts) : base(message)
    {
        FsArtifacts = fsArtifacts;
    }
}
