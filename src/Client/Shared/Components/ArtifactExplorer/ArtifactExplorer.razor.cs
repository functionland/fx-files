using System;

using Functionland.FxFiles.Client.Shared.Components.Common;
using Functionland.FxFiles.Client.Shared.Models;

using Microsoft.AspNetCore.Components.Web;

namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class ArtifactExplorer
    {
        [Parameter] public FsArtifact? CurrentArtifact { get; set; }
        [Parameter] public List<FsArtifact> Artifacts { get; set; } = default!;
        [Parameter] public EventCallback<List<FsArtifact>> ArtifactsChanged { get; set; }
        [Parameter] public SortTypeEnum CurrentSortType { get; set; } = SortTypeEnum.Name;
        [Parameter] public EventCallback<FsArtifact> OnArtifactsOptionsClick { get; set; } = new();
        [Parameter] public EventCallback<FsArtifact> OnSelectArtifact { get; set; } = new();
        [Parameter] public ArtifactExplorerMode ArtifactExplorerMode { get; set; }
        [Parameter] public EventCallback<ArtifactExplorerMode> ArtifactExplorerModeChanged { get; set; }
        [Parameter] public EventCallback OnAddFolderButtonClick { get; set; }
        [Parameter] public EventCallback<bool> IsSelectedChanged { get; set; }
        [Parameter] public List<FsArtifact> SelectedArtifacts { get; set; } = new();
        [Parameter] public EventCallback<List<FsArtifact>> SelectedArtifactsChanged { get; set; }
        [Parameter] public ViewModeEnum ViewMode { get; set; } = ViewModeEnum.list;
        [Parameter] public FileCategoryType? FileCategoryFilter { get; set; }
        [Parameter] public bool IsLoading { get; set; }
        [Parameter] public EventCallback HandleBack { get; set; }

        private System.Timers.Timer? _timer;

        protected override Task OnInitAsync()
        {
            return base.OnInitAsync();
        }

        private async Task HandleArtifactOptionsClick(FsArtifact artifact)
        {
            await OnArtifactsOptionsClick.InvokeAsync(artifact);
        }

        //protected override Task OnParamsSetAsync()
        //{
        //    if (Artifacts is null)
        //    {
        //        Artifacts = Array.Empty<FsArtifact>();
        //    }
        //    return base.OnParamsSetAsync();
        //}

        private async Task HandleArtifactClick(FsArtifact artifact)
        {
            await OnSelectArtifact.InvokeAsync(artifact);
        }

        private bool IsInRoot(FsArtifact? artifact)
        {
            return artifact is null ? true : false;
        }

        public void PointerDown(FsArtifact artifact)
        {
            _timer = new(500);
            _timer.Enabled = true;
            _timer.Start();

            _timer.Elapsed += async (sender, e) =>
            {
                if (_timer.Enabled && ArtifactExplorerMode != ArtifactExplorerMode.SelectDestionation)
                {
                    _timer.Enabled = false;
                    _timer.Stop();
                    ArtifactExplorerMode = ArtifactExplorerMode.SelectArtifact;

                    await InvokeAsync(async () =>
                    {
                        await ArtifactExplorerModeChanged.InvokeAsync(ArtifactExplorerMode);
                        artifact.IsSelected = true;
                        await ArtifactsChanged.InvokeAsync(Artifacts);
                        //await OnSelectionChanged(artifact);
                    });
                }
            };
            //StateHasChanged();
        }

        public async Task PointerUp(FsArtifact artifact)
        {
            if (_timer != null)
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
            StateHasChanged();
        }

        public void PointerCancel()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
        }

        //public async Task OnSelectionChanged(FsArtifact artifact)
        //{
        //    if (SelectedArtifacts.Exists(a => a.FullPath == artifact.FullPath))
        //    {
        //        artifact.IsSelected = false;
        //        SelectedArtifacts.Remove(artifact);
        //    }
        //    else
        //    {
        //        artifact.IsSelected = true;
        //        SelectedArtifacts.Add(artifact);
        //    }

        //    await SelectedArtifactsChanged.InvokeAsync(SelectedArtifacts);
        //    StateHasChanged();
        //}

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

        (TouchPoint ReferencePoint, DateTimeOffset StartTime) startPoint;

        private void HandleTouchStart(TouchEventArgs t)
        {
            startPoint.ReferencePoint = t.TargetTouches[0];
            startPoint.StartTime = DateTimeOffset.Now;
        }

        private async Task HandleTouchEnd(TouchEventArgs t)
        {
            const double swipeThreshold = 0.3;
            if (startPoint.ReferencePoint == null)
            {
                return;
            }

            var endReferencePoint = t.ChangedTouches[0];

            var diffX = startPoint.ReferencePoint.ClientX - endReferencePoint.ClientX;
            var diffY = startPoint.ReferencePoint.ClientY - endReferencePoint.ClientY;
            var diffTime = DateTime.Now - startPoint.StartTime;
            var velocityX = Math.Abs(diffX / diffTime.Milliseconds);
            var velocityY = Math.Abs(diffY / diffTime.Milliseconds);

            //var run = Math.Abs(diffX);
            //var rise = Math.Abs(diffY);
            //var ang = Math.Atan2(rise, run) * (180/Math.PI);
            //
            //if (ang > 10 && ang < 80)
            //{
            //    message = "diagonal";
            //}

            if (velocityX < swipeThreshold && velocityY < swipeThreshold) return;
            if (Math.Abs(velocityX - velocityY) < .3) return;

            if (velocityX >= swipeThreshold)
            {
                if (velocityY >= swipeThreshold)
                {
                    return;
                }
                if (diffX < 0)
                {
                    await HandleBack.InvokeAsync();
                    return;
                }
            }
        }
    }
}