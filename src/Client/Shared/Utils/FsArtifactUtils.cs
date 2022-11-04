using Functionland.FxFiles.Client.Shared.Components.Modal;

namespace Functionland.FxFiles.Client.Shared.Utils;

public static class FsArtifactUtils
{

    public const long OneKB = 1024;

    public const long OneMB = OneKB * OneKB;

    public const long OneGB = OneMB * OneKB;

    public const long OneTB = OneGB * OneKB;

    public static string CalculateSizeStr(long? size)
    {
        if (size == null) return "0 bytes";

        return size switch
        {
            (< OneKB) => $"{size} bytes",
            (>= OneKB) and (< OneMB) => $"{string.Format("{0:F2}", (float)size / OneKB)} KB",
            (>= OneMB) and (< OneGB) => $"{string.Format("{0:F2}", (float)size / OneMB)} MB",
            (>= OneGB) and (< OneTB) => $"{string.Format("{0:F2}", (float)size / OneMB)} GB",
            (>= OneTB) => $"{string.Format("{0:F2}", (float)size / OneTB)} TB"
        };
    }
    public static long ConvertToByte(string sizeValueStr, string sizeUnit)
    {

        if (long.TryParse(sizeValueStr, out var sizeValue))
        {
            if (sizeUnit == "kb")
                sizeValue *= OneKB;
            if (sizeUnit == "mb")
                sizeValue *= OneMB;
            if (sizeUnit == "gb")
                sizeValue *= OneGB;
            if (sizeUnit == "tb")
                sizeValue *= OneTB;
        }

        return sizeValue;
    }

    public static IDictionary<string, FileCategoryType> FileExtentionsType = new Dictionary<string, FileCategoryType>()
    {
        //Image
        {".jpg", FileCategoryType.Image},
        {".jpeg", FileCategoryType.Image},
        {".png", FileCategoryType.Image},
        { ".gif", FileCategoryType.Image},
        { ".bmp", FileCategoryType.Image},
        { ".tiff", FileCategoryType.Image},
        { ".tif", FileCategoryType.Image},
        { ".jfif", FileCategoryType.Image},
        { ".exif", FileCategoryType.Image},
        { ".ppm", FileCategoryType.Image},
        { ".pgm", FileCategoryType.Image},
        { ".pbm", FileCategoryType.Image},
        { ".pnm", FileCategoryType.Image},
        { ".webp", FileCategoryType.Image},
        { ".heif", FileCategoryType.Image},
        { ".avif", FileCategoryType.Image},
        { ".ico", FileCategoryType.Image},
        { ".svg", FileCategoryType.Image},
        
        //PDF
        { ".pdf", FileCategoryType.Pdf},

        //Audio
        { ".mp3", FileCategoryType.Audio},
        { ".wav", FileCategoryType.Audio},
        { ".wma", FileCategoryType.Audio},
        { ".aac", FileCategoryType.Audio},
        { ".ogg", FileCategoryType.Audio},
        { ".flac", FileCategoryType.Audio},
        { ".aa", FileCategoryType.Audio},
        { ".aax", FileCategoryType.Audio},
        { ".act", FileCategoryType.Audio},
        { ".aiff", FileCategoryType.Audio},
        { ".alac", FileCategoryType.Audio},
        { ".amr", FileCategoryType.Audio},
        { ".ape", FileCategoryType.Audio},
        { ".au", FileCategoryType.Audio},
        { ".awb", FileCategoryType.Audio},
        { ".dss", FileCategoryType.Audio},
        { ".dvf", FileCategoryType.Audio},
        { ".gsm", FileCategoryType.Audio},
        { ".iklax", FileCategoryType.Audio},
        { ".ivs", FileCategoryType.Audio},
        { ".m4a", FileCategoryType.Audio},
        { ".m4b", FileCategoryType.Audio},
        { ".m4p", FileCategoryType.Audio},
        { ".mmf", FileCategoryType.Audio},
        { ".mpc", FileCategoryType.Audio},
        { ".msv", FileCategoryType.Audio},
        { ".nmf", FileCategoryType.Audio},
        { ".oga", FileCategoryType.Audio},
        { ".mogg", FileCategoryType.Audio},
        { ".opus", FileCategoryType.Audio},
        { ".ra", FileCategoryType.Audio},
        { ".raw", FileCategoryType.Audio},
        { ".rf64", FileCategoryType.Audio},
        { ".voc", FileCategoryType.Audio},
        { ".vox", FileCategoryType.Audio},
        { ".wv", FileCategoryType.Audio},
        { ".8svx", FileCategoryType.Audio},
        { ".cda", FileCategoryType.Audio},
                  
        //Video
        { ".mp4", FileCategoryType.Video},
        { ".avi", FileCategoryType.Video},
        { ".wmv", FileCategoryType.Video},
        { ".mov", FileCategoryType.Video},
        { ".mkv", FileCategoryType.Video},
        { ".flv", FileCategoryType.Video},
        { ".swf", FileCategoryType.Video},
        { ".mpg", FileCategoryType.Video},
        { ".mpeg", FileCategoryType.Video},
        { ".m4v", FileCategoryType.Video},
        { ".3gp", FileCategoryType.Video},
        { ".3gpp", FileCategoryType.Video},
        { ".3g2", FileCategoryType.Video},
        { ".3gpp2", FileCategoryType.Video},
        { ".webm", FileCategoryType.Video},
        { ".ts", FileCategoryType.Video},
        { ".mts", FileCategoryType.Video},
        { ".m2ts", FileCategoryType.Video},
        { ".vob", FileCategoryType.Video},
        { ".ogv", FileCategoryType.Video},
        { ".mxf", FileCategoryType.Video},
        { ".roq", FileCategoryType.Video},
        { ".nsv", FileCategoryType.Video},
        { ".f4v", FileCategoryType.Video},
        { ".f4p", FileCategoryType.Video},
        { ".f4a", FileCategoryType.Video},
        { ".f4b", FileCategoryType.Video},
        { ".drc", FileCategoryType.Video},
        { ".gifv", FileCategoryType.Video},
        { ".mng", FileCategoryType.Video},
        { ".qt", FileCategoryType.Video},
        { ".yuv", FileCategoryType.Video},
        { ".rm", FileCategoryType.Video},
        { ".rmvb", FileCategoryType.Video},
        { ".viv", FileCategoryType.Video},
        { ".asf", FileCategoryType.Video},
        { ".amv", FileCategoryType.Video},
        { ".mp2", FileCategoryType.Video},
        { ".mpe", FileCategoryType.Video},
        { ".mpv", FileCategoryType.Video},

        //APP
        { ".exe", FileCategoryType.App},
        { ".msi", FileCategoryType.App},
        { ".msix", FileCategoryType.App},
        { ".apk", FileCategoryType.App},
        { ".jar", FileCategoryType.App},
        { ".apkm", FileCategoryType.App},
        { ".apks", FileCategoryType.App},
        { ".xapk", FileCategoryType.App},

        //Documents
        { ".doc", FileCategoryType.Document},
        { ".docx", FileCategoryType.Document},
        { ".html", FileCategoryType.Document},
        { ".htm", FileCategoryType.Document},
        { ".odt", FileCategoryType.Document},
        { ".xls", FileCategoryType.Document},
        { ".xlsx", FileCategoryType.Document},
        { ".ods", FileCategoryType.Document},
        { ".tpt", FileCategoryType.Document},
        { ".tptx", FileCategoryType.Document},
        { ".txt", FileCategoryType.Document},

        //Zip
        {".zip",FileCategoryType.Zip },
        {".rar",FileCategoryType.Zip }
    };

