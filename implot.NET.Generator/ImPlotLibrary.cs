using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Passes;
using im.NET.Generator;
using im.NET.Generator.Passes;

namespace implot.NET.Generator;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
internal sealed class ImPlotLibrary : LibraryBase
{
    public ImmutableSortedSet<string> Namespaces { get; init; } = null!;

    public override void Setup(Driver driver)
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

    public override void SetupPasses(Driver driver)
    {
        driver.AddTranslationUnitPass(new ImGuiIgnorePass());
        driver.AddTranslationUnitPass(new ImEnumPass());

        driver.Generator.OnUnitGenerated += OnUnitGenerated;
    }

    public override void Preprocess(Driver driver, ASTContext ctx)
    {
        RemovePass<CheckIgnoredDeclsPass>(driver);
        PreprocessNamespaces(ctx);
        
        ctx.SetClassAsValueType("ImPlotInputMap");
        ctx.SetClassAsValueType("ImPlotPoint");
        ctx.SetClassAsValueType("ImPlotRange");
        ctx.SetClassAsValueType("ImPlotRect");
    }

    public override void Postprocess(Driver driver, ASTContext ctx)
    {
        Ignore(ctx, "ImPlotPoint", "Item",   IgnoreType.Property); // manual
        Ignore(ctx, "ImPlotStyle", "Colors", IgnoreType.Property); // manual

        ctx.SetNameOfEnumWithName("ImAxis", "ImPlotAxis");

        SetEnumerationsFlags(GetImPlotTranslationUnit(ctx));
    }

    private static TranslationUnit GetImPlotTranslationUnit(ASTContext ctx)
    {
        return ctx.TranslationUnits.Single(s => s.FileName is "implot.h");
    }

    private static void PreprocessNamespaces(ASTContext ctx)
    {
        // move imports class to outer scope, i.e. remove superfluous namespace

        var unit = GetImPlotTranslationUnit(ctx);

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