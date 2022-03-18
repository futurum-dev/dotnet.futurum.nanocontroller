using Futurum.Core.Result;

using Microsoft.AspNetCore.Mvc;

namespace Futurum.NanoController.Sample.Features.CommandWithResponse;

public static class CommandWithRequestWithResponseScenario
{
    public record CommandDto(string Id);

    public class WebApi : NanoController.Command<CommandDto, FeatureDto>.Post
    {
        public WebApi(INanoControllerRouter router) : base(router)
        {
        }

        [HttpPost(NanoControllerRoute.VersionedStaticClass)]
        public override Task<ActionResult<FeatureDto>> PostAsync(CommandDto request, CancellationToken cancellationToken = default) =>
            Router.ExecuteAsync(new Command(request.Id), cancellationToken)
                  .MapAsync(FeatureMapper.Map)
                  .ToBadRequestAsync(this);
    }
    
    public record Command(string Id) : INanoControllerRequest<Feature>;
    
    public class Handler : INanoControllerHandler<Command, Feature>
    {
        public Task<Result<Feature>> ExecuteAsync(Command request, CancellationToken cancellationToken = default) =>
            new Feature($"Name - {request.Id}").ToResultOkAsync();
    }
}