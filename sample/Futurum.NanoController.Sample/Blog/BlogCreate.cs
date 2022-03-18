using FluentValidation;

using Futurum.Core.Option;
using Futurum.Core.Result;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace Futurum.NanoController.Sample.Blog;

public static class BlogCreate
{
    public record CommandDto(string Url);

    public class WebApi : NanoController.Command<CommandDto, BlogDto>.Post
    {
        public WebApi(INanoControllerRouter router)
            : base(router)
        {
        }
        
        [ApiVersion(WebApiVersions.V1_0)]
        [SwaggerOperation(Summary = "Create Blog")]
        public override Task<ActionResult<BlogDto>> PostAsync(CommandDto commandDto, CancellationToken cancellationToken = default) =>
            Router.ExecuteAsync(new Command(new Blog(Option<Id>.None, commandDto.Url)), cancellationToken)
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
            _storageBroker.AddAsync(command.Blog);
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Blog.Url).NotEmpty().WithMessage($"must have a value;");
        }
    }
}