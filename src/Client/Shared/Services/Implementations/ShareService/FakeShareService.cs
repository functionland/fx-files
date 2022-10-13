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

    public Task ShareArtifactAsync(IEnumerable<string> dids, FsArtifact artifact, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task ShareArtifactsAsync(IEnumerable<string> dids, IEnumerable<FsArtifact> fsArtifact, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    private void ShareArtifact(IEnumerable<string> dids, FsArtifact artifact, CancellationToken? cancellationToken = null)
    {
        var lowerCaseArtifact = AppStrings.Artifact.ToLowerText();
        if (artifact is null)
            throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException, artifact?.ArtifactType.ToString() ?? lowerCaseArtifact));

        foreach (var did in dids)
        {
           
        }
    }
    public async Task RevokeShareArtifactAsync(IEnumerable<string> dids, string artifactFullPath, CancellationToken? cancellationToken = null)
    {
        RevokeShareArtifact(dids, artifactFullPath, cancellationToken);
    }

    public async Task RevokeShareArtifactsAsync(IEnumerable<string> dids, IEnumerable<string> artifactFullPaths, CancellationToken? cancellationToken = null)
    {
        foreach(var artifactFullPath in artifactFullPaths)
        {
            RevokeShareArtifact(dids, artifactFullPath, cancellationToken);
        }
    }

    private void RevokeShareArtifact(IEnumerable<string> dids, string artifactFullPath, CancellationToken? cancellationToken = null)
    {
        var lowerCaseArtifact = AppStrings.Artifact.ToLowerText();
        var artifact = _AllFsArtifacts?.FirstOrDefault(a => a.FullPath == artifactFullPath);
        var sharedFsArtifact = _SharedFsArtifacts?.FirstOrDefault(a => a.FullPath == artifactFullPath);

        if (artifact is null)
            throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException, artifact?.ArtifactType.ToString() ?? lowerCaseArtifact));

        if (sharedFsArtifact is null)
            throw new Exception();//TODO

        foreach(var did in dids)
        {
            var artifactPermissionInfo = new ArtifactPermissionInfo()
            {
                FullPath = artifactFullPath,
                DId = did,
                PermissionLevel = ArtifactPermissionLevel.None
            };

            artifact?.PermissionedUsers?.Remove(artifactPermissionInfo);
            _SharedFsArtifacts?.Remove(artifactPermissionInfo);
        }
    }
    public async IAsyncEnumerable<FsArtifact> GetSharedByMeArtifactsAsync(CancellationToken? cancellationToken = null)
    {
        var SharedByMeArtifacts = new List<FsArtifact>();

        if (_AllFsArtifacts is null) yield break;

        foreach (var artifact in _AllFsArtifacts)
        {
            if(artifact.IsSharedByMe == true)
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

    public async Task<List<FulaUser>> GetArtifactSharesAsync(string path, CancellationToken? cancellationToken = null)
    {
        var lowerCaseArtifact = AppStrings.Artifact.ToLowerText();
        var artifact = _AllFsArtifacts?.FirstOrDefault(a => a.FullPath == path);

        if (artifact is null)
            throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException, artifact?.ArtifactType.ToString() ?? lowerCaseArtifact));

        var permissionedUsers = artifact?.PermissionedUsers;

        if (permissionedUsers is null) throw new Exception();//TODO

        var fulaUser = new List< FulaUser >();

        foreach (var user in permissionedUsers)
        {
            var permissionedUser = _FulaUsers?.FirstOrDefault(a => a.DId == user.DId);

            if (permissionedUser is not null)
            {
                fulaUser.Add(permissionedUser);
            }      
        }

        return fulaUser;
    }   

}
