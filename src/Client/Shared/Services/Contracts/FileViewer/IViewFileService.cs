namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IViewFileService<out TFileService>
    where TFileService : IFileService
{
    Task ViewFile(FsArtifact artifact, string returnUrl);
}
