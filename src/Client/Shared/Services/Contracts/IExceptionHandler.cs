namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IExceptionHandler
{
    void Handle(Exception exception, IDictionary<string, string>? parameters = null);
    void Track(Exception exception, IDictionary<string, string>? parameters = null);
}
