namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IFileViewer
{
    Task ViewAsync(string artrifactPath, IFileService fileService, string returnUrl);
    Task<bool> IsSupportedAsync(string artrifactPath, IFileService fileService, CancellationToken? cancellationToken = null);
}
