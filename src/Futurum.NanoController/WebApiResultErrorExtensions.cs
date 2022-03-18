using System.Net;

using Futurum.Core.Result;

namespace Futurum.NanoController;

public static class WebApiResultErrorExtensions
{
    public static IResultError ToResultError(this HttpStatusCode httpStatusCode) =>
        new WebApiResultError(httpStatusCode, ResultErrorEmpty.Value);

    public static IResultError ToResultError(this HttpStatusCode httpStatusCode, string detailErrorMessage) =>
        new WebApiResultError(httpStatusCode, detailErrorMessage);

    public static IResultError ToResultError(this HttpStatusCode httpStatusCode, IResultError error) =>
        new WebApiResultError(httpStatusCode, error);
}