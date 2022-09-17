namespace Functionland.FxFiles.App.Components.Common
{
    public class FileCardConfig
    {
        public FileCardConfig(bool isFavorite, bool isDisabled, bool isPressed, string fileName, string fileFormat, string modifiedDate, string fileSize)
        {
            IsFavorite = isFavorite;
            IsDisable = isDisabled;
            IsPressed = isPressed;
            FileName = fileName;
            ModifiedDate = modifiedDate;
            FileSize = fileSize;
        }
        public bool IsFavorite { get; set; }
        public bool IsDisable { get; set; }
        public bool IsPressed { get; set; }
        public string? FileName { get; set; }
        public string? FileFormat { get; set; }
        public string? ModifiedDate { get; set; }
        public string? FileSize { get; set; }
    }
}