    public static FileCategoryType GetCategoryType(string fileExtension)
    {
        if (FileExtentionsType.ContainsKey(fileExtension) &&
            !string.IsNullOrEmpty(fileExtension) &&
            !string.IsNullOrWhiteSpace(fileExtension))
        {
            return FileExtentionsType[fileExtension];
        }
        return FileCategoryType.Other;
    }

    public static List<string> GetSearchCategoryTypeExtentions(ArtifactCategorySearchType searchType)
    {
        return searchType switch
        {
            ArtifactCategorySearchType.Image =>
                FileExtentionsType.Where(f => f.Value == FileCategoryType.Image).Select(f => f.Key.ToLower()).ToList(),
            ArtifactCategorySearchType.App =>
                FileExtentionsType.Where(f => f.Value == FileCategoryType.App).Select(f => f.Key.ToLower()).ToList(),
            ArtifactCategorySearchType.Audio =>
                FileExtentionsType.Where(f => f.Value == FileCategoryType.Audio).Select(f => f.Key.ToLower()).ToList(),
            ArtifactCategorySearchType.Document =>
                FileExtentionsType.Where(f => f.Value == FileCategoryType.Document || f.Value == FileCategoryType.Pdf).Select(f => f.Key.ToLower()).ToList(),
            ArtifactCategorySearchType.Video =>
                FileExtentionsType.Where(f => f.Value == FileCategoryType.Video).Select(f => f.Key.ToLower()).ToList(),
            _ => new List<string>()
        };
    }

    public static async Task<int> HandleProgressBarAsync(string artifactName, int totalCount, int? progressCount, Func<ProgressInfo, Task> onProgress)
    {
        if (progressCount is null)
        {
            progressCount = 0;
        }
        else
        {
            progressCount++;
        }

        var subText = $"{progressCount} of {totalCount}";

        await onProgress.Invoke(new ProgressInfo
        {
            CurrentText = artifactName,
            CurrentSubText = subText,
            CurrentValue = progressCount,
            MaxValue = totalCount
        });

        return progressCount.Value;
    }
}
