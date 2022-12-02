namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public static class LocalAndroidTxtLogger
{
    public static void Log(string message)
    {
#if Dev || Local
        message = $"[{DateTimeOffset.Now:yyyy/MM/dd HH:mm:ss}] --> [{message}{Environment.NewLine}]";
        var path = "/storage/emulated/0/FxFileLocalLogs.txt";
        if (!File.Exists(path))
        {
            File.WriteAllText(path, message);
        }
        else
        {
            var rawContent = File.ReadAllText(path);
            File.WriteAllText(path, $"{rawContent}{Environment.NewLine}{message}");
        } 
#endif
    }
}
