using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Services.FulaClient.Implementation
{
    public class FakeFulaFileClientFactory
    {
        public FakeFulaFileClient CreateSyncScenario01()
        {
            return new FakeFulaFileClient(null);
        }

        public FakeFulaFileClient CreateSyncScenario02()
        {
            return new FakeFulaFileClient(null);
        }
    }
}
