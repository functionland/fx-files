namespace Functionland.FxFiles.Client.Shared.Models
{
    public static class FileCategory
    {
        public static IDictionary<string, FileCategoryType> FileExtentionsType = new Dictionary<string, FileCategoryType>()
        {
            {".jpg", FileCategoryType.Image},
            {".jpeg", FileCategoryType.Image},
            {".png", FileCategoryType.Image},
            { ".gif", FileCategoryType.Image},
            { ".bmp", FileCategoryType.Image},
            { ".tiff", FileCategoryType.Image},
            { ".tif", FileCategoryType.Image},
            { ".pdf", FileCategoryType.Pdf},
            { ".mp3", FileCategoryType.Audio},
            { ".wav", FileCategoryType.Audio},
            { ".wma", FileCategoryType.Audio},
            { ".aac", FileCategoryType.Audio},
            { ".ogg", FileCategoryType.Audio},
            { ".flac", FileCategoryType.Audio},
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
        };

        #region File category extinction method
        public static FileCategoryType GetCategoryType(string fileExtension)
        {
            if (FileExtentionsType.ContainsKey(fileExtension) && !string.IsNullOrEmpty(fileExtension) && !string.IsNullOrWhiteSpace(fileExtension))
            {
                return FileExtentionsType[fileExtension];
            }
            return FileCategoryType.Other;
        }
        #endregion
    }
}
