[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Functionland.FxFiles.App;

public partial class App
{
    public App()
    {
        AppDomain.CurrentDomain.FirstChanceException += (sender, error) =>
        {
            System.Diagnostics.Debug.WriteLine($"********************************** UNHANDLED EXCEPTION! Details: {error}");
        };
        InitializeComponent();
    }
}
