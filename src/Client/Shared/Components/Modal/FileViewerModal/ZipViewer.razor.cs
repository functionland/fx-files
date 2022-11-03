namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ZipViewer : IFileViewerComponent
{
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }
    [Parameter] public EventCallback OnBack { get; set; }
    [Parameter] public EventCallback OnToggle { get; set; }
    [Parameter] public EventCallback<FsArtifact> OnSelectArtifact { get; set; }

    private List<FsArtifact>? _displayedArtifacts = new();
    private List<FsArtifact>? _selectedArtifacts = new();
    private ArtifactExplorerMode ArtifactExplorerMode { get; set; } = ArtifactExplorerMode.Normal;

    protected async override Task OnInitAsync()
    {
        await LoadAllArtifactsAsync();
        await base.OnInitAsync();
    }

    // Get the list of artifacts to display in the explorer
    private async Task LoadAllArtifactsAsync()
    {
        if (CurrentArtifact is null)
        {
            return;
        }


        // 10 sample artifacts to display
        var fakeArtifacts = new List<FsArtifact>();
        for (int i = 0; i < 50; i++)
        {
            fakeArtifacts.Add(new FsArtifact("C://", "sample 1", FsArtifactType.File, FsFileProviderType.InternalMemory));
        }

        _displayedArtifacts = fakeArtifacts;
    }

    // Extract the artifact to the current directory
    private async Task ExtractArtifactsAsync()
    {

    }
}