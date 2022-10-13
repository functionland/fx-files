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
    
    /// <summary>
    /// Add new folder
    /// </summary>
    /// <param name="token"></param>
    /// <param name="path"></param>
    /// <param name="folderName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddFolderAsync(string token, string path, string folderName, CancellationToken? cancellationToken = null);
    
    /// <summary>
    /// Get a file stream
    /// </summary>
    /// <param name="token"></param>
    /// <param name="filePath"></param>
    /// <param name="onProgress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Stream> GetFileStreamAsync(string token, string filePath, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null);
    
    /// <summary>
    /// move some artifacts to another place.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="sourcePaths"></param>
    /// <param name="destinationPath"></param>
    /// <param name="overwrite">
    /// If is true then replace new artifact and if it's false skipe moving this artifact
    /// </param>
    /// <param name="onProgress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The failed moved artifacts</returns>
    Task<List<string>> MoveArtifactsAsync(string token, IEnumerable<string> sourcePaths, string destinationPath, bool overwrite = false, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null);


    /// <summary>
    /// copy some artifacts to another place.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="sourcePaths"></param>
    /// <param name="destinationPath"></param>
    /// <param name="overwrite">
    /// If is true then replace new artifact and if it's false skipe moving this artifact
    /// </param>
    /// <param name="onProgress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The failed copied artifacts</returns>
    Task<List<string>> CopyArtifactsAsync(string token, IEnumerable<string> sourcePaths, string destinationPath, bool overwrite = false, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null);
   
    /// <summary>
    /// Rename a file
    /// </summary>
    /// <param name="token"></param>
    /// <param name="filePath"></param>
    /// <param name="newName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RenameFileAsync(string token, string filePath, string newName, CancellationToken? cancellationToken = null);
    
    /// <summary>
    /// Rename a folder
    /// </summary>
    /// <param name="token"></param>
    /// <param name="folderPath"></param>
    /// <param name="newName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RenameFolderAsync(string token, string folderPath, string newName, CancellationToken? cancellationToken = null);
    
    /// <summary>
    /// Delete some artifacts
    /// </summary>
    /// <param name="token"></param>
    /// <param name="sourcesPath"></param>
    /// <param name="onProgress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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
    /// Get first information of an artifact.
    /// </summary>
    /// <remarks>ToDo: Specifiy the required fields to return.</remarks>
    /// <param name="token"></param>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FsArtifact> GetArtifactAsync(string token, string? path = null, CancellationToken? cancellationToken = null);

  
    /// <summary>
    /// Get all artifacts that I share.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<FsArtifact> GetSharedByMeArtifacsAsync(string token, CancellationToken? cancellationToken = null);
    
    /// <summary>
    /// Get all artifacts that have been shared with me.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<FsArtifact> GetSharedWithMeArtifacsAsync(string token, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Get meta of an artifac.
    /// </summary>
    /// <remarks>Specify the retuning fields</remarks>
    /// <param name="token"></param>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FsArtifact> GetArtifactMetaAsync(string token, string path, CancellationToken? cancellationToken = null);
    
    /// <summary>
    /// Get activity history of an artifact
    /// </summary>
    /// <param name="token"></param>
    /// <param name="path"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<FsArtifactActivity>> GetActivityHistoryAsync(string token, string path, long? page = null, long? pageSize = null, CancellationToken? cancellationToken = null);

    
    /// <summary>
    /// Get an artifact link for share.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GetLinkForShareAsync(string token, string path, CancellationToken? cancellationToken = null);
}
