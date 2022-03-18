using Futurum.Core.Result;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace Futurum.NanoController.Sample.Blog;

public static class BlogGet
{
    public class WebApi : NanoController.Query<DataCollectionDto<BlogDto>>
    {
        public WebApi(INanoControllerRouter router)
            : base(router)
        {
        }

        [ApiVersion(WebApiVersions.V1_0)]
        [SwaggerOperation(Summary = "Get Blogs")]
        public override Task<ActionResult<DataCollectionDto<BlogDto>>> GetAsync(CancellationToken cancellationToken = default) =>
            Router.ExecuteAsync(new Query(), cancellationToken)
                  .MapAsAsync(BlogMapper.MapToDto)
                  .ToDataCollectionDtoAsync()
                  .ToBadRequestAsync(this);
    }

    public record Query : INanoControllerRequest<IEnumerable<Blog>>;

    public class Handler : INanoControllerHandler<Query, IEnumerable<Blog>>
    {
        private readonly IBlogStorageBroker _storageBroker;

        public Handler(IBlogStorageBroker storageBroker)
        {
            _storageBroker = storageBroker;
        }

        public Task<Result<IEnumerable<Blog>>> ExecuteAsync(Query query, CancellationToken cancellationToken) =>
            _storageBroker.GetAsync();
    }
}