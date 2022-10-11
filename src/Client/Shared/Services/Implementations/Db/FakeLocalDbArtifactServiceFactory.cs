using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations
{
    public class FakeLocalDbArtifactServiceFactory
    {
        public FakeLocalDbArtifactService CreateSyncScenario01()
        {
            return new FakeLocalDbArtifactService(null);
        }

        public FakeLocalDbArtifactService CreateSyncScenario02()
        {
            return new FakeLocalDbArtifactService(null);
        }
    }
}
