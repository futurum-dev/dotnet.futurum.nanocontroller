using Futurum.Microsoft.Extensions.DependencyInjection;
using Futurum.NanoController;
using Futurum.NanoController.OpenApi;
using Futurum.NanoController.Sample;

using Microsoft.AspNetCore.Mvc;

using Serilog;

Log.Logger = new LoggerConfiguration()
             .Enrich.FromLogContext()
             .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
             .CreateBootstrapLogger();

try
{
    Log.Information("Application starting up");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((hostBuilderContext, loggerConfiguration) =>
                                loggerConfiguration.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                                                   .ReadFrom.Configuration(hostBuilderContext.Configuration));

    builder.Host.ConfigureServices(serviceCollection =>
    {
        serviceCollection.RegisterModule(new NanoControllerModule(typeof(AssemblyHook).Assembly));

        serviceCollection.RegisterModule<NanoControllerSerilogLoggerModule>();
        
        serviceCollection.RegisterModule<ApplicationModule>();
    });

    builder.Services.AddNanoControllers();
    builder.Services.EnableApiVersioningForNanoControllers(ApiVersion.Parse(WebApiVersions.V1_0));

    builder.Services.EnableOpenApiForNanoControllers<OpenApiInformation>();

    var application = builder.Build();

    application.UseSerilogRequestLogging();

    if (application.Environment.IsDevelopment())
    {
        application.UseOpenApiUIForNanoControllers();
    }

    application.UseHttpsRedirection();

    application.UseAuthorization();

    application.MapControllers();

    application.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application start-up failed");
}
finally
{
    Log.Information("Application shut down complete");
    Log.CloseAndFlush();
}

namespace Futurum.NanoController.Sample
{
    public partial class Program
    {
    }
}