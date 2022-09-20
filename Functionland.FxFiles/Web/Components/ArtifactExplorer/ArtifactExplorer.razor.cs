namespace Functionland.FxFiles.App.Components;

public partial class ArtifactExplorer
{
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }

    [Parameter] public IEnumerable<FsArtifact> Artifacts { get; set; } = default!;

    [Parameter] public EventCallback<FsArtifact> OnSelectArtifact { get; set; } = default!;

    protected override Task OnInitAsync()
    {
        return base.OnInitAsync();
    }

    private async Task HandleArtifactClick(FsArtifact artifact)
    {
        await OnSelectArtifact.InvokeAsync(artifact);
    }

    private bool IsInRoot(FsArtifact? artifact)
    {
        return artifact is null ? true : false;
    }
}
