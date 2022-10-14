using Functionland.FxFiles.Client.Shared.Services.Implementations.FxFileInfo;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

using System.Net;
namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FxFileProvider : IFileProvider
{
    private readonly IFileProvider _fileProvider;

    public FxFileProvider(IFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        return _fileProvider.GetDirectoryContents(subpath);
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        if (subpath.StartsWith("fula://"))
            return new FulaFileInfo(PreparePath(subpath));
        else if (subpath.StartsWith("storage://"))
            return new StorageFileInfo(PreparePath(subpath));

        return _fileProvider.GetFileInfo(subpath);
    }

    public IChangeToken Watch(string filter)
    {
        return _fileProvider.Watch(filter);
    }

    private string PreparePath(string path)
    {
        path = path.Replace("fula://", string.Empty);
        path = path.Replace("storage://", string.Empty);
        path = WebUtility.UrlDecode(path);
        return path;
    }
}




