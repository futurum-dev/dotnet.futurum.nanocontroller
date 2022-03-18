using Futurum.Microsoft.Extensions.DependencyInjection;
using Futurum.NanoController;
using Futurum.NanoController.Benchmark.NanoController;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

builder.Host.ConfigureServices(serviceCollection =>
{
    serviceCollection.RegisterModule(new NanoControllerModule(typeof(AssemblyHook).Assembly));

    serviceCollection.AddSingleton<INanoControllerLogger, NoOpNanoControllerLogger>();
});

builder.Services.AddNanoControllers()
       .AddJsonOptions(options => { options.JsonSerializerOptions.AddContext<NanoControllerJsonSerializerContext>(); });

// builder.Services.EnableOpenApiForNanoControllers<OpenApiInformation>();

builder.Services.AddAuthorization();

var application = builder.Build();

// if (application.Environment.IsDevelopment())
// {
//     application.UseOpenApiUIForNanoControllers();
// }

application.UseAuthorization();

application.MapControllers();

application.Run();

namespace Futurum.NanoController.Benchmark.NanoController
{
    public partial class Program
    {
    }
}