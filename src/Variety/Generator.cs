using System.Text;
using Listing;
using Listing.Contents;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Variety;

[Generator]
public sealed class Generator : IIncrementalGenerator
{
    private const string Attributes = @"
namespace Variety
{
    [global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = false)]
    internal sealed class VaryAttribute : global::System.Attribute
    {
    }
}";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(
            static context => context.AddSource(
                "VaryAttribute.g.cs",
                SourceText.From(Attributes, Encoding.UTF8)
            )
        );

        var resultRecords = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is RecordDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: (syntaxContext, token) =>
                {
                    token.ThrowIfCancellationRequested();
                    var declaration = (RecordDeclarationSyntax)syntaxContext.Node;
                    var semantic = syntaxContext.SemanticModel;
                    return (Declaration: declaration, Attributed: HasAttribute(declaration, semantic));
                }
            ).Where(pair => pair.Attributed)
            .Select((pair, _) => pair.Declaration)
            .Collect();

        var data = resultRecords.Combine(context.CompilationProvider);

        context.RegisterSourceOutput(
            data,
            (sourceContext, entry) =>
            {
                var (records, compilation) = entry;
                foreach (var record in records)
                {
                    var semantic = compilation.GetSemanticModel(record.SyntaxTree);
                    if (semantic.GetDeclaredSymbol(record) is INamedTypeSymbol symbol)
                    {
                        if (!symbol.IsReferenceType)
                        {
                            var diagnostic = Diagnostic.Create(
                                Diagnostics.VaryRecordMustBeReferenceType,
                                location: record.Keyword.GetLocation(),
                                messageArgs: symbol.ToDisplayString()
                            );
                            sourceContext.ReportDiagnostic(diagnostic);
                        }
                        else
                        {
                            var vary = new Vary(symbol);
                            sourceContext.AddSource(vary.Name, vary);
                        }
                    }
                }
            }
        );
    }

    private static bool HasAttribute(MemberDeclarationSyntax declaration, SemanticModel semantic) 
        => declaration.AttributeLists
            .SelectMany(list => list.Attributes)
            .Any(attributeSyntax =>
            {
                if (semantic.GetSymbolInfo(attributeSyntax).Symbol is IMethodSymbol attributeConstructor)
                {
                    var attributeType = attributeConstructor.ContainingType;
                    var attributeName = attributeType.ToDisplayString();
                    return attributeName == "Variety.VaryAttribute";
                }

                return false;
            });
}