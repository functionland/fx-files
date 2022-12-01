using System.Diagnostics;
using System.IO;
using Microsoft.AppCenter.Crashes;
namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public partial class ExceptionHandler : IExceptionHandler
{
    [AutoInject] IStringLocalizer<AppStrings> _localizer = default!;

    public void Handle(Exception exception, IDictionary<string, string>? parameters = null)
    {
#if DEBUG
        var title = _localizer.GetString(AppStrings.ToastErrorTitle);
        var message = (exception as KnownException)?.Message ?? exception.ToString(); ;
        FxToast.Show(title, message, FxToastType.Error);
        Console.WriteLine(message);
        Debugger.Break();
#else
        if (exception is KnownException or FileNotFoundException or IOException)
        {
            var title = _localizer.GetString(AppStrings.ToastErrorTitle);
            var message = exception.Message;
            FxToast.Show(title, message, FxToastType.Error);
        }
        else
        {
            if (DeviceInfo.Current.Platform != DevicePlatform.macOS && DeviceInfo.Current.Platform != DevicePlatform.MacCatalyst)
            {
                Crashes.TrackError(exception, parameters);
            }

            var title = _localizer.GetString(AppStrings.ToastErrorTitle);
            var message = _localizer.GetString(AppStrings.TheOpreationFailedMessage);
            FxToast.Show(title, message, FxToastType.Error);
        }
#endif

    }

    public void Track(Exception exception, IDictionary<string, string>? parameters = null)
    {
#if DEBUG
        var message = (exception as KnownException)?.Message ?? exception.ToString();
        Console.WriteLine(message);
        Debug.WriteLine(message);
#else
        if (DeviceInfo.Current.Platform != DevicePlatform.macOS && DeviceInfo.Current.Platform != DevicePlatform.MacCatalyst)
        {
            Crashes.TrackError(exception, parameters);
        }
#endif
    }
}
