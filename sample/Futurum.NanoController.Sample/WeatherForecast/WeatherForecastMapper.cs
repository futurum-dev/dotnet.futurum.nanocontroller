using Futurum.Core.Result;

namespace Futurum.NanoController.Sample.WeatherForecast;

public interface IWeatherForecastMapper : IMapperDto<WeatherForecastDto, WeatherForecast>
{
}

public class WeatherForecastMapper : IWeatherForecastMapper
{
    public WeatherForecastDto MapToDto(WeatherForecast domain) => 
        new(domain.Date, domain.TemperatureC, domain.TemperatureF, domain.Summary);

    public Result<WeatherForecast> MapToDomain(WeatherForecastDto dto) =>
        new WeatherForecast(dto.Date, dto.TemperatureC, dto.Summary).ToResultOk();
}