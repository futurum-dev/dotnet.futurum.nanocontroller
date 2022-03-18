using Futurum.Microsoft.Extensions.DependencyInjection;
using Futurum.NanoController.Sample.Blog;
using Futurum.NanoController.Sample.WeatherForecast;

namespace Futurum.NanoController.Sample;

public class ApplicationModule : IModule
{
    public void Load(IServiceCollection services)
    {
        services.AddSingleton<IWeatherForecastMapper, WeatherForecastMapper>();
        services.AddSingleton<IBlogStorageBroker, BlogStorageBroker>();
    }
}