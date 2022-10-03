using Functionland.FxFiles.Client.Shared.Shared;
using Microsoft.AppCenter.Crashes;
using System.Diagnostics;
using System.Linq;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public partial class ExceptionHandler : IExceptionHandler
{
    [AutoInject] IStringLocalizer<AppStrings> _localizer = default!;

    public void Handle(Exception exception, IDictionary<string, object?>? parameters = null)
    {
        Crashes.TrackError(exception);

#if DEBUG
        string exceptionMessage = (exception as KnownException)?.Message ?? exception.ToString();
        MessageBox.Show(exceptionMessage, _localizer[nameof(AppStrings.Error)]);
        Console.WriteLine(exceptionMessage);
        Debugger.Break();
#else
        if (exception is KnownException knownException)
        {
            MessageBox.Show(knownException.Message, _localizer[nameof(AppStrings.Error)]);
        }
        else
        {
            MessageBox.Show(_localizer[nameof(AppStrings.UnknownException)], _localizer[nameof(AppStrings.Error)]);
        }
#endif

    }
}
