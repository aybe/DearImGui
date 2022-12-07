using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using im.NET.Generator;
using im.NET.Generator.Passes;
using implot.NET.Generator.Passes;

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

        module.OutputNamespace = "implot.NET";
        module.IncludeDirs.Add(@"..\..\..\..\imgui\imgui");
        module.IncludeDirs.Add(@"..\..\..\..\implot\implot");
        module.Defines.Add("IMGUI_DISABLE_OBSOLETE_FUNCTIONS");
        module.Defines.Add("IMGUI_DISABLE_OBSOLETE_KEYIO");
        module.Defines.Add("IMPLOT_DISABLE_OBSOLETE_FUNCTIONS");
        module.Headers.Add("implot.h");

        module.IncludeDirs.Add(@"..\..\..\..\implot");
        module.Headers.Add("implot_generics.h");
    }

    public override void SetupPasses(Driver driver)
    {
        driver.AddTranslationUnitPass(new ImIgnoreImGuiPass());

        AddDefaultPasses(driver);

        driver.AddTranslationUnitPass(new ImPlotSummaryPass());

        driver.Generator.OnUnitGenerated += OnUnitGenerated;
    }

    public override void Preprocess(Driver driver, ASTContext ctx)
    {
        PreprocessPasses(driver);
        PreprocessValueTypes(ctx);
        PreprocessGenericMethods(ctx);
    }

    public override void Postprocess(Driver driver, ASTContext ctx)
    {
        PostprocessProperties(ctx);
        PostprocessEnumerations(ctx);
        PostprocessNamespaces(ctx);
    }

    private static void PostprocessNamespaces(ASTContext ctx)
    {
        var unit = GetImPlotTranslationUnit(ctx);

        PushDeclarationUpstream(unit, "ImPlot");
    }

    private static TranslationUnit GetImPlotTranslationUnit(ASTContext ctx)
    {
        return ctx.TranslationUnits.Single(s => s.FileName is "implot.h");
    }

    #region Preprocess

    private static void PreprocessGenericMethods(ASTContext ctx)
    {
        var target = GetImPlotTranslationUnit(ctx);

        var targetNamespace = target.Namespaces.Single(s => s.Name is "ImPlot");

        // ignore generic functions that are to be incorrectly generated (exports don't exist)

        var functions = targetNamespace.Declarations
            .OfType<Function>()
            .Where(s => s.Name.StartsWith("Plot") && s.Parameters.Any(t => t.Type is PointerType { Pointee: TemplateParameterType }));

        foreach (var function in functions)
        {
            function.ExplicitlyIgnore();
        }

        // move the overloads we've generated to the namespace where other functions are

        var source = ctx.TranslationUnits.Single(s => s.FileName is "implot_generics.h");

        var sourceNamespace = source.Namespaces.Single();

        var sourceDeclarations = sourceNamespace.Declarations;

        foreach (var declaration in sourceDeclarations)
        {
            declaration.Namespace = targetNamespace;
        }

        targetNamespace.Declarations.AddRange(sourceDeclarations);

        sourceDeclarations.Clear();
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
        // there is also stuff in T4 templates about that
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

                // these typedefs aren't inferred, add some usings

                var usings = generator.FindBlock(BlockKind.Usings);

                var text = usings.Text;

                text.WriteLine("using ImS8  = System.SByte;");
                text.WriteLine("using ImU8  = System.Byte;");
                text.WriteLine("using ImS16 = System.Int16;");
                text.WriteLine("using ImU16 = System.UInt16;");
                text.WriteLine("using ImS32 = System.Int32;");
                text.WriteLine("using ImU32 = System.UInt32;");
                text.WriteLine("using ImS64 = System.Int64;");
                text.WriteLine("using ImU64 = System.UInt64;");
            }
        }
    }

    #endregion
}