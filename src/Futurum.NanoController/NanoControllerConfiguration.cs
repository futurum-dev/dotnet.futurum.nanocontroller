namespace Futurum.NanoController;

public record NanoControllerConfiguration(bool EnableMiddleware)
{
    /// <summary>
    /// Default NanoController configuration
    /// </summary>
    public static NanoControllerConfiguration Default =>
        new(false);
}