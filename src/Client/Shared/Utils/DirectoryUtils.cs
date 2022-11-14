namespace Functionland.FxFiles.Client.Shared.Utils;

public static class DirectoryUtils
{
    public static void HardDeleteDirectory(string path)
    {
        try
        {
            Directory.Delete(path, true);
        }
        catch (Exception)
        {
            DepthFirstDeleteDirectory(path);
        }
    }

    /// <summary>
    /// Depth-first recursive delete, with handling for descendant 
    /// directories open in Windows Explorer.
    /// </summary>
    private static void DepthFirstDeleteDirectory(string path)
    {
        foreach (string directory in Directory.GetDirectories(path))
        {
            HardDeleteDirectory(directory);
        }

        try
        {
            Directory.Delete(path, true);
        }
        catch (IOException)
        {
            Directory.Delete(path, true);
        }
        catch (UnauthorizedAccessException)
        {
            Directory.Delete(path, true);
        }
    }
}
