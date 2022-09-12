using System.Diagnostics;

namespace Functionland.FxFiles.Shared.Services.Implementations;

public partial class ExceptionHandler : IExceptionHandler
{
    //[AutoInject] IStringLocalizer<AppStrings> _localizer = default!;

    public void Handle(Exception exception, IDictionary<string, object?>? parameters = null)
    {
#if DEBUG
        string exceptionMessage = (exception as KnownException)?.Message ?? exception.ToString();
        //TODO: Show as a MessageBox
        Console.WriteLine(exceptionMessage);
        Debugger.Break();
#else
        if (exception is KnownException knownException)
        {
            //TODO: Show knownException.Message
        }
        else
        {
            //TODO: Show _localizer[nameof(AppStrings.UnknownException)]
        }
#endif

    }
}
