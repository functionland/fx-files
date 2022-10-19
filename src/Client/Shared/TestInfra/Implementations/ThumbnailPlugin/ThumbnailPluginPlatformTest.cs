using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

public abstract class ThumbnailPluginPlatformTest : PlatformTest
{
    protected abstract IThumbnailPlugin OnGetThumbnailPlugin();
    
    protected async Task OnRunThumbnailPluginTestAsync(IThumbnailPlugin thumbnailPlugin)
    {
        throw new NotImplementedException();
    }

    protected override async Task OnRunAsync()
    {
        var thumbnailPlugin = OnGetThumbnailPlugin();
        await OnRunThumbnailPluginTestAsync(thumbnailPlugin);
    }
}
