using System.Text;
using Microsoft.CodeAnalysis;
using Variety.Rendering.Code;
using Variety.Rendering.Contents;

namespace Variety;

internal static class SourceProductionContextExtensions
{
    private const int InitialCapacity = 80 * 1024 / sizeof(char);
    private static Output? cached;

    public static void AddSource<T>(
        this SourceProductionContext context,
        string hintName,
        in T content)
        where T : Content
    {
        var read = Interlocked.Exchange(ref cached, null);
        var output = read ?? new Output(new StringBuilder(InitialCapacity));
        try
        {
            content.Write(output);
            context.AddSource(hintName, output.ToString());
        }
        finally
        {
            output.Clear();
            Interlocked.CompareExchange(ref cached, output, null);
        }
    }
}