using System.Reflection;

namespace Futurum.NanoController.Internal;

internal static class MetadataTypeService
{
    public static IEnumerable<MetadataTypeDefinition> Get(IEnumerable<Assembly> assemblies)
    {
        return assemblies.SelectMany(x => x.GetTypes())
                         .Where(type => type.IsClosedTypeOf(typeof(INanoControllerHandler<,>)))
                         .Select(handlerType =>
                         {
                             var handlerInterfaceType = handlerType.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INanoControllerHandler<,>));

                             var requestType = handlerInterfaceType.GetGenericArguments()[0];
                             var responseType = handlerInterfaceType.GetGenericArguments()[1];

                             return new MetadataTypeDefinition(requestType, responseType, handlerInterfaceType, handlerType);
                         });
    }
}