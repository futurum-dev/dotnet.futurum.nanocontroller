using FluentValidation;

using Futurum.Core.Result;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace Futurum.NanoController.Sample.Blog;

public static class BlogUpdate
{
    public record CommandDto(long Id, string Url);

    public class WebApi : NanoController.Command<CommandDto, BlogDto>.Put
    {
        public WebApi(INanoControllerRouter router)
            : base(router)
        {
        }

        [ApiVersion(WebApiVersions.V1_0)]
        [SwaggerOperation(Summary = "Update Blog")]
        public override Task<ActionResult<BlogDto>> PutAsync(CommandDto commandDto, CancellationToken cancellationToken = default) =>
            Router.ExecuteAsync(new Command(new Blog(commandDto.Id.ToId(), commandDto.Url)), cancellationToken)
                  .MapAsync(BlogMapper.MapToDto)
                  .ToBadRequestAsync(this);
    }

    public record Command(Blog Blog) : INanoControllerRequest<Blog>;

    public class Handler : INanoControllerHandler<Command, Blog>
    {
        private readonly IBlogStorageBroker _storageBroker;

        public Handler(IBlogStorageBroker storageBroker)
        {
            _storageBroker = storageBroker;
        }

        public Task<Result<Blog>> ExecuteAsync(Command command, CancellationToken cancellationToken) =>
            _storageBroker.UpdateAsync(command.Blog);
    }


    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Blog.Url).NotEmpty().WithMessage($"must have a value;");
        }
    }
}