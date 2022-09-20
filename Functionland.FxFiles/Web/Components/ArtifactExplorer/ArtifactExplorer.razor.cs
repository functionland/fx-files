using Functionland.FxFiles.App.Components.Common;

namespace Functionland.FxFiles.App.Components;

public partial class ArtifactExplorer
{
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }
    [Parameter] public IEnumerable<FsArtifact> Artifacts { get; set; } = default!;
    [Parameter] public EventCallback<FsArtifact> OnSelectArtifact { get; set; } = default!;

    public List<FsArtifact> SelectedArtifacts { get; set; } = new List<FsArtifact>();
    public ViewModeEnum ViewMode = ViewModeEnum.list;
    public SortOrderEnum SortOrder = SortOrderEnum.asc;
    public bool IsSelected;
    public bool IsSelectionMode;
    public bool IsSelectedAll = false;
    public DateTimeOffset PointerDownTime;

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

    public void ToggleSortOrder()
    {
        if (SortOrder == SortOrderEnum.asc)
        {
            SortOrder = SortOrderEnum.desc;
        }
        else
        {
            SortOrder = SortOrderEnum.asc;
        }

        //todo: change order of list items
    }

    public void OnSortChange()
    {
        //todo: Open sort bottom sheet
    }

    public void ToggleSelectedAll()
    {
        IsSelectedAll = !IsSelectedAll;
        //todo: select all items
    }

    public void ChangeViewMode(ViewModeEnum mode)
    {
        ViewMode = mode;
    }

    public void OpenArtifactOverFlow()
    {
        //todo: open folder of file overflow bottom sheet
    }

    public void CancelSelection()
    {
        IsSelectionMode = false;
        SelectedArtifacts = new List<FsArtifact>();
    }

    public void PointerDown()
    {
        PointerDownTime = DateTimeOffset.UtcNow;
    }

    public void PointerUp()
    {
        if (!IsSelectionMode)
        {
            var downTime = (DateTimeOffset.UtcNow.Ticks - PointerDownTime.Ticks) / TimeSpan.TicksPerMillisecond;
            IsSelectionMode = downTime > 400;
        }
    }

    public void OnSelectionChanged(FsArtifact selectedArtifact)
    {
        if (SelectedArtifacts.Any(item => item.FullPath == selectedArtifact.FullPath))
        {
            SelectedArtifacts.Remove(selectedArtifact);
        }
        else
        {
            SelectedArtifacts.Add(selectedArtifact);
        }
    }

    public string GetArtifactIcon(FsArtifact artifact)
    {
        //todo: Proper icon for artifact
        return "text-file-icon";
    }

    public string GetArtifactSubText(FsArtifact artifact)
    {
        //todo: Proper subtext for artifact
        return "Modified 09/30/22";
    }
}
