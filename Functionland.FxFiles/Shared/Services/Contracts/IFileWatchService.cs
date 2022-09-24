using Functionland.FxFiles.Shared.TestInfra.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Services.Contracts
{
    public interface IFileWatchService
    {
        void WatchArtifact(FsArtifact fsArtifact);
        void UnWatchArtifact(FsArtifact fsArtifact);
    }

}
