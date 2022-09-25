using Android.Gestures;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.App.Platforms.Android.Implementations
{
    public partial class AndroidFileWatchService: FileWatchService
    {
        public override void WatchArtifact(FsArtifact fsArtifact)
        {
            base.WatchArtifact(fsArtifact);
        }

        public override void UnWatchArtifact(FsArtifact fsArtifact)
        {
            base.UnWatchArtifact(fsArtifact);
        }
    }
}
