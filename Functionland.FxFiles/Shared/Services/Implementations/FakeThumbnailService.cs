using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Services.Implementations
{
    public class FakeThumbnailService : IThumbnailService
    {
        public async Task<string> MakeThumbnailAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null)
        {
            return "/Files/fake-pic.jpg";
        }
    }
}
