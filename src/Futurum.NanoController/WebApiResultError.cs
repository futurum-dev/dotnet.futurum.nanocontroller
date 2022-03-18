using System.Net;

using Futurum.Core.Linq;
using Futurum.Core.Option;
using Futurum.Core.Result;

using Microsoft.AspNetCore.WebUtilities;

namespace Futurum.NanoController;

public class WebApiResultError : IResultErrorComposite
{
    internal WebApiResultError(HttpStatusCode httpStatusCode, string detailErrorMessage)
    {
        HttpStatusCode = httpStatusCode;
        Parent = ReasonPhrases.GetReasonPhrase((int)HttpStatusCode).ToResultError().ToOption();
        Children = EnumerableExtensions.Return(detailErrorMessage.ToResultError());
    }

    internal WebApiResultError(HttpStatusCode httpStatusCode, IResultError error)
    {
        HttpStatusCode = httpStatusCode;
        Parent = ReasonPhrases.GetReasonPhrase((int)HttpStatusCode).ToResultError().ToOption();
        Children = EnumerableExtensions.Return(error);
    }

    /// <inheritdoc />
    public Option<IResultErrorNonComposite> Parent { get; }

    /// <inheritdoc />
    public IEnumerable<IResultError> Children { get; }

    public HttpStatusCode HttpStatusCode { get; }

    /// <inheritdoc />
    public string GetErrorString(string seperator)
    {
        string Transform(IResultError resultError) =>
            resultError.ToErrorString(seperator);

        string GetChildrenErrorString() =>
            Children.Select(Transform)
                    .StringJoin(seperator);

        string GetParentErrorString(IResultErrorNonComposite parent) =>
            parent.GetErrorString();

        return Parent.Switch(parent => $"{GetParentErrorString(parent)}{seperator}{GetChildrenErrorString()}",
                             GetChildrenErrorString);
    }

    public string GetChildrenErrorString(string seperator)
    {
        string Transform(IResultError resultError) =>
            resultError.ToErrorString(seperator);

        string GetChildrenErrorString() =>
            Children.Select(Transform)
                    .StringJoin(seperator);

        return GetChildrenErrorString();
    }

    /// <inheritdoc />
    public ResultErrorStructure GetErrorStructure() =>
        Parent.Switch(parent => ResultErrorStructureExtensions.ToResultErrorStructure(parent.GetErrorString(), Children.Select(ResultErrorStructureExtensions.ToErrorStructure)),
                      () => ResultErrorStructureExtensions.ToResultErrorStructure(Children.Select(ResultErrorStructureExtensions.ToErrorStructure)));
}