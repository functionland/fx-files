using System.Timers;

using Functionland.FxFiles.Client.Shared.Components.Common;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace Functionland.FxFiles.Client.Shared.Components;

public partial class ArtifactExplorer : IAsyncDisposable
{
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }

    private List<FsArtifact> _artifacts = default!;

    [Parameter]
    public List<FsArtifact> Artifacts
    {
        get => _artifacts;
        set
        {
            if (_artifacts == value)
                return;

            _artifacts = value;
            _isArtifactsChanged = true;
        }
    }

    [Parameter] public SortTypeEnum CurrentSortType { get; set; } = SortTypeEnum.Name;
    [Parameter] public EventCallback<FsArtifact> OnArtifactOptionClick { get; set; } = default!;
    [Parameter] public EventCallback<List<FsArtifact>> OnArtifactsOptionClick { get; set; } = default!;
    [Parameter] public EventCallback<FsArtifact> OnSelectArtifact { get; set; } = default!;
    [Parameter] public EventCallback IsTouchStarted { get; set; } = default!;
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
    [Parameter] public FsArtifact? ScrollArtifact { get; set; }
    [Parameter] public EventCallback OnScrollToArtifactCompleted { get; set; }
    [Parameter] public ElementReference? Breadcrumbs { get; set; }

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
    private int _overscanCount = 5;
    private bool _isArtifactsChanged;
    private bool _isInScrollArtifactAction;
    private bool _isScrolling;

    private string _resizeEventListenerId = string.Empty;

    private DotNetObjectReference<ArtifactExplorer>? _dotnetObjectReference;
    private (TouchPoint ReferencePoint, DateTimeOffset StartTime) _startPoint;
    public ElementReference? ArtifactExplorerListRef;

    protected override async Task OnInitAsync()
    {
        _dotnetObjectReference = DotNetObjectReference.Create(this);

        await base.OnInitAsync();
    }

    protected override async Task OnAfterFirstRenderAsync()
    {
        await base.OnAfterFirstRenderAsync();
        await JSRuntime.InvokeVoidAsync("UpdateWindowWidth", _dotnetObjectReference);
        await InitWindowWidthListener();
        await JSRuntime.InvokeVoidAsync("OnScrollCheck", ArtifactExplorerListRef);
        await JSRuntime.InvokeVoidAsync("createScrollStopListener", ArtifactExplorerListRef, _dotnetObjectReference);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (ScrollArtifact is not null && _isInScrollArtifactAction is false)
        {
            _isInScrollArtifactAction = true;
            _timer = new System.Timers.Timer(1000);
            _timer.Enabled = true;
            _timer.Start();
            _timer.Elapsed += async (s, e) => await ScrollTimerElapsed(s, e);
        }
    }

    [JSInvokable]
    public void SetScrolling(bool state)
    {
        _isScrolling = state;
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
        _resizeEventListenerId = Guid.NewGuid().ToString();
        await JSRuntime.InvokeVoidAsync("AddWindowWidthListener", _dotnetObjectReference, _resizeEventListenerId);
    }

    private async Task HandleArtifactOptionClick(FsArtifact artifact)
    {
        if (IsInSearchMode)
        {
            await JSRuntime.InvokeVoidAsync("SearchInputUnFocus");
            StateHasChanged();
        }

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
        if (_isScrolling)
            return;

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
            if (_timer.Enabled && ArtifactExplorerMode != ArtifactExplorerMode.SelectDestination)
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
        if (_timer == null)
            return;

        _timer.Enabled = false;
        _timer.Stop();
        _timer.Elapsed -= TimerElapsed;
        _timer.Dispose();
        _timer = null;
    }

    public async Task PointerUp(MouseEventArgs args, FsArtifact artifact)
    {
        if (_isScrolling)
            return;

        switch (args.Button)
        {
            case 0:
                {
                    if (_timer != null)
                    {
                        DisposeTimer();
                        if (ArtifactExplorerMode != ArtifactExplorerMode.SelectArtifact)
                        {
                            await OnSelectArtifact.InvokeAsync(artifact);
                            await JSRuntime.InvokeVoidAsync("breadCrumbStyle");
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

                    break;
                }
            case 2:
                DisposeTimer();
                switch (SelectedArtifacts.Count)
                {
                    case 0 when ArtifactExplorerMode == ArtifactExplorerMode.Normal:
                        await HandleArtifactOptionClick(artifact);
                        break;
                    case 1:
                        await HandleArtifactOptionClick(SelectedArtifacts[0]);
                        break;
                    case > 1:
                        await HandleArtifactsOptionClick(SelectedArtifacts);
                        break;
                }

                break;
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
        if (ArtifactExplorerMode == ArtifactExplorerMode.Normal)
        {
            await ChangeArtifactExplorerMode(ArtifactExplorerMode.SelectArtifact);
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

        if (SelectedArtifacts.Count == 0)
        {
            await ChangeArtifactExplorerMode(ArtifactExplorerMode.Normal);
        }

        await SelectedArtifactsChanged.InvokeAsync(SelectedArtifacts);
        StateHasChanged();
    }

    public async Task OnGoToTopPage()
    {
        await JSRuntime.InvokeVoidAsync("OnScrollEvent", ArtifactExplorerListRef);
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
        _startPoint.ReferencePoint = t.TargetTouches[0];
        _startPoint.StartTime = DateTimeOffset.Now;
        IsTouchStarted.InvokeAsync();
    }

    private async Task HandleTouchEnd(TouchEventArgs t)
    {
        const double swipeThreshold = 0.3;

        var endReferencePoint = t.ChangedTouches[0];

        var diffX = _startPoint.ReferencePoint.ClientX - endReferencePoint.ClientX;
        var diffY = _startPoint.ReferencePoint.ClientY - endReferencePoint.ClientY;
        var diffTime = DateTimeOffset.Now - _startPoint.StartTime;
        var velocityX = Math.Abs(diffX / diffTime.Milliseconds);
        var velocityY = Math.Abs(diffY / diffTime.Milliseconds);

        if (velocityX < swipeThreshold && velocityY < swipeThreshold) return;
        if (Math.Abs(velocityX - velocityY) < .3) return;

        if (velocityX >= swipeThreshold)
        {
            if (velocityY >= swipeThreshold)
                return;

            if (diffX < 0)
            {
                await HandleBack.InvokeAsync();
            }
        }
    }

    public void UpdateGridRowCount(int width)
    {
        bool shouldRefresh;

        switch (width)
        {
            case >= 530:
                shouldRefresh = true;
                _gridRowCount = 3;
                break;
            case >= 350:
                shouldRefresh = true;
                _gridRowCount = 2;
                break;
            default:
                shouldRefresh = true;
                _gridRowCount = 1;
                break;
        }

        if (shouldRefresh && ViewMode == ViewModeEnum.Grid && _virtualizeGridRef is not null)
        {
            _virtualizeGridRef.RefreshDataAsync();
        }

        StateHasChanged();
    }

    private async ValueTask<ItemsProviderResult<FsArtifact>> ProvideArtifactsListAsync(ItemsProviderRequest request)
    {
        var cancellationToken = request.CancellationToken;

        if (cancellationToken.IsCancellationRequested)
            return default;

        var requestCount = Math.Min(request.Count, Artifacts.Count - request.StartIndex);
        var items = Artifacts.Skip(request.StartIndex).Take(requestCount).ToList();

        _ = Task.Run(async () =>
        {
            await Task.Delay(300);
            var skipCount = Math.Min(_overscanCount, request.StartIndex);
            await LoadThumbnailsAsync(items.Skip(skipCount).ToList(), cancellationToken);
            await LoadThumbnailsAsync(items.Take(skipCount).ToList(), cancellationToken);
        }, cancellationToken);

        return new ItemsProviderResult<FsArtifact>(items: items, totalItemCount: Artifacts.Count);
    }

    private async ValueTask<ItemsProviderResult<FsArtifact[]>> ProvideArtifactGridAsync(ItemsProviderRequest request)
    {
        var cancellationToken = request.CancellationToken;
        if (cancellationToken.IsCancellationRequested) return default;

        var count = request.Count * _gridRowCount;
        var start = request.StartIndex * _gridRowCount;
        var requestCount = Math.Min(count, Artifacts.Count - start);

        var items = Artifacts.Skip(start).Take(requestCount).ToList();

        _ = Task.Run(async () =>
        {
            await Task.Delay(300);
            var skipCount = Math.Min(_overscanCount * _gridRowCount, request.StartIndex);
            await LoadThumbnailsAsync(items.Skip(skipCount).ToList(), cancellationToken);
            await LoadThumbnailsAsync(items.Take(skipCount).ToList(), cancellationToken);
        }, cancellationToken);

        var result = items.Chunk(_gridRowCount).ToList();

        return new ItemsProviderResult<FsArtifact[]>(items: result,
            totalItemCount: (int)Math.Ceiling((decimal)Artifacts.Count / _gridRowCount));
    }

    private async Task LoadThumbnailsAsync(List<FsArtifact> items, CancellationToken cancellationToken)
    {
        if (IsInZipMode)
        {
            return;
        }

        foreach (var item in items)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            try
            {
                if (item.ThumbnailPath is not null)
                    continue;

                item.ThumbnailPath =
                    await ThumbnailService.GetOrCreateThumbnailAsync(item, ThumbnailScale.Small,
                        cancellationToken);

                await InvokeAsync(StateHasChanged);
            }
            catch (Exception exception)
            {
                ExceptionHandler.Track(exception);
            }
        }
    }

    private async Task HandleZipArtifactClickAsync(FsArtifact artifact)
    {
        await OnZipArtifactClick.InvokeAsync(artifact);
    }

    private async Task ChangeArtifactExplorerMode(ArtifactExplorerMode mode)
    {
        ArtifactExplorerMode = mode;
        await ArtifactExplorerModeChanged.InvokeAsync(ArtifactExplorerMode);
    }

    private string GetIdForArtifact(string artifactName)
    {
        var id = artifactName.Trim().Replace(" ", string.Empty);
        return id;
    }

    private async Task<bool> ScrollToArtifact(FsArtifact artifact)
    {
        var listHeight = Artifacts.FindIndex(a => a.FullPath == artifact.FullPath) * 74;
        var listExistResult =
            await JSRuntime.InvokeAsync<bool>("scrollToItem", GetIdForArtifact(artifact.Name), listHeight,
                ArtifactExplorerListRef);
        return listExistResult;
    }

    private async Task ScrollTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _timer?.Stop();
        if (ScrollArtifact == null || IsLoading)
        {
            _timer?.Start();
            return;
        }

        var isListExist = await ScrollToArtifact(ScrollArtifact);
        if (_timer == null || isListExist is false)
        {
            _timer?.Start();
            return;
        }

        DisposeTimer();
        await ScrollToArtifact(ScrollArtifact);

        ScrollArtifact = null;
        _isInScrollArtifactAction = false;
        await InvokeAsync(OnScrollToArtifactCompleted.InvokeAsync);
    }

    public async ValueTask DisposeAsync()
    {
        await JSRuntime.InvokeVoidAsync("RemoveWindowWidthListener", _resizeEventListenerId);
        await JSRuntime.InvokeVoidAsync("removeScrollStopListener");
        _dotnetObjectReference?.Dispose();
    }
}