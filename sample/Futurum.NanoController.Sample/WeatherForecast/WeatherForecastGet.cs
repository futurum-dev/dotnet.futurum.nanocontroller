using Futurum.Core.Result;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace Futurum.NanoController.Sample.WeatherForecast;

public static class WeatherForecastGet
{
    public class WebApi : NanoController.Query<DataCollectionDto<WeatherForecastDto>>
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
        [SwaggerOperation(Summary = "Get WeatherForecasts")]
        public override Task<ActionResult<DataCollectionDto<WeatherForecastDto>>> GetAsync(CancellationToken cancellationToken = default) =>
            Router.ExecuteAsync(new Request(), cancellationToken)
                  .MapAsAsync(_mapper.MapToDto)
                  .ToDataCollectionDtoAsync()
                  .ToBadRequestAsync(this);
    }

    public record Request : INanoControllerRequest<IEnumerable<WeatherForecast>>;

    public class Handler : INanoControllerHandler<Request, IEnumerable<WeatherForecast>>
    {
        private static readonly string[] Summaries = { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

        public Task<Result<IEnumerable<WeatherForecast>>> ExecuteAsync(Request query, CancellationToken cancellationToken = default) =>
            Enumerable.Range(1, 5)
                      .Select(index => new WeatherForecast(DateTime.Now.AddDays(index), Random.Shared.Next(-20, 55), Summaries[Random.Shared.Next(Summaries.Length)]))
                      .ToResultOkAsync();
    }
}