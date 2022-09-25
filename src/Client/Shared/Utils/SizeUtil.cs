namespace Functionland.FxFiles.Client.Shared.Utils
{
    public static class SizeUtil
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
                (>= OneKB) and (< OneMB) => $"{size / OneKB} KB",
                (>= OneMB) and (< OneGB) => $"{size / OneMB} MB",
                (>= OneGB) and (< OneTB) => $"{size / OneMB} GB",
                (>= OneTB) => $"{size / OneTB}"
            };
        }
    }
}
