using Futurum.Core.Result;

using Microsoft.AspNetCore.Mvc;

namespace Futurum.NanoController.Sample.Features.UploadFile;

public static class RequestUploadFileWithResponseScenario
{
    public class WebApi : NanoController.UploadFile.Response<FeatureDto>.Post
    {
        public WebApi(INanoControllerRouter router) : base(router)
        {
        }

        [HttpPost(NanoControllerRoute.VersionedStaticClass)]
        public override Task<ActionResult<FeatureDto>> PostAsync(IFormFile formFile, CancellationToken cancellationToken = default) =>
            Router.ExecuteAsync(new Command(formFile), cancellationToken)
                  .MapAsync(FeatureMapper.Map)
                  .ToBadRequestAsync(this);
    }

    public record Command(IFormFile FormFile) : INanoControllerRequest<Feature>;

    public class Handler : INanoControllerHandler<Command, Feature>
    {
        public Task<Result<Feature>> ExecuteAsync(Command request, CancellationToken cancellationToken = default) =>
            Enumerable.Range(0, 10)
                      .Select(i => new Feature($"Name - {i} - {request.FormFile.FileName}"))
                      .First()
                      .ToResultOkAsync();
    }
}