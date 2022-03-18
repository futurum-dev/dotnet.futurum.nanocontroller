using Futurum.Core.Functional;
using Futurum.Core.Result;

using Microsoft.AspNetCore.Mvc;

namespace Futurum.NanoController.Sample.Features.Error;

public static class ErrorResultScenario
{
    public record CommandDto(string Id);

    public class WebApi : NanoController.Command<CommandDto>.Post
    {
        public WebApi(INanoControllerRouter router) : base(router)
        {
        }

        [HttpPost(NanoControllerRoute.VersionedStaticClass)]
        public override Task<IActionResult> PostAsync(CommandDto request, CancellationToken cancellationToken = default) =>
            Router.ExecuteAsync(new Command(request.Id), cancellationToken)
                  .ToBadRequestAsync(this);
    }
    
    public record Command(string Id) : INanoControllerRequest<Unit>;
    
    public class Handler : INanoControllerHandler<Command>
    {
        public Task<Result<Unit>> ExecuteAsync(Command request, CancellationToken cancellationToken = default) =>
            Result.FailAsync<Unit>($"An result error has occured - {request.Id}");
    }
}