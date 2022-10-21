using Functionland.FxFiles.Client.Shared.Services.Implementations.FxFileInfo;
using Functionland.FxFiles.Client.Shared.Utils;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

using System.Net;
using System.Text.RegularExpressions;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FsFileProvider : IFileProvider
{
    private readonly IFileProvider _fileProvider;
    private ILocalDeviceFileService _localDeviceFileService;
    private IFulaFileService _fulaFileService;


    public FsFileProvider(IFileProvider fileProvider, ILocalDeviceFileService localDeviceFileService, IFulaFileService fulaFileService)
    {
        _fileProvider = fileProvider;
        _localDeviceFileService = localDeviceFileService;
        _fulaFileService = fulaFileService;
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        return _fileProvider.GetDirectoryContents(subpath);
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        if (PathProtocolUtils.IsMatch(subpath))
        {
            var (_, protocol, address) = PathProtocolUtils.Match(subpath);

            return protocol switch
            {
                PathProtocol.Storage => new PreviewFileInfo(PreparePath(address), _localDeviceFileService),
                PathProtocol.Fula => new PreviewFileInfo(PreparePath(address), _fulaFileService),
                PathProtocol.Wwwroot => _fileProvider.GetFileInfo(PreparePath(Regex.Replace(subpath, @"^\*(?<protocol>\w+)\*\/", "_content/Functionland.FxFiles.Client.Shared"))),
                _ => throw new InvalidOperationException($"Protocol not supported: {protocol}")
            };
        }

        return _fileProvider.GetFileInfo(subpath);
    }

    public IChangeToken Watch(string filter)
    {
        return _fileProvider.Watch(filter);
    }

    private string PreparePath(string path)
    {
        path = WebUtility.UrlDecode(path);
        return path;
    }
}




