using Futurum.Core.Result;

using Microsoft.AspNetCore.Mvc;

namespace Futurum.NanoController.Sample.Features.UploadFiles;

public static class RequestUploadFilesWithResponseScenario
{
    public class WebApi : NanoController.UploadFiles.Response<FeatureDto>.Post
    {
        public WebApi(INanoControllerRouter router) : base(router)
        {
        }

        [HttpPost(NanoControllerRoute.VersionedStaticClass)]
        public override Task<ActionResult<FeatureDto>> PostAsync(IEnumerable<IFormFile> formFiles, CancellationToken cancellationToken = default) =>
            Router.ExecuteAsync(new Command(formFiles.Single()), cancellationToken)
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