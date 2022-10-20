namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IViewFileService<TFileService>
    where TFileService : IFileService
{
    Task ViewFile(FsArtifact artifact);
}
