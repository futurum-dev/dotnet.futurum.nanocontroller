using System.Reflection;

using Microsoft.AspNetCore.Mvc.Controllers;

namespace Futurum.NanoController.Internal;

internal class NanoControllerFeatureProvider : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo typeInfo) =>
        typeof(NanoController.Base).IsAssignableFrom(typeInfo);
}