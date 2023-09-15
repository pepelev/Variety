using Variety.Rendering.Contents;

namespace Variety.Rendering.Code;

internal static class Struct
{
    public static Type<T> Open<T>(Output output, string modifiers, T name) where T : Content
        => Type<T>.Open(output, modifiers, "struct", name);
}