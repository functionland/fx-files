namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IFileViewer
{
    Task ViewAsync(FsArtifact artrifact, IFileService fileService);
    bool IsExtenstionSupported(FsArtifact fsArtifact);
}
