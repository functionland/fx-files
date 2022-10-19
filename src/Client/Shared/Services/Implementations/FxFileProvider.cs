using Functionland.FxFiles.Client.Shared.Services.Implementations.FxFileInfo;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

using System.Net;
using System.Text.RegularExpressions;

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

    Regex pathRegex = new Regex(@"^\*(?<protocol>\w+)\*(?<address>.*)");

    enum Protocols { Fula, Storage, Thumb}

    public IFileInfo GetFileInfo(string subpath)
    {
        var match = pathRegex.Match(subpath);

        if (match.Success)
        {
            var protocol = match.Groups["protocol"].Value;
            var address = match.Groups["address"].Value;

            return protocol switch
            {
                "storage" => new StorageFileInfo(PreparePath(subpath)),
                "fula" => new FulaFileInfo(PreparePath(subpath)),
                "wwwroot" => new FulaFileInfo(PreparePath(Regex.Replace(subpath, @"^\*(?<protocol>\w+)\*", "_content/Functionland.FxFiles.Client.Shared"))),
                _ => throw new InvalidOperationException($"Protocol not supported: {protocol}")
            };
        }

        //    if (subpath.StartsWith("_content/Functionland.FxFiles.Client.Shared/fula://"))
        //{
        //    return new FulaFileInfo(PreparePath(subpath));
        //}
        //else if (subpath.StartsWith("_content/Functionland.FxFiles.Client.Shared/storage://"))
        //{
        //    return new StorageFileInfo(PreparePath(subpath));
        //}

        return _fileProvider.GetFileInfo(subpath);
    }

    public IChangeToken Watch(string filter)
    {
        return _fileProvider.Watch(filter);
    }

    private string PreparePath(string path)
    {
        path = path.Replace("_content/Functionland.FxFiles.Client.Shared/", string.Empty);
        path = path.Replace("fula://", string.Empty);
        path = path.Replace("storage://", string.Empty);
        path = WebUtility.UrlDecode(path);
        return path;
    }
}




