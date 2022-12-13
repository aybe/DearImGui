using System.Collections.Immutable;
using CppSharp;
using CppSharp.AST;
using im.NET.Generator;
using im.NET.Generator.Passes;
using implot.NET.Generator.Passes;

// ReSharper disable IdentifierTypo

namespace implot.NET.Generator;

internal sealed class ImPlotLibrary : LibraryBase
{
    public ImmutableSortedSet<string> Namespaces { get; init; } = null!;

    #region Overrides

    public override void Setup(Driver driver)
    {
        base.Setup(driver);

        var module = driver.Options.AddModule("implot");

        module.OutputNamespace = "implot.NET";

        SetupImGui(module);

        module.IncludeDirs.Add(@"..\..\..\..\implot\implot");
        module.Defines.Add("IMPLOT_DISABLE_OBSOLETE_FUNCTIONS");
        module.Headers.Add("implot.h");

        module.IncludeDirs.Add(@"..\..\..\..\implot");
        module.Headers.Add("implot_generics.h");
    }

    public override void SetupPasses(Driver driver)
    {
        // ignore obsolete/useless + friendly naming scheme

        driver.AddTranslationUnitPass(new ImEnumPass()); // before summary!

        // for generating nice documentation

        driver.AddTranslationUnitPass(new ImPlotSummaryPass());

        // for updating usings and formatting documentation

        driver.AddGeneratorOutputPass(new ImPlotGeneratorOutputPass(Namespaces));

        // for not generating imgui stuff

        driver.AddTranslationUnitPass(new ImIgnoreImGuiPass());
    }

    #endregion

    #region Preprocess

    protected override void PreprocessValueTypes(ASTContext ctx)
    {
        base.PreprocessValueTypes(ctx);

        ctx.SetClassAsValueType("ImPlotInputMap");
        ctx.SetClassAsValueType("ImPlotPoint");
        ctx.SetClassAsValueType("ImPlotRange");
        ctx.SetClassAsValueType("ImPlotRect");
    }

    #endregion

    #region Postprocess

    protected override void PostprocessIgnores(ASTContext ctx)
    {
        Ignore(ctx, "ImPlotPoint", "Item",   IgnoreType.Property); // manual
        Ignore(ctx, "ImPlotStyle", "Colors", IgnoreType.Property); // manual
    }

    protected override void PostprocessEnumerations(ASTContext ctx)
    {
        // there is also stuff in T4 templates about that
        ctx.SetNameOfEnumWithName("ImAxis", "ImPlotAxis");

        SetEnumerationsFlags(GetImPlotTranslationUnit(ctx));
    }

    protected override void PostprocessDeclarations(ASTContext ctx)
    {
        var tu = GetImPlotTranslationUnit(ctx);

        // ignore generic functions that are to be incorrectly generated (exports don't exist)

        var ns = tu.Namespaces.Single(s => s.Name is "ImPlot");

        var functions = ns.Declarations
            .OfType<Function>()
            .Where(s => s.Name.StartsWith("Plot") && s.Parameters.Any(t => t.Type is PointerType { Pointee: TemplateParameterType }));

        foreach (var function in functions)
        {
            function.ExplicitlyIgnore();
        }

        // move the overloads we've generated to the namespace where other functions are

        var gu = ctx.TranslationUnits.Single(s => s.FileName is "implot_generics.h");

        var gd = gu.Namespaces.Single().Declarations;

        // BUG works but why heat map functions are seen thrice?

        foreach (var declaration in gd)
        {
            declaration.Namespace = tu;
        }

        tu.Declarations.AddRange(gd);

        gd.Clear();

        PushDeclarationsUpstream(tu, "ImPlot");
    }

    #endregion
}