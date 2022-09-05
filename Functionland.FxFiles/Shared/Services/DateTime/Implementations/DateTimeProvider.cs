using Functionland.FxFiles.Shared.Services.DateTime.Contracts;

namespace Functionland.FxFiles.Shared.Services.DateTime.Implementations;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset GetCurrentDateTime()
    {
        return DateTimeOffset.UtcNow;
    }
}
