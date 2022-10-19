using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

public class ImageThumbnailPluginPlatformTest : ThumbnailPluginPlatformTest
{
    public override string Title => "ImageThumbnailPluginTest";

    public override string Description => "Test image thumbnail ";

    IThumbnailPlugin ThumbnailPlugin { get; set; } = default!;

    public ImageThumbnailPluginPlatformTest(IEnumerable<IThumbnailPlugin> thumbnailPlugins)
    {
        ThumbnailPlugin = thumbnailPlugins.FirstOrDefault(t => t.IsExtensionSupported("jpg"));
    }

    protected override IThumbnailPlugin OnGetThumbnailPlugin()
    {
        return ThumbnailPlugin;
    }
}
