using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

public partial class ImageThumbnailPluginPlatformTest : PlatformTest
{
    public override string Title => "ImageThumbnailPluginTest";

    public override string Description => "Test image thumbnail ";

    [AutoInject] IEnumerable<IThumbnailPlugin> ThumbnailPlugins { get; set; } = default!;

    [AutoInject] ILocalDeviceFileService LocalDeviceFileService { get; set; } = default!;

    protected override Task OnRunAsync()
    {
        throw new NotImplementedException();
    }
}
