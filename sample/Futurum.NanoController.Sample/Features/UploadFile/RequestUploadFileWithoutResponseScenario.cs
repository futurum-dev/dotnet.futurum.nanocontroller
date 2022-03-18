using Futurum.Core.Functional;
using Futurum.Core.Result;

using Microsoft.AspNetCore.Mvc;

namespace Futurum.NanoController.Sample.Features.UploadFile;

public static class RequestUploadFileWithoutResponseScenario
{
    public class WebApi : NanoController.UploadFile.NoResponse.Post
    {
        public WebApi(INanoControllerRouter router) : base(router)
        {
        }

        [HttpPost(NanoControllerRoute.VersionedStaticClass)]
        public override Task<IActionResult> PostAsync(IFormFile formFile, CancellationToken cancellationToken = default) =>
            Router.ExecuteAsync(new Command(formFile), cancellationToken)
                  .ToBadRequestAsync(this);
    }

    public record Command(IFormFile FormFile) : INanoControllerRequest;

    public class Handler : INanoControllerHandler<Command>
    {
        public Task<Result<Unit>> ExecuteAsync(Command request, CancellationToken cancellationToken = default) =>
            Result.OkAsync(Unit.Value);
    }
}