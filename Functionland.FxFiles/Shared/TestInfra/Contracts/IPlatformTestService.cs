using Functionland.FxFiles.Shared.TestInfra.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.TestInfra.Contracts
{
    public interface IPlatformTestService
    {
        event EventHandler<TestProgressChangedEventArgs>? TestProgressChanged;

        IEnumerable<IPlatformTest> GetTests();
        Task RunTestAsync(IPlatformTest platformTest);
    }
}
