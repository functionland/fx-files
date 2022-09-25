using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.Client.Shared.TestInfra.Contracts
{
    public interface IPlatformTest
    {
        string Title { get; }
        string Description { get; }
        Task RunAsync();
        public event EventHandler<TestProgressChangedEventArgs> ProgressChanged;
    }
}
