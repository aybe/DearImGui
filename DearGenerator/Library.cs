using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Passes;
using DearGenerator.Extensions;
using Platform = Microsoft.CodeAnalysis.Platform;

// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace DearGenerator;

public abstract class Library : ILibrary
{
    protected Library(Platform platform, string? directory = null)
    {
        Platform = platform;
        Directory = directory;
    }

    private Platform Platform { get; }

    private string? Directory { get; }

    #region Helpers

    protected static TranslationUnit GetImGuiTranslationUnit(ASTContext ctx)
    {
        return GetTranslationUnit(ctx, "imgui.h");
    }

    protected static TranslationUnit GetImPlotTranslationUnit(ASTContext ctx)
    {
        return GetTranslationUnit(ctx, "implot.h");
    }

    private static TranslationUnit GetTranslationUnit(ASTContext ctx, string fileName)
    {
        return ctx.TranslationUnits.Single(s => s.FileName == fileName);
    }

    protected static void Ignore(ASTContext ctx, string? className, string? memberName, IgnoreType ignoreType)
    {
        switch (ignoreType)
        {
            case IgnoreType.Class:
                ctx.IgnoreClassWithName(className);
                return;
            case IgnoreType.Enum:
                ctx.FindCompleteEnum(className).ExplicitlyIgnore();
                return;
            case IgnoreType.Function:
                ctx.IgnoreFunctionWithName(memberName);
                return;
            case IgnoreType.Method:
                ctx.IgnoreClassMethodWithName(className, memberName);
                return;
            case IgnoreType.Property:
                ctx.FindCompleteClass(className).Properties.Single(s => s.Name == memberName).ExplicitlyIgnore();
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(ignoreType), ignoreType, null);
        }
    }

    protected static void PushDeclarationsUpstream(TranslationUnit unit, string @namespace)
    {
        var ns = unit.Namespaces.Single(s => s.Name == @namespace);

        var declarations = ns.Declarations;

        foreach (var declaration in declarations)
        {
            declaration.Namespace = unit;

            unit.Declarations.Add(declaration);

            using (new ConsoleColorScope(null, ConsoleColor.Yellow))
            {
                Console.WriteLine($"Moved declaration {declaration} to translation unit {unit}");
            }
        }

        declarations.Clear();
    }

    private static void RemovePass<T>(Driver driver, [CallerMemberName] string memberName = null!) where T : TranslationUnitPass
    {
        var count = driver.Context.TranslationUnitPasses.Passes.RemoveAll(s => s is T);

        using (new ConsoleColorScope(null, ConsoleColor.Yellow))
        {
            Console.WriteLine($"Removed {count} passes of type {typeof(T)} in {memberName}");
        }
    }

    protected static void SetEnumerationsFlags(TranslationUnit unit)
    {
        foreach (var enumeration in unit.Enums)
        {
            if (enumeration.Name.Contains("Flags") is false)
            {
                continue;
            }

            if (enumeration.IsFlags)
            {
                continue;
            }

            enumeration.SetFlags();

            using (new ConsoleColorScope(null, ConsoleColor.Yellow))
            {
                Console.WriteLine($"Set enumeration as flags: {enumeration.Name}");
            }
        }
    }

    protected static void SetVectorParametersUsage(Namespace @namespace)
    {
        if (@namespace is null)
            throw new ArgumentNullException(nameof(@namespace));

        // some vectors should be references but currently they aren't thus we get access denied, fix

        // neither DefaultValue or HasDefaultValue have any effect so we're good for some regex again

        var regex = new Regex(@"(ImVec\d\*\sout\s=\sNULL)|(const\sImVec\d\*)", RegexOptions.Compiled | RegexOptions.Singleline);

        foreach (var function in @namespace.Functions)
        {
            foreach (var parameter in function.Parameters)
            {
                var match = regex.Match(parameter.DebugText);

                if (!match.Success)
                    continue;

                var groups = match.Groups;

                switch (match.Success)
                {
                    case true when groups[1].Success:
                        parameter.Usage = ParameterUsage.Out;
                        break;
                    case true when groups[2].Success:
                        parameter.Usage = ParameterUsage.InOut;
                        break;
                    default:
                        continue;
                }

                Console.WriteLine($"Changed usage to '{parameter.Usage}' for parameter '{parameter}' in function '{function}'.");
            }
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    protected static void SetupImGui(Module module)
    {
        module.IncludeDirs.Add(@"..\..\..\..\imgui\imgui");
        module.Defines.Add("IMGUI_DISABLE_OBSOLETE_FUNCTIONS");
        module.Defines.Add("IMGUI_DISABLE_OBSOLETE_KEYIO");
        module.Headers.Add("imgui.h");
    }

    #endregion

    #region Preprocess

    [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
    protected virtual void PreprocessPasses(Driver driver)
    {
        // actually, we do want these, else we'll get pretty much nothing generated

        RemovePass<CheckIgnoredDeclsPass>(driver);

        // this is useless in our case, it also throws when adding our own comments

        RemovePass<CleanCommentsPass>(driver);
    }

    protected virtual void PreprocessValueTypes(ASTContext ctx)
    {
        ctx.SetClassAsValueType("ImDrawCmd");
        ctx.SetClassAsValueType("ImDrawData");
        ctx.SetClassAsValueType("ImDrawList");
        ctx.SetClassAsValueType("ImDrawVert");
        ctx.SetClassAsValueType("ImVec2");
        ctx.SetClassAsValueType("ImVec4");
    }

    protected virtual void PreprocessParameters(ASTContext ctx)
    {
    }

    [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
    protected virtual void PreprocessIgnores(ASTContext ctx)
    {
        Ignore(ctx, "ImGuiContext", null, IgnoreType.Class); // IntPtr

        Ignore(ctx, "ImColor",    null, IgnoreType.Class); // unused
        Ignore(ctx, "ImDrawVert", null, IgnoreType.Class); // manual
        Ignore(ctx, "ImVec2",     null, IgnoreType.Class); // manual
        Ignore(ctx, "ImVec4",     null, IgnoreType.Class); // manual
        Ignore(ctx, "ImVector",   null, IgnoreType.Class); // manual

        Ignore(ctx, "ImGuiModFlags_", null, IgnoreType.Enum); // useless
        Ignore(ctx, "ImGuiNavInput_", null, IgnoreType.Enum); // useless

        Ignore(ctx, "ImDrawCmd",   "GetTexID",       IgnoreType.Method); // manual
        Ignore(ctx, "ImDrawList",  "GetClipRectMax", IgnoreType.Method); // manual
        Ignore(ctx, "ImDrawList",  "GetClipRectMin", IgnoreType.Method); // manual
        Ignore(ctx, "ImFont",      "GetDebugName",   IgnoreType.Method); // manual
        Ignore(ctx, "ImFont",      "IsLoaded",       IgnoreType.Method); // manual
        Ignore(ctx, "ImFontAtlas", "SetTexID",       IgnoreType.Method); // manual

        Ignore(ctx, null, "IM_DELETE", IgnoreType.Function); // unused
    }

    #endregion

    #region Postprocess

    protected virtual void PostprocessIgnores(ASTContext ctx)
    {
        // ignore useless ImVector stuff, removes ~1K LOC

        var imVectorClass = ctx.FindCompleteClass("ImVector");

        imVectorClass.Methods.ForEach(s => s.ExplicitlyIgnore());

        imVectorClass.Specializations.ForEach(s => s.ExplicitlyIgnore());
    }

    protected virtual void PostprocessEnumerations(ASTContext ctx)
    {
    }

    protected virtual void PostprocessDeclarations(ASTContext ctx)
    {
    }

    protected virtual void PostprocessProperties(ASTContext ctx)
    {
    }

    #endregion

    #region ILibrary Members

    public virtual void Setup(Driver driver)
    {
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        driver.ParserOptions.TargetTriple = Platform switch
        {
            Platform.X86 => "i686-pc-win32-msvc",
            Platform.X64 => "x86_64-pc-win32-msvc",
            _            => throw new NotSupportedException(Platform.ToString())
        };

        var options = driver.Options;

        options.OutputDir = Directory ?? Path.Combine(Environment.CurrentDirectory, Platform.ToString());
        options.GenerateDebugOutput = true;
        options.GeneratorKind = GeneratorKind.CSharp;
        options.GenerateFinalizers = true;
        options.GenerateDefaultValuesForArguments = true;
        options.MarshalCharAsManagedChar = true;
        options.UseSpan = true;
        options.Verbose = true;
    }

    public abstract void SetupPasses(Driver driver);

    public virtual void Preprocess(Driver driver, ASTContext ctx)
    {
        PreprocessPasses(driver);
        PreprocessValueTypes(ctx);
        PreprocessParameters(ctx);
        PreprocessIgnores(ctx);
    }

    public virtual void Postprocess(Driver driver, ASTContext ctx)
    {
        PostprocessIgnores(ctx);
        PostprocessEnumerations(ctx);
        PostprocessDeclarations(ctx);
        PostprocessProperties(ctx);
    }

    #endregion
}