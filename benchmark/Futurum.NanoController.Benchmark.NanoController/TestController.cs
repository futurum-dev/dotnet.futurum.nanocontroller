using System.Text.Json.Serialization;

using FluentValidation;

using Futurum.Core.Result;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Futurum.NanoController.Benchmark.NanoController;

public static class TestNanoController
{
    public class RequestDto
    {
        [FromRoute(Name = "id")] public int Id { get; set; }

        [FromBody] public BodyDto Body { get; set; }

        public class BodyDto
        {
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public int Age { get; set; }
            public IEnumerable<string>? PhoneNumbers { get; set; }
        }
    }

    public record ResponseDto(int Id, string? Name, int Age, string? PhoneNumber);

    public class WebApi : Futurum.NanoController.NanoController.Command<RequestDto, ResponseDto>.Post
    {
        public WebApi(INanoControllerRouter router) : base(router)
        {
        }

        [AllowAnonymous]
        [HttpPost("api/benchmark/{id}")]
        public override Task<ActionResult<ResponseDto>> PostAsync(RequestDto request, CancellationToken cancellationToken = default) =>
            Router.ExecuteAsync(new Request(request.Id, request.Body.FirstName, request.Body.LastName, request.Body.Age, request.Body.PhoneNumbers), cancellationToken)
                  .MapAsync(x => new ResponseDto(x.Id, x.Name, x.Age, x.PhoneNumber))
                  .ToBadRequestAsync(this);
    }

    public record Request(int Id, string FirstName, string LastName, int Age, IEnumerable<string> PhoneNumbers) : INanoControllerRequest<Response>;

    public record Response(int Id, string Name, int Age, string PhoneNumber);

    public class Handler : INanoControllerHandler<Request, Response>
    {
        public Task<Result<Response>> ExecuteAsync(Request request, CancellationToken cancellationToken = default) =>
            Result.OkAsync(new Response(request.Id, $"{request.FirstName} {request.LastName}", request.Age, request.PhoneNumbers?.FirstOrDefault()));
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First Name needed");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last Nast needed");
            RuleFor(x => x.Age).GreaterThan(21).WithMessage("You must be at least 21 years old");
            RuleFor(x => x.PhoneNumbers).NotEmpty().WithMessage("Phone Number needed");
        }
    }
}

[JsonSerializable(typeof(TestNanoController.RequestDto))]
[JsonSerializable(typeof(TestNanoController.ResponseDto))]
public partial class NanoControllerJsonSerializerContext : JsonSerializerContext
{
}