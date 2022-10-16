namespace Functionland.FxFiles.Client.Shared.Utils;

public static class FulaUserUtils
{
    public static string? GetFulaDId(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;

        var tokenParts = token.Split(',');

        return tokenParts[0];
    }

    public static string CreateToken(string dId, string securityKey) => $"{dId},{securityKey}";
}
