namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IFileViewer
{
    Task ViewAsync(string artrifactPath, IFileService fileService, string returnUrl);
    bool IsExtenstionSupported(string artrifactPath, IFileService fileService);
}
