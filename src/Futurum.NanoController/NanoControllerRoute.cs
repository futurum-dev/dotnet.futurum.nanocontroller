namespace Futurum.NanoController;

public static class NanoControllerRoute
{
    public const string VersionedNamespace = "api/v{version:apiVersion}/[namespace]";
    public const string VersionedStaticClass = "api/v{version:apiVersion}/[static-class]";
}