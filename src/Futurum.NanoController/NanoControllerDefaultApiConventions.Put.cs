using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Futurum.NanoController;

public static partial class NanoControllerDefaultApiConventions
{
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public static void PutAsync([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)] [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] object model,
                                CancellationToken cancellationToken)
    {
    }
}