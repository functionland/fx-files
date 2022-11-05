using Functionland.FxFiles.Client.Shared.Components.Modal;

namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IZipService
{
    /// <summary>
    /// View a zip file items
    /// </summary>
    /// <param name="zipFilePath">
    /// C:\temp\file.zip
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<FsArtifact>> GetAllArtifactsAsync(
        string zipFilePath,
        string? password = null,
        CancellationToken? cancellationToken = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="zipFullPath"></param>
    /// <param name="destinationPath"></param>
    /// <param name="destinationFolderName"></param>
    /// <param name="itemPath"></param>
    /// <param name="overwrite"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Count of duplicated items</returns>
    Task<int> ExtractZippedArtifactAsync(
        string zipFullPath,
        string destinationPath,
        string destinationFolderName,
        IEnumerable<FsArtifact>? fsArtifacts = null,
        bool overwrite = false,
        string? password = null,
        Func<ProgressInfo, Task>? onProgress = null,
        CancellationToken? cancellationToken = null);
}
