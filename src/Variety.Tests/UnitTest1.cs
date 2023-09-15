using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Variety.Tests;

public class Tests
{
    [Test]
    public void Test1()
    {
        // Parse the provided string into a C# syntax tree
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(@"namespace Variety; [Vary] internal record Abc { public record B; }");

        IEnumerable<PortableExecutableReference> references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };
        
        // Create a Roslyn compilation for the syntax tree.
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references
        );


        // Create an instance of our EnumGenerator incremental source generator
        var generator = new Generator();

        // The GeneratorDriver is used to run our generator against a compilation
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the source generator!
        driver = driver.RunGenerators(compilation);
        
        
    }
}