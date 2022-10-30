using Functionland.FxFiles.Client.Shared.Services.Contracts.FileViewer;

namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ImageViewer : IFileViewerComponent
{
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }
    [Parameter] public EventCallback OnBack { get; set; }
    [Parameter] public EventCallback<FsArtifact> HandlePinArtifact { get; set; }
    [Parameter] public EventCallback<FsArtifact> HandleUnpinArtifact { get; set; }
    [Parameter] public EventCallback<FsArtifact> HandleArtifactOption { get; set; }

    public PathProtocol Protocol =>
            FileService switch
            {
                ILocalDeviceFileService => PathProtocol.Storage,
                IFulaFileService => PathProtocol.Fula,
                _ => throw new InvalidOperationException($"Unsupported file service: {FileService}")
            };
}
