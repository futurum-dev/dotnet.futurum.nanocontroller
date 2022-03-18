using System.Net;

using Futurum.Core.Functional;
using Futurum.Core.Result;

using Microsoft.AspNetCore.Mvc;

namespace Futurum.NanoController;

public static partial class NanoControllerResultExtensions
{
    public static IActionResult ToNotFound(this Result result, NanoController.Base nanoController) =>
        result.ToWebApi(HttpStatusCode.NotFound, nanoController);

    public static ActionResult<T> ToNotFound<T>(this Result<T> result, NanoController.Base nanoController) =>
        result.ToWebApi(HttpStatusCode.NotFound, nanoController);

    public static Task<IActionResult> ToNotFoundAsync(this Task<Result> resultTask, NanoController.Base nanoController) =>
        resultTask.ToWebApiAsync(HttpStatusCode.NotFound, nanoController);

    public static Task<ActionResult<T>> ToNotFoundAsync<T>(this Task<Result<T>> resultTask, NanoController.Base nanoController) =>
        resultTask.ToWebApiAsync(HttpStatusCode.NotFound, nanoController);

    public static Task<IActionResult> ToNotFoundAsync(this Task<Result<Unit>> resultTask, NanoController.Base nanoController) =>
        resultTask.ToWebApiAsync(HttpStatusCode.NotFound, nanoController);
}