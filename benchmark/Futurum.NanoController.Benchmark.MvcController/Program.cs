using FluentValidation;

using Futurum.NanoController.Benchmark.MvcController;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

builder.Host.ConfigureServices(serviceCollection => serviceCollection.AddSingleton<IValidator<RequestDto>, Validator>());

builder.Services.AddControllers()
       .AddJsonOptions(options => { options.JsonSerializerOptions.AddContext<MvcControllerJsonSerializerContext>(); });

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

var application = builder.Build();

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

application.UseAuthorization();

application.MapControllers();

application.Run();

namespace Futurum.NanoController.Benchmark.MvcController
{
    public partial class Program
    {
    }
}