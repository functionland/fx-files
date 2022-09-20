using Functionland.FxFiles.Shared.Models;
using Functionland.FxFiles.Shared.Utils;
using android = Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Graphics;

namespace Functionland.FxFiles.App.Platforms.Android.Implementations
{
    public class AndroidThumbnailService : LocalThumbnailService
    {
        public override Task<string> MakeThumbnailAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }
    }
}
