using Functionland.FxFiles.Client.Shared.Services.Contracts.FileViewer;

namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ImageViewer : IFileViewerComponent
{
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }
    [Parameter] public EventCallback OnBack { get; set; }
    [Parameter] public EventCallback<List<FsArtifact>> HandlePinArtifact { get; set; }
    [Parameter] public EventCallback<List<FsArtifact>> HandleUnpinArtifact { get; set; }
    [Parameter] public EventCallback<FsArtifact> HandleArtifactOption { get; set; }

    private PathProtocol Protocol =>
            FileService switch
            {
                ILocalDeviceFileService => PathProtocol.Storage,
                IFulaFileService => PathProtocol.Fula,
                _ => throw new InvalidOperationException($"Unsupported file service: {FileService}")
            };

    private async Task PinArtifact()
    {
        var pinArtifact = new List<FsArtifact> { CurrentArtifact };
        await HandlePinArtifact.InvokeAsync(pinArtifact);
    }

    private async Task UnpinArtifact()
    {
        var unPinArtifact = new List<FsArtifact> { CurrentArtifact };
        await HandleUnpinArtifact.InvokeAsync(unPinArtifact);
    }

    private async Task ArtifactOption()
    {
        await HandleArtifactOption.InvokeAsync(CurrentArtifact);
    }
}
