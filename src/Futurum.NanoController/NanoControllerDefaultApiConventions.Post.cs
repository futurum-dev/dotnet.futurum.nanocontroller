using System.Net.Mime;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Futurum.NanoController;

public static partial class NanoControllerDefaultApiConventions
{
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public static void PostAsync([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)] [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] object model,
                                 CancellationToken cancellationToken)
    {
    }
}