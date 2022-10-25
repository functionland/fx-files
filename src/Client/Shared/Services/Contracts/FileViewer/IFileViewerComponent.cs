namespace Functionland.FxFiles.Client.Shared.Services.Contracts.FileViewer;

public interface IFileViewerComponent
{
    public IFileService FileService { get; set; }
    public FsArtifact? CurrentArtifact { get; set; }
    public bool Visibility { get; set; }

    public bool IsSupported(FsArtifact artifact);
}
