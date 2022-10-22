namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IViewFileService<out TFileService>
    where TFileService : IFileService
{
    Task ViewFileAsync(string filePath, string returnUrl, CancellationToken? cancellationToken = null);
}
