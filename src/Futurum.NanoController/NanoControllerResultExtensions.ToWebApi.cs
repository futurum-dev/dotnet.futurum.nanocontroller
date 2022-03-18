using System.Net;

using Futurum.Core.Functional;
using Futurum.Core.Result;
using Futurum.NanoController.Internal;

using Microsoft.AspNetCore.Mvc;

namespace Futurum.NanoController;

public static partial class NanoControllerResultExtensions
{
    public static IActionResult ToWebApi(this Result result, HttpStatusCode httpStatusCode, NanoController.Base nanoController)
    {
        static IActionResult Success() => new OkResult();

        IActionResult Failure(IResultError error)
        {
            var problemDetails = error.ToProblemDetails((int)httpStatusCode, nanoController.Request.Path);
            return new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status
            };
        }

        return result.Switch(Success, Failure);
    }

    public static ActionResult<T> ToWebApi<T>(this Result<T> result, HttpStatusCode httpStatusCode, NanoController.Base nanoController)
    {
        static ActionResult<T> Success(T value) => new OkObjectResult(value);

        ActionResult<T> Failure(IResultError error)
        {
            var problemDetails = error.ToProblemDetails((int)httpStatusCode, nanoController.Request.Path);
            return new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status
            };
        }

        return result.Switch(Success, Failure);
    }

    public static async Task<IActionResult> ToWebApiAsync(this Task<Result> resultTask, HttpStatusCode httpStatusCode, NanoController.Base nanoController)
    {
        var result = await resultTask;

        return result.ToWebApi(httpStatusCode, nanoController);
    }

    public static async Task<ActionResult<T>> ToWebApiAsync<T>(this Task<Result<T>> resultTask, HttpStatusCode httpStatusCode, NanoController.Base nanoController)
    {
        var result = await resultTask;

        return result.ToWebApi(httpStatusCode, nanoController);
    }

    public static async Task<IActionResult> ToWebApiAsync(this Task<Result<Unit>> resultTask, HttpStatusCode httpStatusCode, NanoController.Base nanoController)
    {
        var result = await resultTask;

        return result.ToNonGeneric().ToWebApi(httpStatusCode, nanoController);
    }
}