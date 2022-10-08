using Functionland.FxFiles.Client.Shared.Components.Common;
using Functionland.FxFiles.Client.Shared.Models;
using Microsoft.AspNetCore.Components.Web;

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
        [Parameter] public bool IsLoading { get; set; }

        private System.Timers.Timer? _timer;

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
            _timer = new(500);
            _timer.Enabled = true;
            _timer.Start();

            _timer.Elapsed += async (sender, e) =>
            {
                if (_timer.Enabled)
                {
                    IsSelected = false;
                    SelectedArtifacts = Array.Empty<FsArtifact>();
                    ArtifactExplorerMode = ArtifactExplorerMode.SelectArtifact;

                    await InvokeAsync(() =>
                    {
                        ArtifactExplorerModeChanged.InvokeAsync(ArtifactExplorerMode);
                        StateHasChanged();
                    });
                }

                _timer.Enabled = false;
                _timer.Stop();
            };
        }

        public async Task PointerUp(FsArtifact artifact)
        {
            if (_timer.Enabled && ArtifactExplorerMode != ArtifactExplorerMode.SelectArtifact)
            {
                _timer.Stop();
                _timer.Enabled = false;

                await OnSelectArtifact.InvokeAsync(artifact);
                await JSRuntime.InvokeVoidAsync("OnScrollEvent");
            }
            else
            {
                await SelectedArtifactsChanged.InvokeAsync(SelectedArtifacts);
            }
        }

        public void PointerMove()
        {
            _timer.Stop();
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
                        return "pdf-file-icon";
                    case FileCategoryType.Image:
                        return "photo-file-icon";
                    case FileCategoryType.Audio:
                        return "audio-file-icon";
                    case FileCategoryType.Video:
                        return "video-file-icon";
                    case FileCategoryType.App:
                        return "app-file-icon";
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