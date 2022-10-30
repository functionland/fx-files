namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IFileViewerComponent
{
    public IFileService FileService { get; set; }
    public FsArtifact? CurrentArtifact { get; set; }
}
