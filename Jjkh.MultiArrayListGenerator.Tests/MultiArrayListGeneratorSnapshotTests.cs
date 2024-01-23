using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using VerifyTests;
using System.Runtime.CompilerServices;

namespace Jjkh.MultiArrayListGenerator.Tests
{
    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init()
        {
            VerifySourceGenerators.Initialize();
        }
    }

    [TestClass]
    public class MultiArrayListGeneratorSnapshotTests : VerifyBase
    {
        public static GeneratorDriver VerifySource(string source)
        {
            // Parse the provided string into a C# syntax tree
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

            // Create a Roslyn compilation for the syntax tree.
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree });


            // Create an instance of our EnumGenerator incremental source generator
            var generator = new Generators.MultiArrayListGenerator();

            // The GeneratorDriver is used to run our generator against a compilation
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            // Run the source generator!
            return driver.RunGenerators(compilation);
        }

        [TestMethod]
        public Task SimpleStruct()
        {
            var source = @"
using Jjkh.Generators;

namespace TestNamespace;

public class TestClass 
{
    [MultiArrayList]
    public struct Xxx
    {
        string X;
        bool Y;
    }
}";
            return Verify(VerifySource(source));
        }
    }
}