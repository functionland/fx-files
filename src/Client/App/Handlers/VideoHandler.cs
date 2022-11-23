#if IOS || MACCATALYST
using PlatformView = Functionland.FxFiles.Client.App.Platforms.MaciOS.MauiVideoPlayer;
#elif ANDROID
using PlatformView = Functionland.FxFiles.Client.App.Platforms.Android.MauiVideoPlayer;
#elif WINDOWS
using PlatformView = Functionland.FxFiles.Client.App.Platforms.Windows.MauiVideoPlayer;
#elif (NETSTANDARD || !PLATFORM) || (NET6_0_OR_GREATER && !IOS && !ANDROID)
using PlatformView = System.Object;
#endif
using Functionland.FxFiles.Client.App.Controls;
using Microsoft.Maui.Handlers;

namespace Functionland.FxFiles.Client.App.Handlers
{
    public partial class VideoHandler
    {
        public static IPropertyMapper<Video, VideoHandler> PropertyMapper = new PropertyMapper<Video, VideoHandler>(ViewHandler.ViewMapper)
        {
            [nameof(Video.AreTransportControlsEnabled)] = MapAreTransportControlsEnabled,
            [nameof(Video.IsLooping)] = MapIsLooping,
            [nameof(Video.Position)] = MapPosition,
            [nameof(Video.Source)] = MapSource
        };

        public static CommandMapper<Video, VideoHandler> CommandMapper = new(ViewCommandMapper)
        {
            [nameof(Video.UpdateStatus)] = MapUpdateStatus,
            [nameof(Video.PlayRequested)] = MapPlayRequested,
            [nameof(Video.StopRequested)] = MapStopRequested,
            [nameof(Video.PauseRequested)] = MapPauseRequested
        };

        public VideoHandler() : base(PropertyMapper, CommandMapper)
        {
        }
    }
}
