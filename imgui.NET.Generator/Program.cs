using System.Text;
using System.Text.RegularExpressions;
using CppSharp;
using im.NET.Generator;
using imgui.NET.Generator.Logging;

// ReSharper disable StringLiteralTypo

namespace imgui.NET.Generator;

internal static class Program
{
    private static void Main(string[] args)
    {
        if (args.Length is 0)
        {
            Console.WriteLine("No arguments given, available arguments:");
            Console.WriteLine("\t-old: generate old version");
            Console.WriteLine("\t-new: generate new version");
            Console.WriteLine("\t-cln: cleaning new version");
            return;
        }

        Console.WriteLine("Code generation started.");

        if (args.Contains("-old", StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine("Generating old version.");

            Directory.CreateDirectory("OLD");

            using (var writer = new StreamWriter(File.Create(@"OLD\output.txt")))
            using (new AggregateConsoleOut(writer))
            {
                ConsoleDriver.Run(new MyLibrary { Enhanced = false });
            }
        }

        if (args.Contains("-new", StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine("Generating new version.");

            Directory.CreateDirectory("NEW");

            using (var writer = new StreamWriter(File.Create(@"NEW\output.txt")))
            using (new AggregateConsoleOut(writer))
            {
                ConsoleDriver.Run(new MyLibrary { Enhanced = true });
            }
        }

        if (args.Contains("-cln", StringComparer.OrdinalIgnoreCase))
        {
            Cleanup(Path.Combine(Environment.CurrentDirectory, "NEW", "imgui.cs"));

            Cleanup(Path.Combine(Environment.CurrentDirectory, "OLD", "imgui.cs"));
        }

        Console.WriteLine("Code generation finished.");
    }

    private static void Cleanup(string path)
    {
        if (!File.Exists(path))
            return;

        // GeneratorOutputPass misses some structs for whatever reason so let's do it all here

        var text = File.ReadAllText(path);

        var builder = new StringBuilder(text);

        const string @namespace = Constants.ImGuiNamespace;

        // rename imgui class and fix references to it

        builder.Replace(
            "class imgui",
            "class ImGui"
        );

        builder.Replace(
            "imgui()",
            "ImGui()"
        );

        builder.Replace(
            "imgui._",
            "ImGui._"
        );

        // hide pointers that should have been internal

        builder.Replace(
            "public __IntPtr __Instance { get; protected set; }",
            "internal __IntPtr __Instance { get; set; }"
        );

        // hide structs that should have been internal

        builder.Replace(
            "public partial struct __Internal",
            "internal partial struct __Internal"
        );

        builder.Replace(
            "public unsafe partial struct __Internal",
            "internal unsafe partial struct __Internal"
        );

        // hide replaced vectors, their internal stuff is still used

        builder.Replace(
            "public unsafe partial struct ImVec2",
            "internal unsafe partial struct ImVec2"
        );


        builder.Replace(
            "public unsafe partial struct ImVec4",
            "internal unsafe partial struct ImVec4"
        );

        // pass vectors directly, doesn't mean that we can ditch type maps that did the heavy lifting

        builder.Replace(
            $"new global::{@namespace}.ImVec2.__Internal()",
            "new global::System.Numerics.Vector2()"
        );

        builder.Replace(
            $"new global::{@namespace}.ImVec4.__Internal()",
            "new global::System.Numerics.Vector4()"
        );

        // they're now unused in sources, let's add a little guard though

        builder.Replace(
            "internal unsafe partial struct ImVec2",
            "[global::System.Obsolete(null, true)] internal unsafe partial struct ImVec2"
        );

        builder.Replace(
            "internal unsafe partial struct ImVec4",
            "[global::System.Obsolete(null, true)] internal unsafe partial struct ImVec4"
        );

        // hide ImVector namespace as internal class as it cannot be moved onto ImVector<T> because of CS7042

        builder.Replace(
            "namespace ImVector",
            "internal static partial class ImVector"
        );

        // merge symbols with class to remove __Symbols namespace

        builder.Replace(
            $"}}\r\nnamespace {@namespace}.__Symbols\r\n{{\r\n    internal class ImGui",
            "    public unsafe partial class ImGui"
        );

        builder.Replace(
            "public static IntPtr _EmptyString_ImGuiTextBuffer__2PADA",
            "internal static IntPtr _EmptyString_ImGuiTextBuffer__2PADA"
        );

        builder.Replace(
            ".__Symbols",
            string.Empty
        );

        // type map does not appear to be thoroughly applied, fix

        builder.Replace(
            "__element is null ? new global::System.Numerics.Vector2() : *(global::System.Numerics.Vector2*) __element.__Instance",
            "__element"
        );

        // use our own symbol resolver

        builder.Replace(
            "CppSharp.SymbolResolver",
            $"{@namespace}.SymbolResolver"
        );

        // simplify namespaces

        builder.Replace(
            "global::System.Collections.Concurrent.",
            string.Empty
        );

        builder.Replace(
            "global::System.Numerics.",
            string.Empty
        );

        builder.Replace(
            "global::System.Text.",
            string.Empty
        );

        builder.Replace(
            "global::System.",
            string.Empty
        );

        builder.Replace(
            $"global::{@namespace}.",
            string.Empty
        );

        builder.Replace(
            $"{@namespace}.",
            string.Empty
        );

        var newLine = Environment.NewLine;

        builder.Replace(
            $"using __CallingConvention = Runtime.InteropServices.CallingConvention;{newLine}",
            string.Empty
        );

        builder.Replace(
            $"using __IntPtr = IntPtr;{newLine}",
            string.Empty
        );

        builder.Replace(
            "__CallingConvention",
            "CallingConvention"
        );

        builder.Replace(
            "__IntPtr",
            "IntPtr"
        );

        // XML comments are somehow wrong, fix that

        builder.Replace(
            "// <summary>",
            "/// <summary>"
        );

        var str = builder.ToString();

        // add some inherit doc

        str = Regex.Replace(
            str,
            @"^(\s+)(public void Dispose\(\))",
            @"$1/// <inheritdoc />$1$2",
            RegexOptions.Multiline
        );

        str = Regex.Replace(
            str,
            @"^(\s+)(~\w+\(\))",
            @"$1/// <inheritdoc />$1$2",
            RegexOptions.Multiline
        );

        // hide some protected members to remove more CS1591

        str = Regex.Replace(
            str,
            @"(internal\s+)*protected",
            "private protected",
            RegexOptions.Multiline
        );

        File.WriteAllText(path, str);
    }
}