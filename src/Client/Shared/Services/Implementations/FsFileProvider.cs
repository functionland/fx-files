using Functionland.FxFiles.Client.Shared.Services.Implementations.FsFileInfo;
using Functionland.FxFiles.Client.Shared.Services.Implementations.FxFileInfo;
using Functionland.FxFiles.Client.Shared.Utils;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

using System.Net;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FsFileProvider : IFileProvider
{
    private readonly IFileProvider _fileProvider;
    private FsFileProviderDependency _fsFileProviderDependency;


    public FsFileProvider(IFileProvider fileProvider, FsFileProviderDependency fsFileProviderDependency)
    {
        _fileProvider = fileProvider;
        _fsFileProviderDependency = fsFileProviderDependency;
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
                PathProtocol.Storage                => new PreviewFileInfo<ILocalDeviceFileService>(PreparePath(address), _fsFileProviderDependency.LocalDeviceFileService),
                PathProtocol.Fula                   => new PreviewFileInfo<IFulaFileService>(PreparePath(address), _fsFileProviderDependency.FulaFileService),
                PathProtocol.ThumbnailStorageSmall  => new ThumbFileInfo<ILocalDeviceFileService>(PreparePath(address),ThumbnailScale.Small, _fsFileProviderDependency.LocalArtifactThumbnailService, _fsFileProviderDependency.LocalDeviceFileService),
                PathProtocol.ThumbnailStorageMedium => new ThumbFileInfo<ILocalDeviceFileService>(PreparePath(address), ThumbnailScale.Medium, _fsFileProviderDependency.LocalArtifactThumbnailService, _fsFileProviderDependency.LocalDeviceFileService),
                PathProtocol.ThumbnailFulaSmall     => new ThumbFileInfo<IFulaFileService>(PreparePath(address), ThumbnailScale.Small, _fsFileProviderDependency.FulaArtifactThumbnailService, _fsFileProviderDependency.FulaFileService),
                PathProtocol.ThumbnailFulaMedium    => new ThumbFileInfo<IFulaFileService>(PreparePath(address), ThumbnailScale.Medium, _fsFileProviderDependency.FulaArtifactThumbnailService, _fsFileProviderDependency.FulaFileService),
                PathProtocol.Wwwroot                => _fileProvider.GetFileInfo(PreparePath("_content/Functionland.FxFiles.Client.Shared/" + address)),
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
        path = Uri.UnescapeDataString(path);
        return path;
    }
}




