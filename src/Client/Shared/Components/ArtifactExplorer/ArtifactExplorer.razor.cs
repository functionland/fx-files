using System;

using Functionland.FxFiles.Client.Shared.Components.Common;
using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Functionland.FxFiles.Client.Shared.Utils;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class ArtifactExplorer
    {
        [Parameter] public FsArtifact? CurrentArtifact { get; set; }
        [Parameter]
        public List<FsArtifact> Artifacts
        {
            get => artifacts;
            set
            {
                if (artifacts != value)
                {
                    artifacts = value;
                    _isArtifactsChanged = true;
                }
            }
        }
        [Parameter] public SortTypeEnum CurrentSortType { get; set; } = SortTypeEnum.Name;
        [Parameter] public EventCallback<FsArtifact> OnArtifactOptionClick { get; set; } = default!;
        [Parameter] public EventCallback<List<FsArtifact>> OnArtifactsOptionClick { get; set; } = default!;
        [Parameter] public EventCallback<FsArtifact> OnSelectArtifact { get; set; } = default!;
        [Parameter] public ArtifactExplorerMode ArtifactExplorerMode { get; set; }
        [Parameter] public EventCallback<ArtifactExplorerMode> ArtifactExplorerModeChanged { get; set; }
        [Parameter] public EventCallback OnAddFolderButtonClick { get; set; }
        [Parameter] public List<FsArtifact> SelectedArtifacts { get; set; } = new();
        [Parameter] public EventCallback<List<FsArtifact>> SelectedArtifactsChanged { get; set; }
        [Parameter] public ViewModeEnum ViewMode { get; set; } = ViewModeEnum.List;
        [Parameter] public FileCategoryType? FileCategoryFilter { get; set; }
        [Parameter] public bool IsLoading { get; set; }
        [Parameter] public EventCallback HandleBack { get; set; }
        [Parameter] public IFileService FileService { get; set; }
        [Parameter] public bool IsInSearchMode { get; set; }
        [AutoInject] public IArtifactThumbnailService<ILocalDeviceFileService> ThumbnailService { get; set; } = default!;
        public PathProtocol Protocol =>
            FileService switch
            {
                ILocalDeviceFileService => PathProtocol.Storage,
                IFulaFileService => PathProtocol.Fula,
                _ => throw new InvalidOperationException($"Unsupported file service: {FileService}")
            };

        public int WindowWidth { get; set; }

        private System.Timers.Timer? _timer;

        private FsArtifact? _longPressedArtifact;

        private Virtualize<FsArtifact>? _virtualizeListRef;
        private Virtualize<FsArtifact[]>? _virtualizeGridRef;

        private int _gridRowCount = 2;

        private bool _isArtifactsChanged;

        private DotNetObjectReference<ArtifactExplorer>? _objectReference;

        protected override async Task OnInitAsync()
        {
            _objectReference = DotNetObjectReference.Create(this);

            await base.OnInitAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("UpdateWindowWidth", _objectReference);
                await InitWindowWidthListener();
            }
        }

        protected override async Task OnParamsSetAsync()
        {
            if (_isArtifactsChanged)
            {
                if (ViewMode == ViewModeEnum.List && _virtualizeListRef is not null)
                {
                    await _virtualizeListRef.RefreshDataAsync();
                    _isArtifactsChanged = false;
                }

                if (ViewMode == ViewModeEnum.Grid && _virtualizeGridRef is not null)
                {
                    await _virtualizeGridRef.RefreshDataAsync();
                    _isArtifactsChanged = false;
                }
            }

            await base.OnParamsSetAsync();
        }

        [JSInvokable]
        public void UpdateWindowWidth(int windowWidth)
        {
            WindowWidth = windowWidth;
            UpdateGridRowCount(WindowWidth);
            StateHasChanged();
        }

        private async Task InitWindowWidthListener()
        {
            await JSRuntime.InvokeVoidAsync("AddWindowWidthListener", _objectReference);
        }

        private async Task HandleArtifactOptionClick(FsArtifact artifact)
        {
            await OnArtifactOptionClick.InvokeAsync(artifact);
        }

        private async Task HandleArtifactsOptionClick(List<FsArtifact> artifacts)
        {
            await OnArtifactsOptionClick.InvokeAsync(artifacts);
        }

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
            _longPressedArtifact = artifact;
            _timer = new(1000);
            _timer.Enabled = true;
            _timer.Start();
            _timer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (_timer != null)
            {
                if (_timer.Enabled && ArtifactExplorerMode != ArtifactExplorerMode.SelectDestionation)
                {
                    DisposeTimer();
                    ArtifactExplorerMode = ArtifactExplorerMode.SelectArtifact;

                    InvokeAsync(async () =>
                    {
                        if (_longPressedArtifact != null)
                        {
                            await ArtifactExplorerModeChanged.InvokeAsync(ArtifactExplorerMode);
                            _longPressedArtifact.IsSelected = true;
                            await OnSelectionChanged(_longPressedArtifact);
                            _longPressedArtifact = null;
                        }
                        StateHasChanged();
                    });
                };
            }
            DisposeTimer();
        }

        private void DisposeTimer()
        {
            if (_timer != null)
            {
                _timer.Enabled = false;
                _timer.Stop();
                _timer.Elapsed -= TimerElapsed;
                _timer.Dispose();
            }
        }

        public async Task PointerUp(MouseEventArgs args, FsArtifact artifact)
        {
            if (args.Button == 0)
            {
                if (_timer != null)
                {
                    DisposeTimer();
                    if (ArtifactExplorerMode != ArtifactExplorerMode.SelectArtifact)
                    {
                        await OnSelectArtifact.InvokeAsync(artifact);
                        await JSRuntime.InvokeVoidAsync("OnScrollEvent");
                    }
                    else
                    {
                        if (_longPressedArtifact != null)
                        {
                            await OnSelectionChanged(artifact);
                            await SelectedArtifactsChanged.InvokeAsync(SelectedArtifacts);
                        }
                    }
                }
            }
            else if (args.Button == 2)
            {
                DisposeTimer();
                if (SelectedArtifacts.Count == 0 && ArtifactExplorerMode == ArtifactExplorerMode.Normal)
                {
                    await HandleArtifactOptionClick(artifact);
                }
                else if (SelectedArtifacts.Count == 1)
                {
                    await HandleArtifactOptionClick(SelectedArtifacts[0]);
                }
                else if (SelectedArtifacts.Count > 1)
                {
                    await HandleArtifactsOptionClick(SelectedArtifacts);
                }
            }
            StateHasChanged();
        }

        public void PointerCancel()
        {
            DisposeTimer();
        }

        public async Task OnSelectionChanged(FsArtifact artifact)
        {
            DisposeTimer();
            if (true)
            {
                if (ArtifactExplorerMode == ArtifactExplorerMode.Normal)
                {
                    ArtifactExplorerMode = ArtifactExplorerMode.SelectArtifact;
                    await ArtifactExplorerModeChanged.InvokeAsync(ArtifactExplorerMode);
                }
                if (SelectedArtifacts.Exists(a => a.FullPath == artifact.FullPath))
                {
                    artifact.IsSelected = false;
                    SelectedArtifacts.Remove(artifact);
                }
                else
                {
                    artifact.IsSelected = true;
                    SelectedArtifacts.Add(artifact);
                }

                await SelectedArtifactsChanged.InvokeAsync(SelectedArtifacts);
                StateHasChanged();
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
        private List<FsArtifact> artifacts = default!;

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

        public void UpdateGridRowCount(int width)
        {
            bool shouldRefresh = false;

            if (width >= 530)
            {
                shouldRefresh = true;
                _gridRowCount = 3;
            }
            else if (width >= 350)
            {
                shouldRefresh = true;
                _gridRowCount = 2;
            }
            else
            {
                shouldRefresh = true;
                _gridRowCount = 1;
            }

            if (shouldRefresh == true && ViewMode == ViewModeEnum.Grid && _virtualizeGridRef is not null)
            {
                _virtualizeGridRef.RefreshDataAsync();
            }

            StateHasChanged();
        }

        private bool _isLoadingThumbnailFinished;
        private async ValueTask<ItemsProviderResult<FsArtifact>> ProvideArtifactsList(ItemsProviderRequest request)
        {
            _isLoadingThumbnailFinished = false;
            if (_isArtifactsChanged == false)
            {
                await Task.Delay(300);
            }

            if (request.CancellationToken.IsCancellationRequested) return default;

            var requestCount = Math.Min(request.Count, Artifacts.Count - request.StartIndex);
            List<FsArtifact> items = Artifacts.Skip(request.StartIndex).Take(requestCount).ToList();

            foreach (var item in items)
            {
                if (request.CancellationToken.IsCancellationRequested) return default;

                try
                {
                    item.ThumbnailPath = await ThumbnailService.GetOrCreateThumbnailAsync(item, ThumbnailScale.Small, request.CancellationToken);
                }
                catch
                {
                    item.ThumbnailPath = null;
                }
            }

            _isLoadingThumbnailFinished = true;

            return new ItemsProviderResult<FsArtifact>(items: items, totalItemCount: Artifacts.Count);
        }

        private async ValueTask<ItemsProviderResult<FsArtifact[]>> ProvideArtifactGrid(ItemsProviderRequest request)
        {
            _isLoadingThumbnailFinished = false;
            if (_isArtifactsChanged == false)
            {
                await Task.Delay(300);
            }

            if (request.CancellationToken.IsCancellationRequested) return default;

            var count = request.Count * _gridRowCount;
            var start = request.StartIndex * _gridRowCount;
            var requestCount = Math.Min(count, Artifacts.Count - start);

            List<FsArtifact> items = Artifacts.Skip(start).Take(requestCount).ToList();

            foreach (var item in items)
            {
                if (request.CancellationToken.IsCancellationRequested) return default;

                try
                {
                    item.ThumbnailPath = await ThumbnailService.GetOrCreateThumbnailAsync(item, ThumbnailScale.Small, request.CancellationToken);
                }
                catch
                {
                    item.ThumbnailPath = null;
                }
            }
            _isLoadingThumbnailFinished = true;

            var result = new List<FsArtifact[]>();
            if (_gridRowCount == 1)
            {
                for (int i = 0; i < items.Count; i += 1)
                {
                    if (i < items.Count)
                    {
                        result.Add(new FsArtifact[1] { items[i] });
                        continue;
                    }
                    result.Add(new FsArtifact[1] { items[i] });
                }
            }
            else if (_gridRowCount == 2)
            {
                for (int i = 0; i < items.Count; i += 2)
                {
                    if ((i + 1) < items.Count)
                    {
                        result.Add(new FsArtifact[2] { items[i], items[i + 1] });
                        continue;
                    }
                    result.Add(new FsArtifact[1] { items[i] });
                }
            }
            else if (_gridRowCount == 3)
            {
                for (int i = 0; i < items.Count; i += 3)
                {
                    if ((i + 2) < items.Count)
                    {
                        result.Add(new FsArtifact[3] { items[i], items[i + 1], items[i + 2] });
                        continue;
                    }
                    else if ((i + 1) < items.Count)
                    {
                        result.Add(new FsArtifact[2] { items[i], items[i + 1] });
                        continue;
                    }
                    result.Add(new FsArtifact[1] { items[i] });
                }
            }

            return new ItemsProviderResult<FsArtifact[]>(items: result, totalItemCount: (int)Math.Ceiling((decimal)Artifacts.Count / _gridRowCount));
        }

        public void Dispose()
        {
            JSRuntime.InvokeVoidAsync("RemoveWindowWidthListener", _objectReference);
            _objectReference?.Dispose();
        }
    }
}