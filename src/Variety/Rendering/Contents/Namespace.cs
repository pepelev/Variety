using Microsoft.CodeAnalysis;
using Variety.Rendering.Code;

namespace Variety.Rendering.Contents;

internal readonly struct Namespace : Content
{
    private readonly INamespaceSymbol symbol;
    private readonly bool globalPrefix;
    private readonly bool verbatimPrefix;

    public Namespace(INamespaceSymbol symbol, bool globalPrefix, bool verbatimPrefix = true)
    {
        this.symbol = symbol;
        this.globalPrefix = globalPrefix;
        this.verbatimPrefix = verbatimPrefix;
    }

    public void Write(Output output)
    {
        var @this = this;
        Print(symbol);

        void Print(INamespaceSymbol @namespace)
        {
            var outer = @namespace.ContainingNamespace;
            if (outer == null || outer.IsGlobalNamespace)
            {
                if (@this.globalPrefix)
                {
                    output.Write("global::");
                }

                if (@this.verbatimPrefix)
                {
                    output.Write("@");
                }

                output.Write(@namespace.Name);
            }
            else
            {
                Print(outer);
                output.Write(@this.verbatimPrefix ? ".@" : ".");
                output.Write(@namespace.Name);
            }
        }
    }

    public override string ToString() => this.Print();
}