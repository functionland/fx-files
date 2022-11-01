namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IZipService
{
    Task<List<FsArtifact>> ZipFileViewerAsync(string zipFilePath, string subDirectoriesPath, string? password = null, CancellationToken? cancellationToken = null);
    Task ExtractZippedArtifactAsync(string zipFullPath, string destinationPath, string itemPath, string? destinationFolderName = null, bool overwrite = false, string? password = null, CancellationToken? cancellationToken = null);
}
