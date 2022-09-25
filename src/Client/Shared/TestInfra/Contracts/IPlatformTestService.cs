using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.Client.Shared.TestInfra.Contracts
{
    public interface IPlatformTestService
    {
        event EventHandler<TestProgressChangedEventArgs>? TestProgressChanged;

        IEnumerable<IPlatformTest> GetTests();
        Task RunTestAsync(IPlatformTest platformTest);
    }
}
