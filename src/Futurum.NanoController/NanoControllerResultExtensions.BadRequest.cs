using System.Net;

using Futurum.Core.Functional;
using Futurum.Core.Result;

using Microsoft.AspNetCore.Mvc;

namespace Futurum.NanoController;

public static partial class NanoControllerResultExtensions
{
    public static IActionResult ToBadRequest(this Result result, NanoController.Base nanoController) =>
        result.ToWebApi(HttpStatusCode.BadRequest, nanoController);

    public static ActionResult<T> ToBadRequest<T>(this Result<T> result, NanoController.Base nanoController) =>
        result.ToWebApi(HttpStatusCode.BadRequest, nanoController);

    public static Task<IActionResult> ToBadRequestAsync(this Task<Result> resultTask, NanoController.Base nanoController) =>
        resultTask.ToWebApiAsync(HttpStatusCode.BadRequest, nanoController);

    public static Task<ActionResult<T>> ToBadRequestAsync<T>(this Task<Result<T>> resultTask, NanoController.Base nanoController) =>
        resultTask.ToWebApiAsync(HttpStatusCode.BadRequest, nanoController);

    public static Task<IActionResult> ToBadRequestAsync(this Task<Result<Unit>> resultTask, NanoController.Base nanoController) =>
        resultTask.ToWebApiAsync(HttpStatusCode.BadRequest, nanoController);
}