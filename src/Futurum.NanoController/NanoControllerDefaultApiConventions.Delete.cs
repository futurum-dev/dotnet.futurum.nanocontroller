using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Futurum.NanoController;

public static partial class NanoControllerDefaultApiConventions
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public static void DeleteAsync([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)] [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] object id,
                                   CancellationToken cancellationToken)
    {
    }
}