using System.Text.RegularExpressions;

namespace Futurum.NanoController.Internal;

internal static class SlugifyExtensions
{
    public static string ToSlugify(this string controllerNamespace) =>
        Regex.Replace(controllerNamespace, "([a-z])([A-Z])", "$1-$2").ToLower();
}