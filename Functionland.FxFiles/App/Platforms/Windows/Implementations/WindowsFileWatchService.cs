using Microsoft.Extensions.FileSystemGlobbing;
using Prism.Events;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Functionland.FxFiles.App.Platforms.Windows.Implementations
{
    public partial class WindowsFileWatchService : FileWatchService
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
