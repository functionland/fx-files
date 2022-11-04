namespace Functionland.FxFiles.Client.Shared.Infra;

/// <summary>
/// https://bitplatform.dev/todo-template/hosting-models
/// </summary>
public class WebAppDeploymentTypeDetector
{
    public static WebAppDeploymentTypeDetector Current { get; set; } = new WebAppDeploymentTypeDetector();

    public virtual bool IsDefault()
    {
        return Mode == WebAppDeploymentType.Default;
    }

    public virtual bool IsPwa()
    {
        return Mode == WebAppDeploymentType.Pwa;
    }

    public virtual WebAppDeploymentType Mode
    {
        get
        {
#if PWA
            return WebAppDeploymentType.Pwa;
#else
            return WebAppDeploymentType.Default;
#endif
        }
    }
}
