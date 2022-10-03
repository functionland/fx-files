namespace Functionland.FxFiles.Client.Shared.Utils;

public static class ImageUtils
{
    public static (int width, int height) ScaleImage(int width, int height, int targetWidth, int targetHeight)
    {
        var imageWidth = width / 2;
        var imageHeight = height / 2;

        if (imageWidth <= targetWidth)
        {
            imageWidth = width;
        }

        if (imageHeight <= targetHeight)
        {
            imageHeight = height;
        }

        return (imageWidth, imageHeight);
    }
}
