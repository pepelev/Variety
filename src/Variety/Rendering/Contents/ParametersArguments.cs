using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Variety.Rendering.Code;

namespace Variety.Rendering.Contents;

internal readonly struct ParametersArguments : Content
{
    private readonly ImmutableArray<IParameterSymbol> parameters;

    public ParametersArguments(ImmutableArray<IParameterSymbol> parameters)
    {
        this.parameters = parameters;
    }

    public void Write(Output output)
    {
        var arguments = output.CommaSeparated();
        foreach (var parameter in parameters)
        {
            arguments.Append(ContentFactory.From(parameter.Name));
        }
    }

    public override string ToString() => this.Print();
}