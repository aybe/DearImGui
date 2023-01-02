using System.Collections.Immutable;
using System.Text.RegularExpressions;
using CppSharp;
using CppSharp.AST;
using im.NET.Generator;
using im.NET.Generator.Extensions;
using im.NET.Generator.Passes;
using imgui.NET.Generator.Passes;
using Platform = Microsoft.CodeAnalysis.Platform;

// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace imgui.NET.Generator;

internal sealed class ImGuiLibrary : LibraryBase
{
    public ImGuiLibrary(Platform platform, string directory, ImmutableSortedSet<string> namespaces)
        : base(platform, directory)
    {
        Namespaces = namespaces;
    }

    private ImmutableSortedSet<string> Namespaces { get; }

    #region Overrides

    public override void Setup(Driver driver)
    {
        base.Setup(driver);

        var module = driver.Options.AddModule("imgui");

        module.OutputNamespace = Constants.ImGuiNamespace;

        SetupImGui(module);
    }

    public override void SetupPasses(Driver driver)
    {
        // ignore obsolete/useless + friendly naming scheme

        driver.AddTranslationUnitPass(new ImEnumPass()); // before summary!

        // for generating nice documentation

        driver.AddTranslationUnitPass(new ImGuiSummaryPass());

        // for updating usings and formatting documentation

        driver.AddGeneratorOutputPass(new ImGuiGeneratorOutputPass(Namespaces));
    }

    #endregion

    #region Postprocess

    protected override void PostprocessIgnores(ASTContext ctx)
    {
        base.PostprocessIgnores(ctx);

        Ignore(ctx, "ImDrawData",            "CmdLists",        IgnoreType.Property); // manual
        Ignore(ctx, "ImDrawList",            "ClipRectStack",   IgnoreType.Property); // intern
        Ignore(ctx, "ImDrawList",            "CmdHeader",       IgnoreType.Property); // intern
        Ignore(ctx, "ImDrawList",            "Data",            IgnoreType.Property); // intern
        Ignore(ctx, "ImDrawList",            "FringeScale",     IgnoreType.Property); // intern
        Ignore(ctx, "ImDrawList",            "IdxWritePtr",     IgnoreType.Property); // intern
        Ignore(ctx, "ImDrawList",            "OwnerName",       IgnoreType.Property); // intern
        Ignore(ctx, "ImDrawList",            "Path",            IgnoreType.Property); // intern
        Ignore(ctx, "ImDrawList",            "Splitter",        IgnoreType.Property); // intern
        Ignore(ctx, "ImDrawList",            "TextureIdStack",  IgnoreType.Property); // intern
        Ignore(ctx, "ImDrawList",            "VtxCurrentIdx",   IgnoreType.Property); // intern
        Ignore(ctx, "ImDrawList",            "VtxWritePtr",     IgnoreType.Property); // intern
        Ignore(ctx, "ImFontAtlas",           "IsBuilt",         IgnoreType.Property); // manual
        Ignore(ctx, "ImFontAtlas",           "TexUvLines",      IgnoreType.Property); // manual
        Ignore(ctx, "ImFontAtlasCustomRect", "IsPacked",        IgnoreType.Property); // manual
        Ignore(ctx, "ImFontConfig",          "Name",            IgnoreType.Property); // manual
        Ignore(ctx, "ImGuiIO",               "MouseClickedPos", IgnoreType.Property); // manual
        Ignore(ctx, "ImGuiStyle",            "Colors",          IgnoreType.Property); // manual

        Ignore(ctx, "ImVectorExtensions", null, IgnoreType.Class); // unused
    }

    protected override void PostprocessEnumerations(ASTContext ctx)
    {
        base.PostprocessEnumerations(ctx);

        var unit = GetImGuiTranslationUnit(ctx);

        SetEnumerationsFlags(unit);

        unit.FindEnum("ImGuiCond").Modifiers &= ~Enumeration.EnumModifiers.Flags;
    }

    protected override void PostprocessDeclarations(ASTContext ctx)
    {
        base.PostprocessDeclarations(ctx);

        // rename delegates to more appropriate names

        const string delegates = "Delegates";

        var tu = GetImGuiTranslationUnit(ctx);

        var ns = tu.FindNamespace(delegates);

        ns.FindTypedef("Func___IntPtr___IntPtr")
            .Name = "ImGetClipboardTextHandler";

        ns.FindTypedef("Action___IntPtr_string8")
            .Name = "ImSetClipboardTextHandler";

        ns.FindTypedef("Action___IntPtr___IntPtr")
            .Name = "ImSetPlatformImeDataHandler";

        ns.FindTypedef("Func_bool___IntPtr_int_sbytePtrPtr")
            .Name = "ImItemsGetterHandler";

        ns.FindTypedef("Func_float___IntPtr_int")
            .Name = "ImValuesGetterHandler";

        // push everything up so it ends up in ImGui class

        PushDeclarationsUpstream(tu, "ImGui");

        PushDeclarationsUpstream(tu, delegates);
    }

    protected override void PostprocessProperties(ASTContext ctx)
    {
        base.PostprocessProperties(ctx);

        // ImVector<T> properties are not meant to be assignable, make them read-only

        var unit = GetImGuiTranslationUnit(ctx);

        foreach (var c in unit.Classes)
        {
            foreach (var p in c.Properties)
            {
                if (p.QualifiedType.Type is not TemplateSpecializationType type)
                {
                    continue;
                }

                if (type.Template.Name is not "ImVector")
                {
                    continue;
                }

                ctx.SetPropertyAsReadOnly(c.Name, p.Name);

                using (new ConsoleColorScope(null, ConsoleColor.Yellow))
                {
                    Console.WriteLine($"Set ImVector<T> property as read-only: {c.Name}.{p.Name}");
                }
            }
        }

        // type mapped properties in a value type are never generated
        // this is because of nasty bugs in CppSharp, ignore & notify

        foreach (var c in unit.Classes)
        {
            if (c.GenerationKind == GenerationKind.None)
            {
                continue;
            }

            if (!c.IsValueType)
            {
                continue;
            }

            foreach (var p in c.Properties)
            {
                var type = p.QualifiedType.ToString();

                if (!Regex.IsMatch(type, @"^global::System\.Numerics\.Vector\d$"))
                {
                    continue;
                }

                p.ExplicitlyIgnore();

                using (new ConsoleColorScope(null, ConsoleColor.Red))
                {
                    Console.WriteLine($"Type mapped property in value type explicitly ignored (TODO implement manually): {c.Name}.{p.Name}");
                }
            }
        }
    }

    #endregion
}