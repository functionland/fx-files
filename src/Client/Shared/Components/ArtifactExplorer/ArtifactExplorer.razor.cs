using Functionland.FxFiles.Client.Shared.Components.Common;

using Microsoft.JSInterop;

namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class ArtifactExplorer
    {
        [Parameter] public FsArtifact? CurrentArtifact { get; set; }
        [Parameter] public IEnumerable<FsArtifact>? Artifacts { get; set; }
        [Parameter] public SortTypeEnum CurrentSortType { get; set; } = SortTypeEnum.Name;
        [Parameter] public bool IsAscOrder { get; set; } = true;
        [Parameter] public EventCallback<FsArtifact> OnArtifactsOptionsClick { get; set; } = new();
        [Parameter] public EventCallback<FsArtifact[]> OnMultiArtifactsOptionsClick { get; set; } = new();
        [Parameter] public EventCallback<FsArtifact> OnSelectArtifact { get; set; } = new();
        [Parameter] public EventCallback OnCancelSelectDestionationMode { get; set; } = new();
        [Parameter] public EventCallback<FsArtifact[]> OnSelectDestination { get; set; } = new();
        [Parameter] public ArtifactExplorerMode ArtifactExplorerMode { get; set; } = ArtifactExplorerMode.Normal;
        [Parameter] public ArtifactActionResult ArtifactActionResult { get; set; } = new();
        [Parameter] public EventCallback OnFilterClick { get; set; }
        [Parameter] public EventCallback OnSortClick { get; set; }
        [Parameter] public EventCallback<string?> OnSearch { get; set; }
        [Parameter] public EventCallback OnCancelSearch { get; set; }
        [Parameter] public EventCallback OnAddFolderButtonClick { get; set; }   //ToDo: So many parameters! Is it fine?
        [Parameter] public EventCallback OnSortOrderClick { get; set; }



        public List<FsArtifact> SelectedArtifacts { get; set; } = new List<FsArtifact>();
        public ViewModeEnum ViewMode = ViewModeEnum.list;
        public bool IsSelected;
        public bool IsSelectedAll = false;
        public bool IsFiltered = false;
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

        private async Task HandelArtifactMoveClick()
        {
            await OnSelectDestination.InvokeAsync(SelectedArtifacts.ToArray());
        }

        private async Task HandleMultiArtifactsOptionsClick()
        {
            await OnMultiArtifactsOptionsClick.InvokeAsync(SelectedArtifacts.ToArray());
        }

        private bool IsInRoot(FsArtifact? artifact)
        {
            return artifact is null ? true : false;
        }

        public void HandleSortClick()
        {
            OnSortClick.InvokeAsync();
        }

        public void HandleSortOrderClick()
        {

            OnSortOrderClick.InvokeAsync();
        }

        public void ToggleSelectedAll()
        {
            if (ArtifactExplorerMode == ArtifactExplorerMode.Normal)
            {
                ArtifactExplorerMode = ArtifactExplorerMode.SelectArtifact;
                IsSelectedAll = !IsSelectedAll;
                IsSelected = false;
                SelectedArtifacts = Artifacts?.ToList();
            }
        }

        public void ChangeViewMode(ViewModeEnum mode)
        {
            ViewMode = mode;
        }

        public void CancelSelectionMode()
        {
            ArtifactExplorerMode = ArtifactExplorerMode.Normal;
            SelectedArtifacts = new List<FsArtifact>();
            IsSelectedAll = false;
            IsSelected = true;
        }

        public async Task HandleCancelSelectDestionationMode()
        {
            await OnCancelSelectDestionationMode.InvokeAsync();
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
        }

        public void OnSelectionChanged(FsArtifact selectedArtifact)
        {
            var item = SelectedArtifacts.Any(item => item.FullPath == selectedArtifact.FullPath);
            IsSelected = item;

            if (IsSelected)
            {
                SelectedArtifacts.Remove(selectedArtifact);
            }
            else
            {
                SelectedArtifacts.Add(selectedArtifact);
            }
        }

        public void OnCreateFolder()
        {
            OnAddFolderButtonClick.InvokeAsync();
        }

        public async Task OnGoToTopPage()
        {
            await JSRuntime.InvokeVoidAsync("OnScrollEvent");
        }

        public async Task OnScrollCheck()
        {
            await JSRuntime.InvokeVoidAsync("OnScrollCheck");
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

        private void HandleFilterClick()
        {
            IsFiltered = !IsFiltered;
            OnFilterClick.InvokeAsync();
        }
    }
}

