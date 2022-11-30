using Functionland.FxFiles.Client.Shared.Components.Common;

namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ArtifactSelectionModal
{
    private bool _isModalOpen;
    private TaskCompletionSource<ArtifactSelectionResult>? _tcs;
    private List<FsArtifact> _artifacts = new();
    private FsArtifact? _currentArtifact;
    private string _buttonText = string.Empty;
    private List<FsArtifact> _excludedArtifacts = new();
    private InputModal _inputModalRef = default!;
    private FsArtifact? _scrolledToArtifact;
    private FxBreadcrumbs? _breadcrumbsRef;

    [Parameter] public SortTypeEnum SortType { get; set; } = SortTypeEnum.Name;
    [Parameter] public bool IsAscOrder { get; set; }
    [Parameter] public bool IsMultiple { get; set; }
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public IArtifactThumbnailService<IFileService> ThumbnailService { get; set; } = default!;

    public async Task<ArtifactSelectionResult> ShowAsync(FsArtifact? artifact, string buttonText, List<FsArtifact> excludedArtifacts)
    {
        GoBackService.SetState((Task () =>
        {
            Close();
            StateHasChanged();
            return Task.CompletedTask;
        }), true, false);

        _tcs?.SetCanceled();
        _currentArtifact = artifact;
        _buttonText = buttonText;
        _excludedArtifacts = excludedArtifacts;
        await LoadArtifacts(artifact?.FullPath);

        _isModalOpen = true;
        StateHasChanged();

        _tcs = new TaskCompletionSource<ArtifactSelectionResult>();
        var result = await _tcs.Task;

        GoBackService.ResetToPreviousState();

        return result;
    }

    private async Task SelectArtifact(FsArtifact artifact)
    {
        if (_breadcrumbsRef is null)
            return;

        await JSRuntime.InvokeVoidAsync("breadCrumbStyle", _breadcrumbsRef.BreadcrumbsRef);
        _currentArtifact = artifact;
        await LoadArtifacts(artifact.FullPath);
        StateHasChanged();
    }

    private void SelectDestination()
    {
        try
        {
            if (_currentArtifact is null)
            {
                return;
            }

            var result = new ArtifactSelectionResult
            {
                ResultType = ArtifactSelectionResultType.Ok,
                SelectedArtifacts = new[] { _currentArtifact }
            };

            _tcs?.SetResult(result);
            _tcs = null;
            _isModalOpen = false;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task LoadArtifacts(string? path)
    {
        try
        {
            _artifacts = new List<FsArtifact>();
            var artifacts = FileService.GetArtifactsAsync(path);
            var artifactPaths = _excludedArtifacts.Select(a => a.FullPath);

            await foreach (var item in artifacts)
            {
                if (item.ArtifactType == FsArtifactType.File || (artifactPaths != null && artifactPaths.Contains(item.FullPath)))
                {
                    item.IsDisabled = true;
                }

                _artifacts.Add(item);
            }
        }
        catch (IOException ex)
        {
            ExceptionHandler.Handle(new KnownIOException(ex.Message, ex));
        }
        catch (UnauthorizedAccessException ex)
        {
            ExceptionHandler.Handle(new UnauthorizedException(ex.Message, ex));
        }
        catch (Exception exception)
        {
            ExceptionHandler.Handle(exception);
        }
    }

    //TODO: Move to service and use in ArtifactExplorer
    private async Task CreateFolder()
    {
        if (_inputModalRef is null)
            return;

        var createFolder = Localizer.GetString(AppStrings.CreateFolder);
        var newFolderPlaceholder = Localizer.GetString(AppStrings.NewFolderPlaceholder);

        var result = await _inputModalRef.ShowAsync(createFolder, string.Empty, string.Empty, newFolderPlaceholder);

        try
        {
            if (result?.ResultType == InputModalResultType.Confirm)
            {
                var newFolder = await FileService.CreateFolderAsync(_currentArtifact.FullPath, result?.Result); //ToDo: Make CreateFolderAsync nullable
                _scrolledToArtifact = newFolder;
                _artifacts.Add(newFolder);
                RefreshArtifacts();
            }
        }
        catch (Exception exception)
        {
            ExceptionHandler?.Handle(exception);
        }
    }

    private void RefreshArtifacts()
    {
        _artifacts = ApplySortArtifacts(_artifacts).ToList();
    }

    private IEnumerable<FsArtifact> ApplySortArtifacts(IEnumerable<FsArtifact> artifacts)
    {
        IEnumerable<FsArtifact> sortedArtifactsQuery = SortType switch
        {
            SortTypeEnum.LastModified when IsAscOrder => artifacts
                .OrderBy(artifact => artifact.ArtifactType != FsArtifactType.Folder)
                .ThenBy(artifact => artifact.LastModifiedDateTime),
            SortTypeEnum.LastModified => artifacts
                .OrderByDescending(artifact => artifact.ArtifactType == FsArtifactType.Folder)
                .ThenByDescending(artifact => artifact.LastModifiedDateTime),
            SortTypeEnum.Size when IsAscOrder => artifacts
                .OrderBy(artifact => artifact.ArtifactType != FsArtifactType.Folder)
                .ThenBy(artifact => artifact.Size),
            SortTypeEnum.Size => artifacts.OrderByDescending(artifact => artifact.ArtifactType == FsArtifactType.Folder)
                .ThenByDescending(artifact => artifact.Size),
            SortTypeEnum.Name when IsAscOrder => artifacts
                .OrderBy(artifact => artifact.ArtifactType != FsArtifactType.Folder)
                .ThenBy(artifact => artifact.Name),
            SortTypeEnum.Name => artifacts.OrderByDescending(artifact => artifact.ArtifactType == FsArtifactType.Folder)
                .ThenByDescending(artifact => artifact.Name),
            _ => artifacts.OrderBy(artifact => artifact.ArtifactType != FsArtifactType.Folder)
                .ThenBy(artifact => artifact.Name)
        };

        return sortedArtifactsQuery;
    }

    private async Task Back()
    {
        try
        {
            _currentArtifact = await FileService.GetArtifactAsync(_currentArtifact?.ParentFullPath);
        }
        catch (DomainLogicException ex) when (ex is ArtifactPathNullException)
        {
            _currentArtifact = null;
        }

        await LoadArtifacts(_currentArtifact?.FullPath);
    }

    private void Close()
    {
        var result = new ArtifactSelectionResult
        {
            ResultType = ArtifactSelectionResultType.Cancel
        };

        _tcs?.SetResult(result);
        _tcs = null;
        _isModalOpen = false;
    }

    public void Dispose()
    {
        _tcs?.SetCanceled();
    }
}