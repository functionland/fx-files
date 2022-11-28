namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FxBreadcrumbs
    {
        private string[] _breadcrumbs = new string[0];

        [Parameter] public string Source { get; set; } = "browsing";
        [Parameter] public string[] BreadcrumbsPath { get => _breadcrumbs; 
            set {
                //if (_breadcrumbs == value) return;
                _breadcrumbs = value;
                JSRuntime.InvokeVoidAsync("breadCrumbStyle", Source);
            }
        }

    }
}
