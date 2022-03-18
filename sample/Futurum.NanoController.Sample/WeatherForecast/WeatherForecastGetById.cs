using Futurum.Core.Result;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace Futurum.NanoController.Sample.WeatherForecast;

public static class WeatherForecastGetById
{
    public class WebApi : NanoController.QueryById<string, WeatherForecastDto>
    {
        private readonly IWeatherForecastMapper _mapper;

        public WebApi(INanoControllerRouter router,
                      IWeatherForecastMapper mapper)
            : base(router)
        {
            _mapper = mapper;
        }

        [ApiVersion(WebApiVersions.V1_0)]
        [ApiVersion(WebApiVersions.V2_0)]
        [SwaggerOperation(Summary = "Get WeatherForecasts", Description = "Get WeatherForecast by Id")]
        public override Task<ActionResult<WeatherForecastDto>> GetAsync(string id, CancellationToken cancellationToken = default) =>
            Router.ExecuteAsync(new Request(id), cancellationToken)
                  .MapAsync(_mapper.MapToDto)
                  .ToBadRequestAsync(this);
    }

    public record Request(string Id) : INanoControllerRequest<WeatherForecast>;

    public class Handler : INanoControllerHandler<Request, WeatherForecast>
    {
        private static readonly string[] Summaries = { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

        public Task<Result<WeatherForecast>> ExecuteAsync(Request query, CancellationToken cancellationToken = default) =>
            Enumerable.Range(1, 5)
                      .Select(index => new WeatherForecast(DateTime.Now.AddDays(index), Random.Shared.Next(-20, 55), Summaries[Random.Shared.Next(Summaries.Length)]))
                      .First()
                      .ToResultOkAsync();
    }
}