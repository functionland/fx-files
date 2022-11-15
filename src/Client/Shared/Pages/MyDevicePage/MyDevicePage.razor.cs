using Functionland.FxFiles.Client.Shared.Components.Modal;

using Microsoft.AspNetCore.Components.Web;

namespace Functionland.FxFiles.Client.Shared.Pages
{
    public partial class MyDevicePage
    {
        private ArtifactSelectionModal _artifactSelectionModalRef = default!;

        [AutoInject] private ILocalDeviceFileService _fileService { get; set; } = default!;

        [AutoInject] private ILocalDevicePinService _pinService { get; set; } = default!;

        [AutoInject] private IArtifactThumbnailService<ILocalDeviceFileService> _thumbnailService { get; set; } = default!;
        [AutoInject] private IAppStateStore _appStateStore { get; set; } = default!;
        private string? DecodedDefaultPath
        {
            get
            {
                var query = new Uri(NavigationManager.Uri).Query;
                if (string.IsNullOrWhiteSpace(query)) return null;

                var decodedQuery = System.Net.WebUtility.UrlDecode(query);
                if (string.IsNullOrWhiteSpace(decodedQuery)) return null;

                var decodedQueryParts = decodedQuery.Split('&');
                if (decodedQueryParts.Length < 1) return null;

                foreach (var item in decodedQueryParts)
                {
                    var keyValue = item.Trim('?').ToString();

                    if (keyValue.StartsWith("encodedArtifactPath="))
                    {
                        return keyValue.Replace("encodedArtifactPath=", "");
                    }
                }

                return null;
            }
        }
        private bool test = false;
        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();
            test = true;
            await Task.Delay(5000);
            if (_appStateStore.CurrentPagePath.ToLower().Equals("mydevice"))
            {
                _appStateStore.CurrentMyDeviceArtifact = null;
            }

            _appStateStore.CurrentPagePath = "mydevice";
        }
    }
}