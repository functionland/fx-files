using Functionland.FxFiles.Client.Shared.Components.Modal;
using Functionland.FxFiles.Client.Shared.Extensions;
using Microsoft.Extensions.Localization;
using SQLitePCL;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakeFulaFileClient : IFulaFileClient
{
    private readonly ConcurrentDictionary<FulaUser, List<KeyValuePair<FsArtifact, Stream?>>> _artifacts = new();
    private const string FulaFileRootPath = "/Fula";
    public IStringLocalizer<AppStrings> StringLocalizer { get; set; }
    public TimeSpan? ActionLatency { get; set; }
    public TimeSpan? EnumerationLatency { get; set; }

    public async Task LatencyActionAsync()
    {
        if (ActionLatency is not null)
            await Task.Delay(ActionLatency.Value);
    }

    public async Task LatencyEnumerationAsync()
    {
        if (EnumerationLatency is not null)
            await Task.Delay(EnumerationLatency.Value);
    }

    public FakeFulaFileClient(Dictionary<FulaUser, List<KeyValuePair<FsArtifact, Stream?>>> artifacts,
                              IStringLocalizer<AppStrings> stringLocalizer,
                              TimeSpan? actionLatency = null,
                              TimeSpan? enumerationLatency = null)
    {
        _artifacts.Clear();

        StringLocalizer = stringLocalizer;
        ActionLatency = actionLatency ?? TimeSpan.FromSeconds(2);
        EnumerationLatency = enumerationLatency ?? TimeSpan.FromMilliseconds(9);

        foreach (var artifact in artifacts)
        {
            //check output
            _artifacts.TryAdd(artifact.Key, artifact.Value);
        }
    }

    public async Task AddFolderAsync(string token, string path, string folderName, CancellationToken? cancellationToken = null)
    {
        var lowerCaseFolder = AppStrings.Folder.ToLowerText();

        //Possible Exceptions
        if (string.IsNullOrWhiteSpace(path))
            throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, lowerCaseFolder));

        if (string.IsNullOrWhiteSpace(folderName))
            throw new ArtifactNameNullException(StringLocalizer.GetString(AppStrings.ArtifactNameIsNull, lowerCaseFolder));

        if (CheckIfNameHasInvalidChars(folderName))
            throw new ArtifactInvalidNameException(StringLocalizer.GetString(AppStrings.ArtifactNameHasInvalidChars, lowerCaseFolder));

        var newPath = Path.Combine(path, folderName);
        if (ArtifactExists(newPath))
            throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException));

        //CreateFolder in a seperate method?
        var newFolder = new FsArtifact(path, folderName, FsArtifactType.Folder, FsFileProviderType.Fula)
        {
            //Size = ,
            ContentHash = "",
            LastModifiedDateTime = DateTimeOffset.UtcNow,
        };
        _artifacts.Add(newFolder);
        await LatencyActionAsync();
    }

    public Task<List<string>> CopyArtifactsAsync(string token, IEnumerable<string> sourcePaths, string destinationPath, bool overwrite = false, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();

    }

    public Task DeleteArtifactsAsync(string token, IEnumerable<string> sourcesPath, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<FsArtifactActivity>> GetActivityHistoryAsync(string token, string path, long? page = null, long? pageSize = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public async Task<FsArtifact> GetArtifactAsync(string token, string? path = null, CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();
        var lowerCaseArtifact = AppStrings.Artifact.ToLower();

        if (string.IsNullOrWhiteSpace(path))
            throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, lowerCaseArtifact));

        if (!ArtifactExists(path))
            throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException));

        //Nullability issue here have already been checked in the above conditional statement. Check the ArtifactExists method for more info.
        return _artifacts.FirstOrDefault(artifact => artifact.FullPath == path)!;

    }

    public async Task<FsArtifact> GetArtifactMetaAsync(string token, string path, CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();
        var artifact = _artifacts.FirstOrDefault(artifact => artifact.FullPath == path);

        if (artifact is null)
            throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException));

        return new FsArtifact(artifact.FullPath, artifact.Name, artifact.ArtifactType, FsFileProviderType.Fula)
        {
            //Fill all the fields? From Where?! From artifact itself or generate the data here?
        };
    }

    public IAsyncEnumerable<FsArtifact> GetChildrenArtifactsAsync(string token, string? path = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public async Task<Stream> GetFileStreamAsync(string token, string filePath, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        //Only jpg, png, jpeg and txt file types are supported for now.

        await LatencyActionAsync();
        string streamPath;
        if (Path.GetExtension(filePath).ToLower() == ".jpg" ||
            Path.GetExtension(filePath).ToLower() == ".png" ||
            Path.GetExtension(filePath).ToLower() == ".jpeg"
            )
        {
            streamPath = "/Files/fake-pic.jpg";
        }
        else
        {
            streamPath = "/Files/test.txt";
        }

        using FileStream fs = File.Open(streamPath, FileMode.Open);
        return fs;
    }

    //ToDo: Which link exactly?
    public Task<string> GetLinkForShareAsync(string token, string path, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<FsArtifact> GetSharedByMeArtifacsAsync(string token, CancellationToken? cancellationToken = null)
    {
        //Private Methods:
        //Did GetDId(token)
        //List<FsArtifact> GetArtifacts(token)
        throw new NotImplementedException();
    }

    //This one doesn't exist in interface!
    //public IAsyncEnumerable<FsArtifact> GetSharedWithMeArtifacsAsync(string token, CancellationToken? cancellationToken = null)
    //{
    //    throw new NotImplementedException();
    //}

    public Task<List<string>> MoveArtifactsAsync(string token, IEnumerable<string> sourcePaths, string destinationPath, bool overwrite = false, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public async Task RenameFileAsync(string token, string filePath, string newName, CancellationToken? cancellationToken = null)
    {
        //Method arguments aren't declared nullable. Still nullability check is needed?

        //Possible Exceptions
        if (!ArtifactExists(filePath))
            throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException));

        foreach (var artifact in _artifacts)
        {
            await LatencyEnumerationAsync();
            if (artifact.ArtifactType is not FsArtifactType.File) continue;

            if (string.Equals(artifact.FullPath, filePath, StringComparison.CurrentCultureIgnoreCase))
            {
                //newName has been entered without extention. So we should retrieve the extention from preious file name.
                if (string.IsNullOrEmpty(Path.GetExtension(newName)))
                {
                    var OldFileNameExtention = artifact.FileExtension;
                    if (!string.IsNullOrEmpty(OldFileNameExtention))    //Is this checking really necessary? Why on earth would it be not true?
                    {
                        newName += OldFileNameExtention;
                    }

                    var parentPath = Path.GetDirectoryName(filePath);
                    var newPath = Path.Combine(parentPath, newName);

                    //There is already a file with the same name.
                    if (ArtifactExists(newPath))
                        throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException));

                    artifact.FullPath = newPath;
                    artifact.Name = newName;
                    break;

                }
            }
        }
    }

    public async Task RenameFolderAsync(string token, string folderPath, string newName, CancellationToken? cancellationToken = null)
    {
        var oldFolderName = Path.GetFileName(folderPath);
        foreach (var artifact in _artifacts)
        {
            if (artifact.ArtifactType is not FsArtifactType.Folder) continue;

            await LatencyEnumerationAsync();

            //var artifactName = Path.GetFileName(artifact.FullPath.TrimEnd(Path.DirectorySeparatorChar)); Why this?
            var artifactName = Path.GetFileName(artifact.FullPath);
            var parentPath = Path.GetDirectoryName(folderPath);

            //Here in fake service, parentPath is a substring of artifact.FullPath. So if artifactName is not null, it ensures us of parentPath's existing.
            var newPath = Path.Combine(parentPath!, newName);

            //Check if newly generated path already exists.
            if (ArtifactExists(newPath))
                throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException));

            //Rename folder itself.
            if (string.Equals(artifactName, oldFolderName, StringComparison.CurrentCultureIgnoreCase))
            {
                artifact.FullPath = newPath;
                artifact.Name = newName;
            }
            //Rename artifacts inside that folder which is about to be renamed (the path of child artifacts should also change).
            else if (artifact.FullPath.ToLower().StartsWith(folderPath.ToLower()))
            {
                artifact.FullPath = artifact.FullPath.Replace($"{Path.DirectorySeparatorChar}{oldFolderName}{Path.DirectorySeparatorChar}",
                                                              $"{Path.DirectorySeparatorChar}{newName}{Path.DirectorySeparatorChar}");
            }

        }
    }

    public IAsyncEnumerable<FsArtifact> SearchArtifactsAsync(string token, string? path = null, string? searchText = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task SetPermissionArtifactsAsync(string token, IEnumerable<ArtifactPermissionInfo> permissionInfos, CancellationToken? cancellationToken = null)
    {
        //if (!permissionInfos.Any())
        //    throw new Exception();
        throw new NotImplementedException();
    }

    public Task UpdateFileAsync(string token, string path, Stream stream, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public async Task UploadFileAsync(string token, string path, Stream stream, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        await LatencyActionAsync();

        var dId = token.Split(',')[0];
        var fulaUser = _artifacts.Keys.Where(f => f.DId == dId).FirstOrDefault();
        if (fulaUser is null)
        {
            throw new Exception();
        }
        _ = _artifacts.TryGetValue(fulaUser, out var fulaUserNode);

        var fsArtifact = new FsArtifact(path, "name", FsArtifactType.File, FsFileProviderType.Fula);
        if (fulaUserNode is null)
        {
            var fulaFileRootArtifact = new FsArtifact(FulaFileRootPath, "name", FsArtifactType.Folder, FsFileProviderType.Fula);
            _artifacts.AddOrUpdate(fulaUser, new List<KeyValuePair<FsArtifact, Stream?>>
            {
                new KeyValuePair<FsArtifact, Stream?>(fulaFileRootArtifact, null),
                new KeyValuePair<FsArtifact, Stream?>(fsArtifact, stream)
            }, (k, v) => v);
        }
        else
        {
            fulaUserNode.Add(new KeyValuePair<FsArtifact, Stream?>(fsArtifact, stream));
        }

    }


    //Private Methods
    private bool CheckIfNameHasInvalidChars(string name)
    {
        var invalidChars = Path.GetInvalidFileNameChars();

        foreach (var invalid in invalidChars)
            if (name.Contains(invalid)) return true;
        return false;
    }

    private bool ArtifactExists(string newPath)
    {
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;
        return _artifacts.Any(f => comparer.Compare(f.FullPath, newPath) == 0);
    }

}
