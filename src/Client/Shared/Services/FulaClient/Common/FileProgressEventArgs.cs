namespace Functionland.FxFiles.Client.Shared.Services.FulaClient.Common;

public class FileProgressEventArgs : EventArgs
{
    public FileProgressEventArgs(long progress, FsArtifactProgressType fsArtifactProgressType, FsArtifact fsArtifact)
    {
        Progress = progress;
        FsArtifactProgressType = fsArtifactProgressType;
        FsArtifact = fsArtifact;
    }

    public long Progress { get; set; }
    public FsArtifactProgressType FsArtifactProgressType { get; set; }
    public FsArtifact FsArtifact { get; set; }
}
