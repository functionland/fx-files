using Functionland.FxFiles.Client.Shared.Extensions;
using Microsoft.Extensions.Localization;
using System.IO;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakeShareService : IFulaShareService, ILocalDeviceShareService
{
    private readonly List<FulaUser>? _FulaUsers;
    private readonly List<FsArtifact>? _AllFsArtifacts;
    private readonly List<ArtifactPermissionInfo>? _SharedFsArtifacts;
    public TimeSpan? ActionLatency { get; set; }
    public TimeSpan? EnumerationLatency { get; set; }
    public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

    public Task InitAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task EnsureInitializedAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    //public Task ShareArtifactAsync(IEnumerable<string> dids, FsArtifact artifact, CancellationToken? cancellationToken = null)
    //{
    //    throw new NotImplementedException();
    //}

    //public Task ShareArtifactsAsync(IEnumerable<string> dids, IEnumerable<FsArtifact> fsArtifact, CancellationToken? cancellationToken = null)
    //{
    //    throw new NotImplementedException();
    //}

    //private void ShareArtifact(IEnumerable<string> dids, FsArtifact artifact, CancellationToken? cancellationToken = null)
    //{
    //    var lowerCaseArtifact = AppStrings.Artifact.ToLowerText();
    //    if (artifact is null)
    //        throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException, artifact?.ArtifactType.ToString() ?? lowerCaseArtifact));

    //    foreach (var did in dids)
    //    {

    //    }
    //}
    //public async Task RevokeShareArtifactAsync(IEnumerable<string> dids, string artifactFullPath, CancellationToken? cancellationToken = null)
    //{
    //    var lowerCaseArtifact = AppStrings.Artifact.ToLowerText();
    //    var artifact = _AllFsArtifacts?.FirstOrDefault(a => a.FullPath == artifactFullPath);
    //    var sharedFsArtifact = _SharedFsArtifacts?.FirstOrDefault(a => a.FullPath == artifactFullPath);

    //    if (artifact is null)
    //        throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException, artifact?.ArtifactType.ToString() ?? lowerCaseArtifact));

    //    if (sharedFsArtifact is null)
    //        throw new Exception();//TODO

    //    foreach (var did in dids)
    //    {
    //        var artifactPermissionInfo = new ArtifactPermissionInfo()
    //        {
    //            FullPath = artifactFullPath,
    //            DId = did,
    //            PermissionLevel = ArtifactPermissionLevel.None
    //        };

    //        _SharedFsArtifacts?.Remove(artifactPermissionInfo);
    //    }

    //    var sharesArtifacts = await GetArtifactSharesAsync(artifactFullPath, cancellationToken);


    //}

    //public async Task RevokeShareArtifactsAsync(IEnumerable<string> dids, IEnumerable<string> artifactFullPaths, CancellationToken? cancellationToken = null)
    //{
    //    foreach (var artifactFullPath in artifactFullPaths)
    //    {
    //        await RevokeShareArtifactAsync(dids, artifactFullPath, cancellationToken);
    //    }
    //}


    public async IAsyncEnumerable<FsArtifact> GetSharedByMeArtifactsAsync(CancellationToken? cancellationToken = null)
    {
        var SharedByMeArtifacts = new List<FsArtifact>();

        if (_AllFsArtifacts is null) yield break;

        foreach (var artifact in _AllFsArtifacts)
        {
            if (artifact.IsSharedByMe == true)
            {
                SharedByMeArtifacts.Add(artifact);
            }

            yield return artifact;
        }
    }

    public async IAsyncEnumerable<FsArtifact> GetSharedWithMeArtifactsAsync(CancellationToken? cancellationToken = null)
    {
        var SharedWithMeArtifacts = new List<FsArtifact>();

        if (_AllFsArtifacts is null) yield break;

        foreach (var artifact in _AllFsArtifacts)
        {
            if (artifact.IsSharedWithMe == true)
            {
                SharedWithMeArtifacts.Add(artifact);
            }

            yield return artifact;
        }
    }

    public async Task<bool> IsSahredByMeAsync(string path, CancellationToken? cancellationToken = null)
    {
        var lowerCaseArtifact = AppStrings.Artifact.ToLowerText();
        var artifact = _AllFsArtifacts?.FirstOrDefault(a => a.FullPath == path);

        if (artifact is null)
            throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException, artifact?.ArtifactType.ToString() ?? lowerCaseArtifact));

        if (artifact.IsSharedByMe == true)
            return true;

        return false;
    }

    public async Task<List<ArtifactUserPermission>> GetArtifactSharesAsync(string path, CancellationToken? cancellationToken = null)
    {
        var lowerCaseArtifact = AppStrings.Artifact.ToLowerText();
        var artifact = _AllFsArtifacts?.FirstOrDefault(a => a.FullPath == path);

        if (artifact is null)
            throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException, artifact?.ArtifactType.ToString() ?? lowerCaseArtifact));


        var permissionedUsers = _SharedFsArtifacts?.Where(a => a.FullPath == path).ToList();
        return permissionedUsers?
            .Select(c =>
                new ArtifactUserPermission(_FulaUsers.First(a => a.DId == c.DId))
                {
                    PermissionLevel = c.PermissionLevel
                })
            .ToList() ?? new List<ArtifactUserPermission>();
    }

    public async Task SetPermissionArtifactsAsync(IEnumerable<ArtifactPermissionInfo> permissionInfos, CancellationToken? cancellationToken = null)
    {

        foreach (var permissionInfo in permissionInfos)
        {
            var sharedItem = _SharedFsArtifacts
                .Where(c => c.FullPath == permissionInfo.FullPath && c.DId == permissionInfo.DId)
                .FirstOrDefault();

            if (sharedItem == null)
            {
                if (permissionInfo.PermissionLevel == ArtifactPermissionLevel.None)
                    throw new Exception(""); //TODO: correct exception

                sharedItem = new ArtifactPermissionInfo
                {
                    DId = permissionInfo.DId,
                    FullPath = permissionInfo.FullPath,
                    PermissionLevel = permissionInfo.PermissionLevel
                };

                _SharedFsArtifacts.Add(sharedItem);
            }
            else
            {
                if (permissionInfo.PermissionLevel == ArtifactPermissionLevel.None)
                {
                    _SharedFsArtifacts.Remove(sharedItem);
                }
                else
                {
                    sharedItem.PermissionLevel = permissionInfo.PermissionLevel;
                }
            }

            var artifact = _AllFsArtifacts.Where(c => c.FullPath == permissionInfo.FullPath).First();

            artifact.PermissionedUsers = await GetArtifactSharesAsync(permissionInfo.FullPath, cancellationToken);
        }
    }
}
