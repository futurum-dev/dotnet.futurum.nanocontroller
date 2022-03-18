using Futurum.Core.Result;

using Microsoft.AspNetCore.Mvc;

namespace Futurum.NanoController.Sample.Features.QueryWithRequest;

public static class QueryByQueryWithResponseScenario
{
    public class WebApi : NanoController.Query<string, FeatureDto>
    {
        public WebApi(INanoControllerRouter router) : base(router)
        {
        }

        [HttpGet(NanoControllerRoute.VersionedStaticClass)]
        public override Task<ActionResult<FeatureDto>> GetAsync(string id, CancellationToken cancellationToken = default) =>
            Router.ExecuteAsync(new Command(id), cancellationToken)
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