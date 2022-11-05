namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IPathUtilService
{
    string GetZipEntryPath(string itemPath);

    string GetRarEntryPath(string itemPath);
}
