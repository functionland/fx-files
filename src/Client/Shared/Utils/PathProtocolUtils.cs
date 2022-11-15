using System.Text.RegularExpressions;

namespace Functionland.FxFiles.Client.Shared.Utils;

public static class PathProtocolUtils
{
    public static Regex PathRegex { get; set; } = new Regex(@"^(./)?\*(?<protocol>\w+)\*(?<address>.*)");

    public static string GetPathProtocol(PathProtocol protocol)
    {
        return protocol switch
        {
            PathProtocol.Storage                => nameof(PathProtocol.Storage),
            PathProtocol.Fula                   => nameof(PathProtocol.Fula),
            PathProtocol.Wwwroot                => nameof(PathProtocol.Wwwroot),
            PathProtocol.ThumbnailStorageSmall  => nameof(PathProtocol.ThumbnailStorageSmall),
            PathProtocol.ThumbnailStorageMedium => nameof(PathProtocol.ThumbnailStorageMedium),
            PathProtocol.ThumbnailFulaSmall     => nameof(PathProtocol.ThumbnailFulaSmall),
            PathProtocol.ThumbnailFulaMedium    => nameof(PathProtocol.ThumbnailFulaMedium),
            _ => throw new ArgumentOutOfRangeException(nameof(protocol), protocol, null)
        };
    }

    public static string InProtocol(this string address, PathProtocol protocol)
    {
        var encodedAddress = Uri.EscapeDataString(address);
        return GetPath(protocol, encodedAddress);
    }

    public static PathProtocol GetPathProtocol(string protocol)
    {
        return protocol switch
        {
            nameof(PathProtocol.Storage)                => PathProtocol.Storage,
            nameof(PathProtocol.Fula)                   => PathProtocol.Fula,
            nameof(PathProtocol.Wwwroot)                => PathProtocol.Wwwroot,
            nameof(PathProtocol.ThumbnailStorageSmall)  => PathProtocol.ThumbnailStorageSmall,
            nameof(PathProtocol.ThumbnailStorageMedium) => PathProtocol.ThumbnailStorageMedium,
            nameof(PathProtocol.ThumbnailFulaSmall)     => PathProtocol.ThumbnailFulaSmall,
            nameof(PathProtocol.ThumbnailFulaMedium)    => PathProtocol.ThumbnailFulaMedium,
            _ => throw new ArgumentOutOfRangeException(nameof(protocol), protocol, null)
        };
    }

    public static string GetPath(PathProtocol protocol, string address)
    {
        var protocolStr = GetPathProtocol(protocol);
        return $"*{protocolStr}*{address}";
    }

    public static bool IsMatch(string path)
    {
        return PathRegex.IsMatch(path);
    }

    public static (bool, PathProtocol, string) Match(string path)
    {
        var match = PathRegex.Match(path);

        if (match.Success)
        {
            var protocolStr = match.Groups["protocol"].Value;
            var protocol = GetPathProtocol(protocolStr);

            var address = match.Groups["address"].Value;

            return (true, protocol, address);
        }

        return (false, PathProtocol.None, path);
    }
}


