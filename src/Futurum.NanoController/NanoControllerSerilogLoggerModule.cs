using Futurum.Microsoft.Extensions.DependencyInjection;

namespace Futurum.NanoController;

public class NanoControllerSerilogLoggerModule : IModule
{
    public void Load(IServiceCollection services)
    {
        services.AddSingleton<INanoControllerLogger, NanoControllerLogger>();
    }
}