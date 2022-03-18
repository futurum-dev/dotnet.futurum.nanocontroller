using Futurum.Core.Result;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace Futurum.NanoController.Sample.Blog;

public static class BlogGetById
{
    public class WebApi : NanoController.QueryById<long, BlogDto>
    {
        public WebApi(INanoControllerRouter router)
            : base(router)
        {
        }

        [ApiVersion(WebApiVersions.V1_0)]
        [SwaggerOperation(Summary = "Get Blogs By Id")]
        public override Task<ActionResult<BlogDto>> GetAsync(long id, CancellationToken cancellationToken = default) =>
            Router.ExecuteAsync(new Query(id.ToId()), cancellationToken)
                  .MapAsync(BlogMapper.MapToDto)
                  .ToBadRequestAsync(this);
    }

    public record Query(Id Id) : INanoControllerRequest<Blog>;

    public class Handler : INanoControllerHandler<Query, Blog>
    {
        private readonly IBlogStorageBroker _storageBroker;

        public Handler(IBlogStorageBroker storageBroker)
        {
            _storageBroker = storageBroker;
        }

        public Task<Result<Blog>> ExecuteAsync(Query query, CancellationToken cancellationToken) =>
            _storageBroker.GetByIdAsync(query.Id);
    }
}