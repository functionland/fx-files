
namespace Functionland.FxFiles.Client.Shared.Extensions;

public static class TextExtensions
{
    public static string ToLowerFirstChar(this string item)
    {
        if (string.IsNullOrWhiteSpace(item)) return "artifact";

        var result = char.ToLower(item[0]) + item[1..];

        return result;
    }
}
