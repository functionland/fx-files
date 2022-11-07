using Functionland.FxFiles.Client.Shared.Components.Modal;
using Functionland.FxFiles.Client.Shared.Extensions;
using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakeFulaFileClient : IFulaFileClient
{
    readonly ConcurrentDictionary<FulaUser, List<KeyValuePair<FsArtifact, Stream?>>> _fulaUserNodes = new();
    IStringLocalizer<AppStrings> StringLocalizer { get; set; }
    TimeSpan? ActionLatency { get; set; }
    TimeSpan? EnumerationLatency { get; set; }

    public FakeFulaFileClient(Dictionary<FulaUser, List<KeyValuePair<FsArtifact, Stream?>>> artifacts,
                              IStringLocalizer<AppStrings> stringLocalizer,
                              TimeSpan? actionLatency = null,
                              TimeSpan? enumerationLatency = null)
    {
        _fulaUserNodes.Clear();

        StringLocalizer = stringLocalizer;
        ActionLatency = actionLatency ?? TimeSpan.FromSeconds(2);
        EnumerationLatency = enumerationLatency ?? TimeSpan.FromMilliseconds(9);

        foreach (var artifact in artifacts)
        {
            if (_fulaUserNodes.TryAdd(artifact.Key, artifact.Value))
            {
                throw new InvalidOperationException("Can not fill _artifacts");
            }
        }
    }

    public async Task AddFolderAsync(string token, string path, string folderName, string originDevice, CancellationToken? cancellationToken = null)
    {
        var dtNow = DateTimeOffset.Now;
        var lowerCaseFolder = StringLocalizer[nameof(AppStrings.Folder)].Value.ToLowerFirstChar();

        await LatencyActionAsync();

        if (string.IsNullOrWhiteSpace(path))
            throw new ArtifactPathNullException(StringLocalizer.GetString(nameof(AppStrings.ArtifactPathIsNull), lowerCaseFolder));

        if (string.IsNullOrWhiteSpace(folderName))
            throw new ArtifactNameNullException(StringLocalizer.GetString(nameof(AppStrings.ArtifactNameIsNull), lowerCaseFolder));

        if (CheckIfNameHasInvalidChars(folderName))
            throw new ArtifactInvalidNameException(StringLocalizer.GetString(nameof(AppStrings.ArtifactNameHasInvalidChars), lowerCaseFolder));

        var newPath = Path.Combine(path, folderName);
        if (ArtifactExists(token, newPath))
            throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(nameof(AppStrings.ArtifactAlreadyExistsException)));

        var newFolder = new FsArtifact(path, folderName, FsArtifactType.Folder, FsFileProviderType.Fula)
        {
            ArtifactPermissionLevel = ArtifactPermissionLevel.Write | ArtifactPermissionLevel.Read | ArtifactPermissionLevel.Delegate,
            CreateDateTime = dtNow,
            FullPath = newPath,
            OriginDevice = originDevice,
            ParentFullPath = path,
            ContentHash = Guid.NewGuid().ToString(),
            LastModifiedDateTime = DateTimeOffset.Now,
        };

        await AddArtifactAsync(token, newFolder, null, cancellationToken);
    }

    public async Task DeleteArtifactsAsync(string token, IEnumerable<string> sourcesPath, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();
        FulaUser user = GetFulaUser(token);

        _fulaUserNodes.TryGetValue(user, out var node);

        if (node == null) throw new Exception("File not found");

        var toRemove = node
            .Where(artifact =>
                    sourcesPath
                        .Any(source =>
                            artifact.Key.FullPath.StartsWith(source)
                            )
                        )
            .ToList();

        toRemove.ForEach(c => node.Remove(c));
    }

    public async Task<List<string>> CopyArtifactsAsync(string token, IEnumerable<string> sourcePaths, string destinationPath, bool overwrite = false, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();
        FulaUser user = GetFulaUser(token);

        _fulaUserNodes.TryGetValue(user, out var node);

        if (node == null) throw new Exception("File not found");

        if (string.IsNullOrWhiteSpace(destinationPath))
            throw new NullReferenceException(nameof(destinationPath));

        var destination = await GetArtifactAsync(token, destinationPath, cancellationToken);

        if (destination == null)
            throw new Exception("The destination does not exist");

        var destinationChildren = await GetChildrenArtifactsAsync(token, destinationPath, cancellationToken).ToListAsync();

        destinationChildren ??= new List<FsArtifact>();

        var toCopy = node
            .Where(artifact =>
                    sourcePaths
                        .Any(source =>
                            artifact.Key.FullPath.StartsWith(source)
                            )
                        )
            .ToList();

        List<string> ignoreCopy = new();

        foreach (var item in toCopy)
        {
            var artifact = item.Key;
            //TODO: Check inner artifacts' FullPath
            if (destinationChildren.Any(c => c.Name.Equals(artifact.Name)) && !overwrite)
            {
                ignoreCopy.Add(artifact.FullPath);
            }
            else
            {
                artifact.FullPath = $"{destinationPath}/{artifact.Name}";
                artifact.ParentFullPath = destinationPath;
            }
        }

        return ignoreCopy;
    }

    public async Task<List<string>> MoveArtifactsAsync(string token, IEnumerable<string> sourcePaths, string destinationPath, bool overwrite = false, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        var ignored = await CopyArtifactsAsync(token, sourcePaths, destinationPath, overwrite, onProgress, cancellationToken);
        await DeleteArtifactsAsync(token, sourcePaths, onProgress, cancellationToken);
        return ignored;
    }

    public async Task<List<FsArtifactActivity>> GetActivityHistoryAsync(string token, string path, long? page = null, long? pageSize = null, CancellationToken? cancellationToken = null)
    {
        var artifact = await GetArtifactAsync(token, path);
        if (artifact?.FsArtifactActivity is null) throw new NullReferenceException(nameof(artifact));

        if (page == null || pageSize == null)
        {
            return artifact.FsArtifactActivity;
        }
        else
        {
            return artifact.FsArtifactActivity.Skip((int)((page - 1) * pageSize))
                 .Take((int)pageSize).ToList();
        }
    }

    public async Task<FsArtifact> GetArtifactAsync(string token, string? path = null, CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();
        var lowerCaseArtifact = StringLocalizer[nameof(AppStrings.Artifact)].Value.ToLowerFirstChar();

        if (string.IsNullOrWhiteSpace(path))
            throw new ArtifactPathNullException(StringLocalizer.GetString(nameof(AppStrings.ArtifactPathIsNull), lowerCaseArtifact));

        if (!ArtifactExists(token, path))
            throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(nameof(AppStrings.ArtifactDoseNotExistsException)));

        var artifacts = GetUserArtifacts(token);

        var artifact = artifacts.Where(c => c.Key.FullPath == path).FirstOrDefault().Key;

        return new FsArtifact(artifact.FullPath, artifact.Name, artifact.ArtifactType, artifact.ProviderType)
        {
            ParentFullPath = artifact.ParentFullPath
        };
    }

    public async Task<FsArtifact> GetArtifactMetaAsync(string token, string path, CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();
        var lowerCaseArtifact = StringLocalizer[nameof(AppStrings.Artifact)].Value.ToLowerFirstChar();

        if (string.IsNullOrWhiteSpace(path))
            throw new ArtifactPathNullException(StringLocalizer.GetString(nameof(AppStrings.ArtifactPathIsNull), lowerCaseArtifact));

        if (!ArtifactExists(token, path))
            throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(nameof(AppStrings.ArtifactDoseNotExistsException)));

        var artifacts = GetUserArtifacts(token);

        return artifacts.Where(c => c.Key.FullPath == path).FirstOrDefault().Key;
    }

    public async IAsyncEnumerable<FsArtifact> GetChildrenArtifactsAsync(string token, string? path = null, CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();
        var lowerCaseArtifact = StringLocalizer[nameof(AppStrings.Artifact)].Value.ToLowerFirstChar();

        if (string.IsNullOrWhiteSpace(path))
            throw new ArtifactPathNullException(StringLocalizer.GetString(nameof(AppStrings.ArtifactPathIsNull), lowerCaseArtifact));

        if (!ArtifactExists(token, path))
            throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(nameof(AppStrings.ArtifactDoseNotExistsException)));

        var artifacts = GetUserArtifacts(token);
        var result = artifacts.Where(c => c.Key.ParentFullPath == path).ToList();

        foreach (var item in result)
        {
            yield return item.Key;
        }
    }

    public async Task<Stream> GetFileStreamAsync(string token, string filePath, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        var lowerCasefile = StringLocalizer[nameof(AppStrings.File)].Value.ToLowerFirstChar();
        await LatencyActionAsync();

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArtifactPathNullException(StringLocalizer.GetString(nameof(AppStrings.ArtifactPathIsNull), lowerCasefile));

        if (!ArtifactExists(token, filePath))
            throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(nameof(AppStrings.ArtifactDoseNotExistsException)));

        var artifacts = GetUserArtifacts(token);

        var result = artifacts.Where(c => c.Key.FullPath == filePath).FirstOrDefault().Value;

        if (result is null)
            throw new NullReferenceException(nameof(result));

        return result;
    }


    public async Task<string> GetLinkForShareAsync(string token, string path, CancellationToken? cancellationToken = null)
    {
        _ = GetFulaUser(token);
        await LatencyActionAsync();
        return "https://th.bing.com/th/id/OIP.iAhcp6m_91O-ClK79h8EQQHaFj?pid=ImgDet&rs=1";
    }

    public async IAsyncEnumerable<FsArtifact> GetSharedByMeArtifacsAsync(string token, CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();
        var artifacts = GetUserArtifacts(token);

        foreach (var item in artifacts.Where(c => c.Key.IsSharedByMe == true).ToList())
        {
            yield return item.Key;
        }
    }

    public async Task RenameFileAsync(string token, string filePath, string newName, CancellationToken? cancellationToken = null)
    {
        var lowerCasefile = StringLocalizer[nameof(AppStrings.File)].Value.ToLowerFirstChar();
        await LatencyActionAsync();

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArtifactPathNullException(StringLocalizer.GetString(nameof(AppStrings.ArtifactPathIsNull), lowerCasefile));

        if (!ArtifactExists(token, filePath))
            throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(nameof(AppStrings.ArtifactDoseNotExistsException)));

        var artifacts = GetUserArtifacts(token);

        var artifact = artifacts
            .Where(c => c.Key.FullPath == filePath && c.Key.ArtifactType == FsArtifactType.File)
            .FirstOrDefault().Key;

        if (artifact is null)
            throw new NullReferenceException("artifact does not found");

        var directoryName = Path.GetDirectoryName(artifact.FullPath);
        if (string.IsNullOrEmpty(Path.GetExtension(newName)))
        {
            var oldNameExtention = Path.GetExtension(artifact.Name);
            newName += oldNameExtention;
        }
        var newPath = Path.Combine(directoryName, newName);

        if (ArtifactExists(token, newPath))
            throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, lowerCasefile));

        artifact.FullPath = newPath;
        artifact.Name = newName;
    }

    public async Task RenameFolderAsync(string token, string folderPath, string newName, CancellationToken? cancellationToken = null)
    {
        var lowerCaseFolder = StringLocalizer[nameof(AppStrings.Folder)].Value.ToLowerFirstChar();
        await LatencyActionAsync();

        if (string.IsNullOrWhiteSpace(folderPath))
            throw new ArtifactPathNullException(StringLocalizer.GetString(nameof(AppStrings.ArtifactPathIsNull), lowerCaseFolder));

        if (!ArtifactExists(token, folderPath))
            throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(nameof(AppStrings.ArtifactDoseNotExistsException)));

        var artifacts = GetUserArtifacts(token);

        var artifact = artifacts
            .Where(c => c.Key.FullPath == folderPath && c.Key.ArtifactType == FsArtifactType.Folder)
            .FirstOrDefault().Key;

        if (artifact is null)
            throw new NullReferenceException("artifact does not found");

        var oldName = artifact.Name;
        var oldPath = artifact.FullPath;

        artifact.FullPath = $"{artifact.ParentFullPath}/{newName}";
        artifact.Name = newName;

        var children = artifacts.Select(c => c.Key).Where(c => c.ParentFullPath.Equals(oldPath)).ToList();

        await UpdateParentFullPathAsync(oldPath, artifact.FullPath, children, cancellationToken);

    }

    public async IAsyncEnumerable<FsArtifact> SearchArtifactsAsync(string token, string? path = null, string? searchText = null, CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();
        var artifacts = GetUserArtifacts(token).Select(c => c.Key).ToList();

        var result = new List<FsArtifact>();

        result = artifacts
            .Where(c => path == null || c.FullPath.Equals(path))
            .Where(c => searchText != null && c.Name.ToLower().Contains(searchText.ToLower()))
            .ToList();

        foreach (var item in result)
        {
            yield return item;
        }
    }

    public async Task SetPermissionArtifactsAsync(string token, IEnumerable<ArtifactPermissionInfo> permissionInfos, CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();
        var artifacts = GetUserArtifacts(token)
            .Select(c => c.Key)
            .Where(c => permissionInfos
                    .Select(a => a.FullPath)
                    .Contains(c.FullPath));


        foreach (var item in permissionInfos)
        {
            await LatencyEnumerationAsync();
            var artifact = artifacts.Where(c => c.FullPath.Equals(item.FullPath)).FirstOrDefault();
            artifact.PermissionUsers ??= new List<ArtifactPermissionInfo>();
            artifact.PermissionUsers.Add(item);
        }
    }

    public async Task UpdateFileAsync(string token, string path, Stream stream, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        var lowerCasefile = StringLocalizer[nameof(AppStrings.File)].Value.ToLowerFirstChar();
        await LatencyActionAsync();

        if (string.IsNullOrWhiteSpace(path))
            throw new ArtifactPathNullException(StringLocalizer.GetString(nameof(AppStrings.ArtifactPathIsNull), lowerCasefile));

        if (!ArtifactExists(token, path))
            throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(nameof(AppStrings.ArtifactDoseNotExistsException)));

        var artifacts = GetUserArtifacts(token);

        var artifact = artifacts.Where(c => c.Key.FullPath.Equals(path)).FirstOrDefault();
        artifacts.Remove(artifact);
        artifact.Key.ContentHash = Guid.NewGuid().ToString();
        artifacts.Add(new KeyValuePair<FsArtifact, Stream?>(artifact.Key, stream));

        await UpdateParentContentHashAsync(artifact.Key, artifacts.Select(c => c.Key), cancellationToken);
    }

    public async Task UploadFileAsync(string token, string path, string originDevice, Stream stream, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        var dtNow = DateTimeOffset.Now;
        var lowerCasefile = StringLocalizer[nameof(AppStrings.File)].Value.ToLowerFirstChar();
        await LatencyActionAsync();

        if (string.IsNullOrWhiteSpace(path))
            throw new ArtifactPathNullException(StringLocalizer.GetString(nameof(AppStrings.ArtifactPathIsNull), lowerCasefile));

        if (!ArtifactExists(token, path))
            throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(nameof(AppStrings.ArtifactDoseNotExistsException)));

        var name = Path.GetFileName(path);

        var newFile = new FsArtifact(path, name, FsArtifactType.File, FsFileProviderType.Fula)
        {
            ArtifactPermissionLevel = ArtifactPermissionLevel.Write | ArtifactPermissionLevel.Read | ArtifactPermissionLevel.Delegate,
            CreateDateTime = dtNow,
            FileExtension = Path.GetExtension(path),
            Size = stream.Length,
            FullPath = path,
            OriginDevice = originDevice,
            ParentFullPath = Directory.GetParent(path)?.FullName,
            ContentHash = Guid.NewGuid().ToString(),
            LastModifiedDateTime = DateTimeOffset.Now,
        };

        await AddArtifactAsync(token, newFile, stream, cancellationToken);
        var fulaUserNode = GetUserArtifacts(token);

        await UpdateSizeOfArtifactAsync(newFile, stream, fulaUserNode, cancellationToken);
    }

    private static bool CheckIfNameHasInvalidChars(string name)
    {
        var invalidChars = Path.GetInvalidFileNameChars();

        foreach (var invalid in invalidChars)
            if (name.Contains(invalid)) return true;
        return false;
    }

    private bool ArtifactExists(string token, string path)
    {
        var userArtifacts = GetUserArtifacts(token);
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;
        return userArtifacts.Any(f => comparer.Compare(f.Key.FullPath, path) == 0);
    }

    private async Task AddArtifactAsync(string token, FsArtifact fsArtifact, Stream? stream, CancellationToken? cancellationToken = null)
    {
        var user = GetFulaUser(token);
        var fulaUserNode = GetUserArtifacts(token);

        if (fulaUserNode.Any())
        {
            fulaUserNode.Add(new KeyValuePair<FsArtifact, Stream?>(fsArtifact, stream));
        }
        else
        {
            var fulaFileRootArtifact = new FsArtifact(FulaConvention.FulaFilesRootPath, "name", FsArtifactType.Folder, FsFileProviderType.Fula);
            _fulaUserNodes.TryAdd(user, new List<KeyValuePair<FsArtifact, Stream?>>
            {
                new KeyValuePair<FsArtifact, Stream?>(fulaFileRootArtifact, null),
                new KeyValuePair<FsArtifact, Stream?>(fsArtifact, stream)
            });
        }

        await UpdateParentContentHashAsync(fsArtifact, fulaUserNode.Select(c => c.Key), cancellationToken);
    }

    private List<KeyValuePair<FsArtifact, Stream?>> GetUserArtifacts(string token)
    {
        var dId = FulaUserUtils.GetFulaDId(token);

        var user = _fulaUserNodes.Keys.Where(c => c.DId.Equals(c.DId)).FirstOrDefault();

        if (user == null || !_fulaUserNodes.TryGetValue(user, out var artifacts))
            throw new InvalidOperationException("User must exist");

        return artifacts ?? new List<KeyValuePair<FsArtifact, Stream?>>();
    }

    private FulaUser GetFulaUser(string token)
    {
        var dId = FulaUserUtils.GetFulaDId(token);

        if (string.IsNullOrWhiteSpace(dId))
            throw new InvalidOperationException("User not found");

        var user = _fulaUserNodes.Keys.Where(c => c.DId.Equals(dId)).FirstOrDefault();
        if (user == null)
            throw new InvalidOperationException("User not found");

        return user;
    }

    private async Task LatencyActionAsync()
    {
        if (ActionLatency is not null)
            await Task.Delay(ActionLatency.Value);
    }

    private async Task LatencyEnumerationAsync()
    {
        if (EnumerationLatency is not null)
            await Task.Delay(EnumerationLatency.Value);
    }

    private async Task UpdateParentFullPathAsync(string parentPath, string newParentPath, List<FsArtifact> artifacts, CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();
        var fsArtifacts = artifacts.Where(c => c.ParentFullPath.Equals(parentPath)).ToList();
        if (!fsArtifacts.Any())
            return;

        foreach (var item in fsArtifacts)
        {
            await LatencyEnumerationAsync();
            var oldPath = item.FullPath;
            item.ParentFullPath = newParentPath;
            item.FullPath = $"{item.ParentFullPath}/{item.Name}";
            await UpdateParentFullPathAsync(oldPath, item.FullPath, fsArtifacts, cancellationToken);
        }
    }

    private async Task UpdateParentContentHashAsync(FsArtifact artifact, IEnumerable<FsArtifact> fsArtifacts, CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();
        var parent = fsArtifacts.Where(c => c.FullPath == artifact.ParentFullPath).FirstOrDefault();
        if (parent == null) return;

        parent.ContentHash = Guid.NewGuid().ToString();
        await UpdateParentContentHashAsync(parent, fsArtifacts, cancellationToken);
    }

    private async Task UpdateSizeOfArtifactAsync(FsArtifact fsArtifact, Stream? stream, List<KeyValuePair<FsArtifact, Stream?>> fsArtifacts, CancellationToken? cancellationToken)
    {
        if (fsArtifact.ArtifactType == FsArtifactType.File)
        {
            fsArtifact.Size = stream?.Length ?? 0;
            var parent = fsArtifacts.Where(c => c.Key.FullPath.Equals(fsArtifact.ParentFullPath)).FirstOrDefault().Key;
            if (parent is null)
                return;

            await UpdateSizeOfArtifactAsync(parent, null, fsArtifacts, cancellationToken);
        }
        else
        {
            var children = fsArtifacts.Where(c => c.Key.ParentFullPath == fsArtifact.FullPath).Select(c => c.Key).ToList();
            fsArtifact.Size = children.Sum(c => c.Size);

            var parent = fsArtifacts.Where(c => c.Key.FullPath.Equals(fsArtifact.ParentFullPath)).FirstOrDefault().Key;
            if (parent is null)
                return;

            await UpdateSizeOfArtifactAsync(parent, null, fsArtifacts, cancellationToken);
        }


    }
}
