using Functionland.FxFiles.Client.Shared.Components.Modal;

namespace Functionland.FxFiles.Client.Shared.Services.FulaClient.Contracts;

public interface IFulaFileClient
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>ToDo: Resume Upload Support</remarks>
    /// <param name="token"></param>
    /// <param name="path"></param>
    /// <param name="stream"></param>
    /// <param name="onProgress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UploadFileAsync(string token, string path, Stream stream, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null);
    Task AddFolderAsync(string token, string path, string folderName, CancellationToken? cancellationToken = null);
    Task<Stream> GetFileStreamAsync(string token, string filePath, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="sourcePaths"></param>
    /// <param name="destinationPath"></param>
    /// <param name="overwrite"></param>
    /// <param name="onProgress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The failed moved artifacts</returns>
    Task<List<string>> MoveArtifactsAsync(string token, IEnumerable<string> sourcePaths, string destinationPath, bool overwrite = false, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="sourcePaths"></param>
    /// <param name="destinationPath"></param>
    /// <param name="overwrite"></param>
    /// <param name="onProgress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The failed copied artifacts</returns>
    Task<List<string>> CopyArtifactsAsync(string token, IEnumerable<string> sourcePaths, string destinationPath, bool overwrite = false, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null);
    Task RenameFileAsync(string token, string filePath, string newName, CancellationToken? cancellationToken = null);
    Task RenameFolderAsync(string token, string folderPath, string newName, CancellationToken? cancellationToken = null);
    Task DeleteArtifactsAsync(string token, IEnumerable<string> sourcesPath, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null);
    /// <summary>
    /// AsyncEnumerable or Async?
    /// </summary>
    /// <remarks>ToDo: Specifiy the required fields to return.</remarks>
    /// <param name="token"></param>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<FsArtifact> GetChildrenArtifactsAsync(string token, string? path = null, CancellationToken? cancellationToken = null);
    /// <summary>
    /// AsyncEnumerable or Async?
    /// </summary>
    /// <remarks>ToDo: Specifiy the required fields to return.</remarks>
    /// <param name="token"></param>
    /// <param name="path"></param>
    /// <param name="searchText"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<FsArtifact> SearchArtifactsAsync(string token, string? path = null, string? searchText = null, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>ToDo: Specifiy the required fields to return.</remarks>
    /// <param name="token"></param>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FsArtifact> GetArtifactAsync(string token, string? path = null, CancellationToken? cancellationToken = null);

    Task SetPermissionArtifactsAsync(string token, IEnumerable<ArtifactPermissionInfo> permissionInfos, CancellationToken? cancellationToken = null);
   
    IAsyncEnumerable<FsArtifact> GetSharedByMeArtifacsAsync(string token, CancellationToken? cancellationToken = null);
    IAsyncEnumerable<FsArtifact> GetSharedWithMeArtifacsAsync(string token, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Specify the retuning fields</remarks>
    /// <param name="token"></param>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FsArtifact> GetArtifactMetaAsync(string token, string path, CancellationToken? cancellationToken = null);
    Task<List<FsArtifactActivity>> GetActivityHistoryAsync(string token, string path, long? page = null, long? pageSize = null, CancellationToken? cancellationToken = null);

    Task<string> GetLinkForShareAsync(string token, string path, CancellationToken? cancellationToken = null);
}
