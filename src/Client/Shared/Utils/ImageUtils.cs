namespace Functionland.FxFiles.Client.Shared.Utils;

public static class ImageUtils
{
    public static (int width, int height) ScaleImage(int imageWidth, int imageHeight, ThumbnailScale thumbnailScale)
    {
        int thumbWidth;
        int thumbHeight;

        var (scaleWidth, scaleHeight) = GetHeightAndWidthFromThumbnailScale(thumbnailScale);

        if (scaleWidth >= imageWidth && scaleHeight >= imageHeight)
        {
            thumbWidth = imageWidth;
            thumbHeight = imageHeight;
        }
        else if (scaleWidth >= imageWidth && scaleHeight <= imageHeight)
        {
            thumbWidth = imageWidth;
            var ratio = CalculateRatio(imageWidth, imageHeight, scaleWidth, scaleHeight);
            thumbHeight = (int)Math.Round(imageHeight * ratio);
        }
        else if (scaleHeight >= imageHeight && scaleWidth <= imageWidth)
        {
            thumbHeight = imageHeight;
            var ratio = CalculateRatio(imageWidth, imageHeight, scaleWidth, scaleHeight);
            thumbWidth = (int)Math.Round(imageWidth * ratio);
        }
        else
        {
            var ratio = CalculateRatio(imageWidth, imageHeight, scaleWidth, scaleHeight);
            thumbWidth = (int)Math.Round(imageWidth * ratio);
            thumbHeight = (int)Math.Round(imageHeight * ratio);
        }

        return (thumbWidth, thumbHeight);
    }

    private static (int width, int height) GetHeightAndWidthFromThumbnailScale(ThumbnailScale thumbnailScale)
    {
        int width = 0;
        int height = 0;

        if (thumbnailScale == ThumbnailScale.XXSmall)
        {
            width = 50;
            height = 50;
        }
        else if (thumbnailScale == ThumbnailScale.XSmall)
        {
            width = 175;
            height = 90;
        }
        else if(thumbnailScale == ThumbnailScale.Small)
        {
            width = 260;
            height = 150;
        }
        else if (thumbnailScale == ThumbnailScale.Medium)
        {
            width = 380;
            height = 260;
        }

        return (width, height);
    }

    private static float CalculateRatio(int orginalWidth, int orgianlHeight, int targetWidth, int targetHeight)
    {
        float originalAspectRatio = (float)orginalWidth / (float)orgianlHeight;
        float targetAspectRatio = (float)targetWidth / (float)targetHeight;
        float scalingRatio;

        if (targetAspectRatio >= originalAspectRatio)
        {
            scalingRatio = (float)targetWidth / (float)orginalWidth;
        }
        else
        {
            scalingRatio = (float)targetHeight / (float)orgianlHeight;
        }

        return scalingRatio;
    }
}
