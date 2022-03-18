using Futurum.Core.Functional;
using Futurum.Core.Result;

using Microsoft.AspNetCore.Mvc;

namespace Futurum.NanoController.Sample.Features.UploadFiles;

public static class RequestUploadFilesWithoutResponseScenario
{
    public class WebApi : NanoController.UploadFiles.NoResponse.Post
    {
        public WebApi(INanoControllerRouter router) : base(router)
        {
        }

        [HttpPost(NanoControllerRoute.VersionedStaticClass)]
        public override Task<IActionResult> PostAsync(IEnumerable<IFormFile> formFiles, CancellationToken cancellationToken = default) =>
            Router.ExecuteAsync(new Command(formFiles.Single()), cancellationToken)
                  .ToBadRequestAsync(this);
    }

    public record Command(IFormFile FormFile) : INanoControllerRequest;

    public class Handler : INanoControllerHandler<Command>
    {
        public Task<Result<Unit>> ExecuteAsync(Command request, CancellationToken cancellationToken = default) =>
            Result.OkAsync(Unit.Value);
    }
}