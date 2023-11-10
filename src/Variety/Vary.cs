using Listing.Code;
using Listing.Contents;
using Microsoft.CodeAnalysis;
using Type = Listing.Contents.Type;

namespace Variety;

internal sealed class Vary : IContent
{
    private readonly INamedTypeSymbol varyRecord;

    public Vary(INamedTypeSymbol varyRecord)
    {
        this.varyRecord = varyRecord;
    }

    public string Name => $"{Type}.g.cs".Replace('<', '(').Replace('>', ')');

    private Type Type => new(varyRecord, verbatimPrefix: false, globalPrefix: false);

    public void Write(Output output)
    {
        var part = new Part(new Verbatim("abstract"), varyRecord, baseType: null);
        using (part.Open(output))
        {
            output.WriteLine("public abstract TVisitorResult Accept<TVisitorResult>(Visitor<TVisitorResult> visitor);".AsContent());
            output.WriteLine(new Empty());
            output.WriteLine(new Verbatim("public abstract class Visitor<TVisitorResult>"));
            using (output.OpenBlock())
            {
                foreach (var record in InnerRecords)
                {
                    using (output.StartLine())
                    {
                        output.Write("public abstract TVisitorResult Visit(");
                        output.Write(new Type(record));
                        output.Write(" @");
                        output.Write(record.Name.ToLowerFirstCharacter());
                        output.Write(");");
                    }
                }
            }
        }

        foreach (var innerRecord in InnerRecords)
        {
            var innerPart = new Part(additionalModifiers: null, innerRecord, baseType: new Type(varyRecord));
            using (innerPart.Open(output))
            {
                output.WriteLine(
                    "public override TVisitorResult Accept<TVisitorResult>(Visitor<TVisitorResult> visitor) { return visitor.Visit(this); }".AsContent()
                );
            }
        }
    }

    private IEnumerable<INamedTypeSymbol> InnerRecords => varyRecord.GetTypeMembers()
        .Where(type => type.IsRecord && type.IsReferenceType);
}