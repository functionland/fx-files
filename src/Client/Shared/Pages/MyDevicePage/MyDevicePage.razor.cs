using Functionland.FxFiles.Client.Shared.Components.Modal;

namespace Functionland.FxFiles.Client.Shared.Pages
{
    public partial class MyDevicePage
    {
        private ArtifactSelectionModal _artifactSelectionModalRef = default!;

        [AutoInject] private ILocalDeviceFileService _fileService { get; set; } = default!;

        [AutoInject] private ILocalDevicePinService _pinService { get; set; } = default!;

        [AutoInject] private IArtifactThumbnailService<ILocalDeviceFileService> _thumbnailService { get; set; } = default!;
        private string? DecodedDefaultPath
        {
            get
            {
                var query = new Uri(NavigationManager.Uri).Query;
                if (string.IsNullOrWhiteSpace(query)) return null;

                var decodedQuery = Uri.UnescapeDataString(query);
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

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (AppStateStore.CurrentPagePath.ToLower().Equals("mydevice"))
            {
                AppStateStore.CurrentMyDeviceArtifact = null;
            }

            AppStateStore.CurrentPagePath = "mydevice";
        }
    }
}