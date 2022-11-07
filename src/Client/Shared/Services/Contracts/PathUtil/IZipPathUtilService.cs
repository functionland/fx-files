namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IZipPathUtilService
{
    string GetZipEntryPath(string itemPath);

    string GetRarEntryPath(string itemPath);
}
