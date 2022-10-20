namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IFileViewer
{
    Task ViewAsync(FsArtifact artrifact, IFileService fileService, string returnUrl);
    bool IsExtenstionSupported(FsArtifact fsArtifact);
}
