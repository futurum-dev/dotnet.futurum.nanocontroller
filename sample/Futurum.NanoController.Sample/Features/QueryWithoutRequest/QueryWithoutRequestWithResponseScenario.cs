using Futurum.Core.Result;

using Microsoft.AspNetCore.Mvc;

namespace Futurum.NanoController.Sample.Features.QueryWithoutRequest;

public static class QueryWithoutRequestWithResponseScenario
{
    public class WebApi : NanoController.Query<FeatureDto>
    {
        public WebApi(INanoControllerRouter router) : base(router)
        {
        }

        [HttpGet(NanoControllerRoute.VersionedStaticClass)]
        public override Task<ActionResult<FeatureDto>> GetAsync(CancellationToken cancellationToken = default) =>
            Router.ExecuteAsync(new Command(), cancellationToken)
                  .MapAsync(FeatureMapper.Map)
                  .ToBadRequestAsync(this);
    }

    public record Command : INanoControllerRequest<Feature>;

    public class Handler : INanoControllerHandler<Command, Feature>
    {
        public Task<Result<Feature>> ExecuteAsync(Command request, CancellationToken cancellationToken = default) =>
            Result.OkAsync(new Feature(Guid.NewGuid().ToString()));
    }
}