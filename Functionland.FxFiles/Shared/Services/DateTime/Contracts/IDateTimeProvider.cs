namespace Functionland.FxFiles.Shared.Services.DateTime.Contracts;

public interface IDateTimeProvider
{
    DateTimeOffset GetCurrentDateTime();
}
