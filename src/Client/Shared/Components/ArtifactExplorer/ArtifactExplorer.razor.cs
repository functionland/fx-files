using Functionland.FxFiles.Client.Shared.Components.Common;

namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class ArtifactExplorer
    {
        [Parameter] public FsArtifact? CurrentArtifact { get; set; }
        [Parameter] public IEnumerable<FsArtifact>? Artifacts { get; set; }
        [Parameter] public SortTypeEnum CurrentSortType { get; set; } = SortTypeEnum.Name;
        [Parameter] public EventCallback<FsArtifact> OnArtifactsOptionsClick { get; set; } = new();
        [Parameter] public EventCallback<FsArtifact> OnSelectArtifact { get; set; } = new();
        [Parameter] public ArtifactExplorerMode ArtifactExplorerMode { get; set; }
        [Parameter] public EventCallback<ArtifactExplorerMode> ArtifactExplorerModeChanged { get; set; }
        [Parameter] public EventCallback OnAddFolderButtonClick { get; set; }
        [Parameter] public bool IsSelected { get; set; }
        [Parameter] public EventCallback<bool> IsSelectedChanged { get; set; }
        [Parameter] public FsArtifact[] SelectedArtifacts { get; set; } = Array.Empty<FsArtifact>();
        [Parameter] public EventCallback<FsArtifact[]> SelectedArtifactsChanged { get; set; }
        [Parameter] public ViewModeEnum ViewMode { get; set; } = ViewModeEnum.list;
        [Parameter] public FileCategoryType? FileCategoryFilter { get; set; }

        public DateTimeOffset PointerDownTime;

        protected override Task OnInitAsync()
        {
            return base.OnInitAsync();
        }

        private async Task HandleArtifactOptionsClick(FsArtifact artifact)
        {
            await OnArtifactsOptionsClick.InvokeAsync(artifact);
        }

        protected override Task OnParamsSetAsync()
        {
            if (Artifacts is null)
            {
                Artifacts = Array.Empty<FsArtifact>();
            }
            return base.OnParamsSetAsync();
        }

        private async Task HandleArtifactClick(FsArtifact artifact)
        {
            await OnSelectArtifact.InvokeAsync(artifact);
        }

        private bool IsInRoot(FsArtifact? artifact)
        {
            return artifact is null ? true : false;
        }

        public void PointerDown()
        {
            PointerDownTime = DateTimeOffset.UtcNow;
        }

        public async Task PointerUp(FsArtifact artifact)
        {
            if (ArtifactExplorerMode == ArtifactExplorerMode.Normal)
            {
                var downTime = (DateTimeOffset.UtcNow.Ticks - PointerDownTime.Ticks) / TimeSpan.TicksPerMillisecond;
                if (downTime > 400)
                {
                    IsSelected = false;
                    SelectedArtifacts = Array.Empty<FsArtifact>();
                    ArtifactExplorerMode = ArtifactExplorerMode.SelectArtifact;
                }
                else
                {
                    await OnSelectArtifact.InvokeAsync(artifact);
                }
            }
            else if (ArtifactExplorerMode == ArtifactExplorerMode.SelectDestionation)
            {
                await OnSelectArtifact.InvokeAsync(artifact);
            }
            await SelectedArtifactsChanged.InvokeAsync(SelectedArtifacts);
            await ArtifactExplorerModeChanged.InvokeAsync(ArtifactExplorerMode);
        }

        public async Task OnSelectionChanged(FsArtifact selectedArtifact)
        {
            if (SelectedArtifacts.Any(item => item.FullPath == selectedArtifact.FullPath))
            {
                IsSelected = false;
                SelectedArtifacts = SelectedArtifacts.Where(s => s.FullPath != selectedArtifact.FullPath).ToArray();
            }
            else
            {
                IsSelected = true;
                SelectedArtifacts = SelectedArtifacts.Append(selectedArtifact).ToArray();
            }
            await SelectedArtifactsChanged.InvokeAsync(SelectedArtifacts);
            await IsSelectedChanged.InvokeAsync(IsSelected);
        }

        public void OnCreateFolder()
        {
            OnAddFolderButtonClick.InvokeAsync();
        }

        public string GetArtifactIcon(FsArtifact artifact)
        {
            if (artifact.ArtifactType == FsArtifactType.File)
            {
                switch (artifact.FileCategory)
                {
                    case FileCategoryType.Document:
                        return "text-file-icon";
                    case FileCategoryType.Other:
                        return "text-file-icon";
                    case FileCategoryType.Pdf:
                        return "text-file-icon";
                    case FileCategoryType.Image:
                        return "photo-file-icon";
                    case FileCategoryType.Audio:
                        return "audio-file-icon";
                    case FileCategoryType.Video:
                        return "video-file-icon";
                }
            }

            return "folder-icon";
        }

        public string GetArtifactSubText(FsArtifact artifact)
        {
            //todo: Proper subtext for artifact
            return "Modified 09/30/22";
        }
    }
}