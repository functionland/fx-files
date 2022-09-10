namespace Functionland.FxFiles.Shared.TestInfra.Implementations
{
    public class TestProgressChangedEventArgs : EventArgs
    {
        public TestProgressChangedEventArgs(string title, string description, TestProgressType progressType)
        {
            Title = title;
            Description = description;
            ProgressType = progressType;
        }
        public string Title { get; set; }
        public string Description { get; set; }
        public TestProgressType ProgressType { get; set; }
    }
}
