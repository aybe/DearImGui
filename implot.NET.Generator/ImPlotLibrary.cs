using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Passes;
using im.NET.Generator;
using im.NET.Generator.Passes;

// ReSharper disable IdentifierTypo

namespace implot.NET.Generator;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
internal sealed class ImPlotLibrary : LibraryBase
{
    public override ImmutableSortedSet<string> Namespaces { get; init; } = null!;

    public override void Setup(Driver driver)
    {
        base.Setup(driver);

        var module = driver.Options.AddModule("implot");

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
        PreprocessPasses(driver);
        PreprocessNamespaces(ctx);
        PreprocessValueTypes(ctx);
    }

    public override void Postprocess(Driver driver, ASTContext ctx)
    {
        PostprocessProperties(ctx);
        PostprocessEnumerations(ctx);
    }

    private static TranslationUnit GetImPlotTranslationUnit(ASTContext ctx)
    {
        return ctx.TranslationUnits.Single(s => s.FileName is "implot.h");
    }

    #region Preprocess

    private static void PreprocessNamespaces(ASTContext ctx)
    {
        // move imports class to outer scope, i.e. remove superfluous namespace

        var unit = GetImPlotTranslationUnit(ctx);

        var ns = unit.Namespaces.Single(s => s.Name is "ImPlot");

        ns.Namespace.Declarations.AddRange(ns.Declarations);

        ns.Declarations.Clear();
    }

    private static void PreprocessPasses(Driver driver)
    {
        RemovePass<CheckIgnoredDeclsPass>(driver);
    }

    private static void PreprocessValueTypes(ASTContext ctx)
    {
        ctx.SetClassAsValueType("ImPlotInputMap");
        ctx.SetClassAsValueType("ImPlotPoint");
        ctx.SetClassAsValueType("ImPlotRange");
        ctx.SetClassAsValueType("ImPlotRect");
    }

    #endregion

    #region Postprocess

    private static void PostprocessEnumerations(ASTContext ctx)
    {
        ctx.SetNameOfEnumWithName("ImAxis", "ImPlotAxis");

        SetEnumerationsFlags(GetImPlotTranslationUnit(ctx));
    }

    private static void PostprocessProperties(ASTContext ctx)
    {
        Ignore(ctx, "ImPlotPoint", "Item",   IgnoreType.Property); // manual
        Ignore(ctx, "ImPlotStyle", "Colors", IgnoreType.Property); // manual
    }

    private void OnUnitGenerated(GeneratorOutput output)
    {
        foreach (var generator in output.Outputs.Cast<CSharpSources>())
        {
            if (generator.Module.LibraryName is "implot")
            {
                ProcessSources(generator);
            }
        }
    }

    #endregion
}