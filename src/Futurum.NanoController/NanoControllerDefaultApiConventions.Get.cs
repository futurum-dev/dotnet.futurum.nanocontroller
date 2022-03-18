using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Futurum.NanoController;

public static partial class NanoControllerDefaultApiConventions
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public static void GetAsync(CancellationToken cancellationToken)
    {
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public static void GetAsync([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Suffix)] [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] object id,
                                CancellationToken cancellationToken)
    {
    }
}