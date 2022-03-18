using Futurum.Core.Functional;
using Futurum.Core.Result;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace Futurum.NanoController.Sample.Blog;

public static class BlogDelete
{
    public record CommandDto(long Id);

    public class WebApi : NanoController.Command<CommandDto>.Delete
    {
        public WebApi(INanoControllerRouter router)
            : base(router)
        {
        }

        [ApiVersion(WebApiVersions.V1_0)]
        [SwaggerOperation(Summary = "Delete Blog")]
        public override Task<IActionResult> DeleteAsync(CommandDto commandDto, CancellationToken cancellationToken = default) =>
            Router.ExecuteAsync(new Command(commandDto.Id.ToId()), cancellationToken)
                  .ToBadRequestAsync(this);
    }

    public record Command(Id Id) : INanoControllerRequest;

    public class Handler : INanoControllerHandler<Command>
    {
        private readonly IBlogStorageBroker _storageBroker;

        public Handler(IBlogStorageBroker storageBroker)
        {
            _storageBroker = storageBroker;
        }

        public Task<Result<Unit>> ExecuteAsync(Command command, CancellationToken cancellationToken) =>
            _storageBroker.DeleteAsync(command.Id).MapAsync(Unit.Value);
    }
}