using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Variety.Rendering.Code;

namespace Variety.Rendering.Contents;

internal readonly struct Type : Content
{
    private static readonly string?[] arraySuffixesCache = new string?[16];
    private readonly ITypeSymbol symbol;
    private readonly bool globalPrefix;
    private readonly bool verbatimPrefix;

    public Type(ITypeSymbol symbol, bool verbatimPrefix = true, bool globalPrefix = true)
    {
        this.symbol = symbol;
        this.verbatimPrefix = verbatimPrefix;
        this.globalPrefix = globalPrefix;
    }

    public void Write(Output output)
    {
        var @this = this;
        Print(symbol);

        void Print(ITypeSymbol type)
        {
            if (type is IErrorTypeSymbol error)
            {
                output.Write(error.ToDisplayString());
                return;
            }

            if (type is IArrayTypeSymbol arrayType)
            {
                var arrays = new Queue<IArrayTypeSymbol>();
                ITypeSymbol element = arrayType;
                for (; element is IArrayTypeSymbol arrayElement; element = arrayElement.ElementType)
                {
                    arrays.Enqueue(arrayElement);
                }

                Print(element);
                foreach (var arrayTypeSymbol in arrays)
                {
                    output.Write(GetArraySuffix(arrayTypeSymbol.Rank));
                }

                return;
            }

            if (type is INamedTypeSymbol namedType)
            {
                if (namedType is { IsGenericType: true, TypeArguments.Length: > 0 } && namedType.TypeArguments.All(argument => argument.Kind != SymbolKind.TypeParameter))
                {
                    Print(namedType.ConstructedFrom);
                    output.Write("<");
                    var list = output.CommaSeparated();

                    foreach (var argument in namedType.TypeArguments)
                    {
                        list.Append(new Type(argument, verbatimPrefix: @this.verbatimPrefix, globalPrefix: @this.globalPrefix));
                    }

                    output.Write(">");
                    return;
                }
            }

            var outer = type.ContainingType;
            if (outer == null)
            {
                var @namespace = new Namespace(
                    type.ContainingNamespace,
                    @this.globalPrefix,
                    verbatimPrefix: @this.verbatimPrefix
                );
                @namespace.Write(output);
                output.Write(@this.verbatimPrefix ? ".@" : ".");
                output.Write(type.Name);
                return;
            }

            Print(outer);
            output.Write(@this.verbatimPrefix ? ".@" : ".");
            output.Write(type.Name);
        }
    }

    private static string GetArraySuffix(int rank)
    {
        if (rank < arraySuffixesCache.Length)
        {
            if (arraySuffixesCache[rank] is { } value)
            {
                return value;
            }

            var suffix = Suffix();
            arraySuffixesCache[rank] = suffix;
            return suffix;
        }

        return Suffix();

        string Suffix() => $"[{new string(',', rank - 1)}]";
    }

    public override string ToString() => this.Print();
}