using Functionland.FxFiles.Client.Shared.Services.Implementations.FxFileInfo;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

using System.Net;
using System.Text.RegularExpressions;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FxFileProvider : IFileProvider
{
    private readonly IFileProvider _fileProvider;
    private ILocalDeviceFileService _localDeviceFileService;
    private IFulaFileService _fulaFileService;


    public FxFileProvider(IFileProvider fileProvider, ILocalDeviceFileService localDeviceFileService, IFulaFileService fulaFileService)
    {
        _fileProvider = fileProvider;
        _localDeviceFileService = localDeviceFileService;
        _fulaFileService = fulaFileService;
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        return _fileProvider.GetDirectoryContents(subpath);
    }

    Regex pathRegex = new Regex(@"^\*(?<protocol>\w+)\*\/(?<address>.*)");
    public IFileInfo GetFileInfo(string subpath)
    {
        var match = pathRegex.Match(subpath);

        if (match.Success)
        {
            var protocol = match.Groups["protocol"].Value;
            var address = match.Groups["address"].Value;

            return protocol switch
            {
                "storage" => new PreviewFileInfo(PreparePath(address), _localDeviceFileService),
                "fula" => new PreviewFileInfo(PreparePath(address), _fulaFileService),
                "wwwroot" => _fileProvider.GetFileInfo(PreparePath(Regex.Replace(subpath, @"^\*(?<protocol>\w+)\*\/", "_content/Functionland.FxFiles.Client.Shared"))),
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




