using Variety.Rendering.Contents;

namespace Variety.Rendering.Code;

internal static class Class
{
    public static Type<T> Open<T>(Output output, string modifiers, T name) where T : Content
        => Type<T>.Open(output, modifiers, "class", name);
}