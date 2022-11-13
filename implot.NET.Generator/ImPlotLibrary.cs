using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using im.NET.Generator;

namespace implot.NET.Generator;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
// generator.GenerateNativeConstructorsByValue(); // todo try this
internal sealed class ImPlotLibrary : ILibrary
{
    public GeneratorType GeneratorType { get; init; }

    public ImmutableSortedSet<string> Namespaces { get; init; } = null!;

    #region ILibrary Members

    public void Setup(Driver driver)
    {
        var options = driver.Options;

        options.GeneratorKind = GeneratorKind.CSharp;
        options.GenerateFinalizers = true;
#if DEBUG
        options.GenerateDebugOutput = true;
#endif
        options.MarshalCharAsManagedChar = true;
        options.Verbose = true;

        var module = options.AddModule("implot");

        module.OutputNamespace = Constants.ImPlotNamespace;
        module.IncludeDirs.Add(@"..\..\..\..\imgui\imgui");
        module.IncludeDirs.Add(@"..\..\..\..\implot\implot");
        module.Defines.Add("IMGUI_DISABLE_OBSOLETE_FUNCTIONS");
        module.Defines.Add("IMGUI_DISABLE_OBSOLETE_KEYIO");
        module.Headers.Add("implot.h");
    }

    public void SetupPasses(Driver driver)
    {
        driver.AddTranslationUnitPass(new ImGuiEnumPass { GeneratorType = GeneratorType });

        driver.Generator.OnUnitGenerated += OnUnitGenerated;
    }

    public void Preprocess(Driver driver, ASTContext ctx)
    {
        PreprocessNamespaces(ctx);
    }

    public void Postprocess(Driver driver, ASTContext ctx)
    {
    }

    #endregion

    private static void PreprocessNamespaces(ASTContext ctx)
    {
        // move imports class to outer scope, i.e. remove superfluous namespace

        var unit = ctx.TranslationUnits.Single(s => s.FileName is "implot.h");

        var ns = unit.Namespaces.Single(s => s.Name is "ImPlot");

        ns.Namespace.Declarations.AddRange(ns.Declarations);

        ns.Declarations.Clear();
    }

    private void OnUnitGenerated(GeneratorOutput output)
    {
        foreach (var generator in output.Outputs.Cast<CSharpSources>())
        {
            if (generator.Module.LibraryName is not "implot")
            {
                continue;
            }

            var header = generator.FindBlock(BlockKind.Header);

            header.Text.WriteLine(
                "#pragma warning disable CS0109 // The member 'member' does not hide an inherited member. The new keyword is not required");

            var usings = generator.FindBlock(BlockKind.Usings);

            usings.Text.StringBuilder.Clear();

            foreach (var item in Namespaces)
            {
                usings.Text.WriteLine($"using {item};");
            }
        }
    }
}