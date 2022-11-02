namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IZipService
{
    /// <summary>
    /// View a zip file items
    /// </summary>
    /// <param name="zipFilePath">
    /// C:\temp\file.zip
    /// </param>
    /// <param name="subDirectoriesPath">
    /// Send file.zip for view root path items
    /// Send file.zip\folder1 for view all item inside file.zip\folder1
    /// </param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<FsArtifact>> ViewZipFileAsync(string zipFilePath, string subDirectoriesPath, string? password = null, CancellationToken? cancellationToken = null);
    Task ExtractZippedArtifactAsync(string zipFullPath, string destinationPath, string destinationFolderName, string? itemPath = null, bool overwrite = false, string? password = null, CancellationToken? cancellationToken = null);
}
