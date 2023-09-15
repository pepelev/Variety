using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Variety.Rendering.Code;
using Variety.Rendering.Contents;
using Type = Variety.Rendering.Contents.Type;

namespace Variety;

internal sealed class Vary : Content
{
    private readonly INamedTypeSymbol varyRecord;

    public Vary(INamedTypeSymbol varyRecord)
    {
        this.varyRecord = varyRecord;
    }

    public string Name => Type.Print() + ".g.cs";

    private ImmutableStack<INamedTypeSymbol> NestedHierarchy
    {
        get
        {
            return ImmutableStack.CreateRange(Yield());

            IEnumerable<INamedTypeSymbol> Yield()
            {
                for (var type = varyRecord; type != null; type = type.ContainingType)
                {
                    yield return type;
                }
            }
        }
    }

    private Type Type => new(varyRecord, globalPrefix: false);
    private Namespace Namespace => new(varyRecord.ContainingNamespace, globalPrefix: false);

    public void Write(Output output)
    {
        using (output.OpenNamespace(Namespace))
        {
            RenderPart(output, NestedHierarchy);
        }
    }

    private void RenderPart(Output output, ImmutableStack<INamedTypeSymbol> parent)
    {
        if (!parent.IsEmpty)
        {
            var top = parent.Peek();
            using (Type<Verbatim>.OpenPart(output, top))
            {
                RenderPart(output, parent.Pop());
                return;
            }
        }

        foreach (var innerRecord in InnerRecords)
        {
            using (Type<Verbatim>.OpenPart2(output, innerRecord, varyRecord))
            {
                output.WriteLine(
                    new Verbatim(
                        "public override TVisitorResult Accept<TVisitorResult>(Visitor<TVisitorResult> visitor) { return visitor.Visit(this); }"
                    )
                );
            }
        }

        output.WriteLine(new Verbatim("public abstract TVisitorResult Accept<TVisitorResult>(Visitor<TVisitorResult> visitor);"));
        output.WriteLine(new Verbatim("public abstract class Visitor<TVisitorResult>"));
        using (output.OpenBlock())
        {
            foreach (var record in InnerRecords)
            {
                using (output.StartLine())
                {
                    output.Write("public abstract TVisitorResult Visit(");
                    new Type(record).Write(output);
                    output.Write(" ");
                    output.Write(record.Name.ToLowerFirstCharacter());
                    output.Write(");");
                }
            }
        }
    }

    private IEnumerable<INamedTypeSymbol> InnerRecords => varyRecord.GetTypeMembers().Where(type => type.IsRecord);
}