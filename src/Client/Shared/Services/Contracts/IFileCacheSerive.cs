namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IOfflineAvailablityService
{
    Task LoginAsync(string did, CancellationToken? cancellationToken = null);
    Task InitAsync(CancellationToken? cancellationToken = null);
    //Task EnsureInitializedAsync();
    Task MakeOfflineAvailableAsync(FsArtifact artifact, CancellationToken? cancellationToken = null);
    Task RemoveOfflineAvailableAsync(FsArtifact artifact, CancellationToken? cancellationToken = null);
    Task<bool> IsAvailableOfflineAsync(FsArtifact artifact, CancellationToken? cancellationToken = null);
}
