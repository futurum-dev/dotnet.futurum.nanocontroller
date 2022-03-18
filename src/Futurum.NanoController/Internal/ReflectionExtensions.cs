namespace Futurum.NanoController.Internal;

/// <summary>
/// Borrowed from Autofac
/// </summary>
internal static class ReflectionExtensions
{
    /// <summary>
    /// Determines whether the candidate type supports any base or
    /// interface that closes the provided generic type.
    /// </summary>
    /// <param name="typeToCheck">The type to test.</param>
    /// <param name="openGeneric">The open generic against which the type should be tested.</param>
    public static bool IsClosedTypeOf(this Type typeToCheck, Type openGeneric)
    {
        return typeToCheck.GetTypesThatClose(openGeneric).Any();
    }
    
    /// <summary>
    /// Returns the first concrete interface supported by the candidate type that
    /// closes the provided open generic service type.
    /// </summary>
    /// <param name="typeToCheck">The type that is being checked for the interface.</param>
    /// <param name="openGeneric">The open generic type to locate.</param>
    /// <returns>The type of the interface.</returns>
    private static IEnumerable<Type> GetTypesThatClose(this Type typeToCheck, Type openGeneric)
    {
        return FindAssignableTypesThatClose(typeToCheck, openGeneric);
    }
    /// <summary>
    /// Looks for an interface on the candidate type that closes the provided open generic interface type.
    /// </summary>
    /// <param name="candidateType">The type that is being checked for the interface.</param>
    /// <param name="openGenericServiceType">The open generic service type to locate.</param>
    /// <returns>True if a closed implementation was found; otherwise false.</returns>
    private static IEnumerable<Type> FindAssignableTypesThatClose(Type candidateType, Type openGenericServiceType)
    {
        return TypesAssignableFrom(candidateType)
            .Where(t => t.IsClosedTypeOfInternal(openGenericServiceType));
    }
    /// <summary>
    /// Checks whether this type is a closed type of a given generic type.
    /// </summary>
    /// <param name="typeToCheck">The type we are checking.</param>
    /// <param name="openGeneric">The open generic type to validate against.</param>
    /// <returns>True if <paramref name="typeToCheck"/> is a closed type of <paramref name="openGeneric"/>. False otherwise.</returns>
    private static bool IsClosedTypeOfInternal(this Type typeToCheck, Type openGeneric)
    {
        return TypesAssignableFrom(typeToCheck).Any(t => t.IsGenericType && !typeToCheck.ContainsGenericParameters && t.GetGenericTypeDefinition() == openGeneric);
    }

    private static IEnumerable<Type> TypesAssignableFrom(Type candidateType)
    {
        return candidateType.GetInterfaces().Concat(Traverse.Across(candidateType, t => t.BaseType!));
    }
    
    /// <summary>
    /// Provides a method to support traversing structures.
    /// </summary>
    private static class Traverse
    {
        /// <summary>
        /// Traverse across a set, taking the first item in the set, and a function to determine the next item.
        /// </summary>
        /// <typeparam name="T">The set type.</typeparam>
        /// <param name="first">The first item in the set.</param>
        /// <param name="next">A callback that will take the current item in the set, and output the next one.</param>
        /// <returns>An enumerable of the set.</returns>
        public static IEnumerable<T> Across<T>(T first, Func<T, T> next)
            where T : class
        {
            var item = first;
            while (item != null)
            {
                yield return item;
                item = next(item);
            }
        }
    }
}