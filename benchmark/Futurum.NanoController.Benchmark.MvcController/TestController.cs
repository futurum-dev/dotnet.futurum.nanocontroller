using System.Text.Json.Serialization;

using FluentValidation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurum.NanoController.Benchmark.MvcController;

public record RequestDto(string? FirstName, string? LastName, int Age, IEnumerable<string>? PhoneNumbers);

public record ResponseDto(int Id, string? Name, int Age, string? PhoneNumber);

public class TestController : Controller
{
    private readonly IValidator<RequestDto> _validator;

    public TestController(IValidator<RequestDto> validator)
    {
        _validator = validator;
    }

    [AllowAnonymous]
    [HttpPost("api/benchmark/{id}")]
    public IActionResult Index([FromRoute] int id, [FromBody] RequestDto requestDto)
    {
        _validator.Validate(requestDto);

        return Ok(new ResponseDto(id, $"{requestDto.FirstName} {requestDto.LastName}", requestDto.Age, requestDto.PhoneNumbers?.FirstOrDefault()));
    }
}

public class Validator : AbstractValidator<RequestDto>
{
    public Validator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First Name needed");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last Nast needed");
        RuleFor(x => x.Age).GreaterThan(21).WithMessage("You must be at least 21 years old");
        RuleFor(x => x.PhoneNumbers).NotEmpty().WithMessage("Phone Number needed");
    }
}

[JsonSerializable(typeof(RequestDto))]
[JsonSerializable(typeof(ResponseDto))]
public partial class MvcControllerJsonSerializerContext : JsonSerializerContext
{
}