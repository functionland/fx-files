using Functionland.FxFiles.Client.Shared.Components.Common;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace Functionland.FxFiles.Client.Shared.Components;

public partial class ArtifactExplorer
{
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }


    private List<FsArtifact> _artifacts = default!;
    [Parameter]
    public List<FsArtifact> Artifacts
    {
        get => _artifacts;
        set
        {
            if (_artifacts != value)
            {
                _artifacts = value;
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
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public bool IsInSearchMode { get; set; }
    [Parameter] public IArtifactThumbnailService<IFileService> ThumbnailService { get; set; } = default!;
    [Parameter] public bool IsInZipMode { get; set; }
    [Parameter] public EventCallback<FsArtifact> OnZipArtifactClick { get; set; }
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
    (TouchPoint ReferencePoint, DateTimeOffset StartTime) startPoint;

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
        return artifact is null;
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
            }
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

    public async Task OnGoToTopPage()
    {
        await JSRuntime.InvokeVoidAsync("OnScrollEvent");
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
                case FileCategoryType.Zip:
                    return "zip-file-icon";
            }
        }

        return "folder-icon";
    }

    private void HandleTouchStart(TouchEventArgs t)
    {
        startPoint.ReferencePoint = t.TargetTouches[0];
        startPoint.StartTime = DateTimeOffset.Now;
    }

    private async Task HandleTouchEnd(TouchEventArgs t)
    {
        const double swipeThreshold = 0.3;
        //if (startPoint.ReferencePoint == null)
        //{
        //    return;
        //}

        var endReferencePoint = t.ChangedTouches[0];

        var diffX = startPoint.ReferencePoint.ClientX - endReferencePoint.ClientX;
        var diffY = startPoint.ReferencePoint.ClientY - endReferencePoint.ClientY;
        var diffTime = DateTimeOffset.Now - startPoint.StartTime;
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
                return;

            if (diffX < 0)
                await HandleBack.InvokeAsync();
        }
    }

    public void UpdateGridRowCount(int width)
    {
        bool shouldRefresh;

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

        if (shouldRefresh && ViewMode == ViewModeEnum.Grid && _virtualizeGridRef is not null)
        {
            _virtualizeGridRef.RefreshDataAsync();
        }

        StateHasChanged();
    }

    private async ValueTask<ItemsProviderResult<FsArtifact>> ProvideArtifactsList(ItemsProviderRequest request)
    {
        if (request.CancellationToken.IsCancellationRequested) return default;

        var requestCount = Math.Min(request.Count, Artifacts.Count - request.StartIndex);
        List<FsArtifact> items = Artifacts.Skip(request.StartIndex).Take(requestCount).ToList();

        _ = Task.Run(async () =>
        {
            await Task.Delay(300);
            foreach (var item in items)
            {
                if (request.CancellationToken.IsCancellationRequested)
                    return;
                try
                {
                    var thumbPath =
                        await ThumbnailService.GetOrCreateThumbnailAsync(item, ThumbnailScale.Small,
                            request.CancellationToken);
                    await InvokeAsync(() =>
                    {
                        item.ThumbnailPath = thumbPath;
                    });
                }
                catch (Exception exception)
                {
                    ExceptionHandler.Track(exception);
                }
            }

            await InvokeAsync(() => { StateHasChanged(); });
        });

        return new ItemsProviderResult<FsArtifact>(items: items, totalItemCount: Artifacts.Count);
    }

    private async ValueTask<ItemsProviderResult<FsArtifact[]>> ProvideArtifactGrid(ItemsProviderRequest request)
    {
        if (request.CancellationToken.IsCancellationRequested) return default;

        var count = request.Count * _gridRowCount;
        var start = request.StartIndex * _gridRowCount;
        var requestCount = Math.Min(count, Artifacts.Count - start);

        List<FsArtifact> items = Artifacts.Skip(start).Take(requestCount).ToList();

        _ = Task.Run(async () =>
        {
            await Task.Delay(300);
            foreach (var item in items)
            {
                if (request.CancellationToken.IsCancellationRequested)
                    return;

                try
                {
                    var thumbPath =
                        await ThumbnailService.GetOrCreateThumbnailAsync(item, ThumbnailScale.Small,
                            request.CancellationToken);
                    await InvokeAsync(() =>
                    {
                        item.ThumbnailPath = thumbPath;
                    });

                }
                catch (Exception exception)
                {
                    ExceptionHandler.Track(exception);
                }
            }

            await InvokeAsync(() => { StateHasChanged(); });
        });

        var result = items.Chunk(_gridRowCount).ToList();

        return new ItemsProviderResult<FsArtifact[]>(items: result, totalItemCount: (int)Math.Ceiling((decimal)Artifacts.Count / _gridRowCount));
    }

    private async Task HandleZipArtifactClickAsync(FsArtifact artifact)
    {
        await OnZipArtifactClick.InvokeAsync(artifact);
    }

    public void Dispose()
    {
        JSRuntime.InvokeVoidAsync("RemoveWindowWidthListener", _objectReference);
        _objectReference?.Dispose();
    }
}