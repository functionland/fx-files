using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations
{
    public class FakeLocalDbArtifactService : ILocalDbArtifactService
    {
        public FakeLocalDbArtifactService(List<FsArtifact> artifacts)
        {

        }

        public Task<FsArtifact> GetArtifactAsync(string localPath)
        {
            throw new NotImplementedException();
        }

        public Task<List<FsArtifact>> GetChildrenArtifactsAsync(string localPath)
        {
            throw new NotImplementedException();
        }
    }
}
