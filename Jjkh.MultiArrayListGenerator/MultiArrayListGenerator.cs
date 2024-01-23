using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Jjkh.Generators;

[Generator]
public class MultiArrayListGenerator : IIncrementalGenerator
{
    private readonly record struct MultiArrayListToGenerate
    {
        public enum ContainerTypeKind { Class, Struct }

        public readonly string NamespaceName;
        public readonly string ContainerName;
        public readonly string Visibility;
        public readonly ContainerTypeKind ContainerType;
        public readonly string ContainerVisibility;
        public readonly string FullName;
        public readonly string Name;
        public readonly EquatableDictionary<string, string> Fields;

        public MultiArrayListToGenerate(
            string namespaceName,
            string containerName,
            Accessibility containerAccessibility,
            ContainerTypeKind containerType,
            string fullName,
            Accessibility accessibility,
            Dictionary<string, string> fields)
        {
            NamespaceName = namespaceName;
            ContainerName = containerName;
            ContainerVisibility = AccessibilityToString(containerAccessibility);
            ContainerType = containerType;
            Visibility = AccessibilityToString(accessibility);
            FullName = fullName;
            var lastDot = fullName.LastIndexOf('.');
            if (lastDot == -1)
                Name = fullName;
            else
                Name = fullName.Substring(lastDot + 1);
            Fields = new(fields);
        }

        static string AccessibilityToString(Accessibility accessibility) => accessibility switch
        {
            Accessibility.NotApplicable => "",
            Accessibility.Public => "public",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.ProtectedAndInternal => "private protected",
            _ => "private",
        };

        public SourceText ToSourceText()
        {
            string className = $"{Name}MultiArray";
            string interfaceName = $"I{className}";

            var sb = new StringBuilder();

            sb.Append($@"using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace {NamespaceName}
{{
    {ContainerVisibility} partial {ContainerType.ToString().ToLowerInvariant()} {ContainerName}
    {{
        {Visibility} interface {interfaceName}: IList<{Name}>
        {{");

            foreach (var field in Fields)
            {
                sb.Append($@"
            ReadOnlyCollection<{field.Value}> {field.Key} {{ get; }}");
            }
            sb.Append($@"
        }}

        {Visibility} class {className} : {interfaceName}
        {{");
            foreach (var field in Fields)
            {
                sb.Append($@"
            readonly List<{field.Value}> _{field.Key};
            public ReadOnlyCollection<{field.Value}> {field.Key} {{ get; }}");
            }

            sb.Append($@"

            public int Count => {Fields.First().Key}.Count;
            public bool IsReadOnly => false;

            public {Name} this[int index]
            {{
                get => new {Name} {{");
            foreach (var field in Fields)
            {
                sb.Append($@"
                    {field.Key} = {field.Key}[index],");
            }
            sb.Append($@"
                }};
                set
                {{");
            foreach (var field in Fields)
            {
                sb.Append($@"
                    _{field.Key}[index] = value.{field.Key};");
            }
            sb.Append($@"
                }}
            }}

            public {className}(IEnumerable<{Name}> data)
            {{
                int count = (data as ICollection<{Name}>)?.Count ?? 0;");
            foreach (var field in Fields)
            {
                sb.Append($@"
                _{field.Key} = new List<{field.Value}>(count);");
            }
            sb.Append($@"

                foreach (var datum in data)
                {{");
            foreach (var field in Fields)
            {
                sb.Append($@"
                    _{field.Key}.Add(datum.{field.Key});");
            }
            sb.AppendLine($@"
                }}");
            foreach (var field in Fields)
            {
                sb.Append($@"
                {field.Key} = _{field.Key}.AsReadOnly();");
            }
            sb.Append($@"
            }}

            public int IndexOf({Name} item)
            {{
                for (int i = 0; i < Count; i++)
                {{
                    if (");
            {
                bool firstItem = true;
                foreach (var field in Fields)
                {
                    if (firstItem)
                    {
                        firstItem = false;
                    }
                    else
                    {
                        sb.Append($@"
                        && ");
                    }
                    sb.Append($@"{field.Key}[i] == item.{field.Key}");

                }
            }
            sb.Append($@")
                        return i;
                }}
                return -1;
            }}

            public void Insert(int index, {Name} item)
            {{");
            foreach (var field in Fields)
            {
                sb.Append($@"
                _{field.Key}.Insert(index, item.{field.Key});");
            }
            sb.Append($@"
            }}

            public void RemoveAt(int index)
            {{");
            foreach (var field in Fields)
            {
                sb.Append($@"
                _{field.Key}.RemoveAt(index);");
            }
            sb.Append($@"
            }}

            public void Add({Name} item)
            {{");
            foreach (var field in Fields)
            {
                sb.Append($@"
                _{field.Key}.Add(item.{field.Key});");
            }
            sb.Append($@"
            }}

            public void Clear()
            {{");
            foreach (var field in Fields)
            {
                sb.Append($@"
                _{field.Key}.Clear();");
            }
            sb.Append($@"
            }}

            public bool Contains({Name} item)
            {{
                return IndexOf(item) != -1;
            }}

            public void CopyTo({Name}[] array, int arrayIndex)
            {{
                for (int i = 0; i < Count; i++)
                    array[arrayIndex+i] = this[i];
            }}

            public bool Remove({Name} item)
            {{
                int index = IndexOf(item);
                if (index == -1)
                    return false;

                RemoveAt(index);
                return true;
            }}

            public IEnumerator<{Name}> GetEnumerator()
            {{
                for (int i = 0; i < Count; i++)
                    yield return this[i];
            }}

            IEnumerator IEnumerable.GetEnumerator()
            {{
                return GetEnumerator();
            }}
        }}
    }}
}}");
            return SourceText.From(sb.ToString(), Encoding.UTF8);
        }
    }

    const string masAttribute = @"
namespace Jjkh.Generators
{
    [System.AttributeUsage(System.AttributeTargets.Struct | System.AttributeTargets.Class)]
    public class MultiArrayListAttribute : System.Attribute
    {
    }
}";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "MultiArrayListAttribute.g.cs", SourceText.From(masAttribute, Encoding.UTF8)));

        // Do a simple filter for enums
        IncrementalValuesProvider<MultiArrayListToGenerate?> multiArrayListsToGenerate = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "Jjkh.Generators.MultiArrayListAttribute",
                predicate: static (s, _) => true,
                transform: static (ctx, _) => GetMultiArrayListToGenerate(ctx.SemanticModel, ctx.TargetNode))
            .Where(static m => m is not null); // Filter out errors that we don't care about

        context.RegisterSourceOutput(multiArrayListsToGenerate,
            static (spc, source) => Execute(source, spc));
    }

    static void Execute(MultiArrayListToGenerate? multiArrayListToGenerate, SourceProductionContext context)
    {
        if (multiArrayListToGenerate is { } value)
        {
            // generate the source code and add it to the output
            context.AddSource($"MultiArrayList.{value.FullName}.g.cs", value.ToSourceText());
        }
    }

    static MultiArrayListToGenerate? GetMultiArrayListToGenerate(SemanticModel semanticModel, SyntaxNode structOrClassDeclarationSyntax)
    {
        if (semanticModel.GetDeclaredSymbol(structOrClassDeclarationSyntax) is not INamedTypeSymbol structOrClassSymbol)
            return null;

        if (structOrClassSymbol.ContainingSymbol is not INamedTypeSymbol containerSymbol)
            return null;

        MultiArrayListToGenerate.ContainerTypeKind? containerType = containerSymbol.TypeKind switch
        {
            TypeKind.Class => MultiArrayListToGenerate.ContainerTypeKind.Class,
            TypeKind.Struct => MultiArrayListToGenerate.ContainerTypeKind.Struct,
            _ => null
        };
        if (containerType is null)
            return null;

        string namespaceName = containerSymbol.ContainingNamespace.Name;
        string containerName = containerSymbol.Name;
        
        string structOrClassName = structOrClassSymbol.ToString();
        var structOrClassMembers = structOrClassSymbol.GetMembers();
        

        var fields = new Dictionary<string, string>(structOrClassMembers.Length);
        foreach (ISymbol member in structOrClassMembers)
        {
            if (member is IFieldSymbol field && 
                (member.Kind == SymbolKind.Property || member.Kind == SymbolKind.Field))
            {
                fields.Add(field.Name, field.Type.Name);
            }
        }
        return new MultiArrayListToGenerate(
            namespaceName, 
            containerName,
            containerSymbol.DeclaredAccessibility,
            containerType.Value, 
            structOrClassName,
            structOrClassSymbol.DeclaredAccessibility,
            fields);
    }
}
