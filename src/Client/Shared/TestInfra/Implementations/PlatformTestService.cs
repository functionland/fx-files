using Functionland.FxFiles.Client.Shared.TestInfra.Contracts;

namespace Functionland.FxFiles.Client.Shared.TestInfra.Implementations
{
    public abstract partial class PlatformTestService : IPlatformTestService
    {

        protected virtual List<IPlatformTest> OnGetTests()
        {
            return new List<IPlatformTest>()
            {
            };
        }

        public IEnumerable<IPlatformTest> GetTests()
        {
            return OnGetTests();
        }

        public async Task RunTestAsync(IPlatformTest platformTest)
        {
            try
            {
                platformTest.ProgressChanged += OnTestProgressChanged;
                await platformTest.RunAsync();
            }
            finally
            {
                platformTest.ProgressChanged -= OnTestProgressChanged;
            }
        }

        public void OnTestProgressChanged(object? sender, TestProgressChangedEventArgs eventArgs)
        {
            TestProgressChanged?.Invoke(sender, eventArgs);
        }

        public event EventHandler<TestProgressChangedEventArgs>? TestProgressChanged;
    }


}
