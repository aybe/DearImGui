using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using CppSharp;
using implot.NET.Generator;

if (Debugger.IsAttached) // cleanup garbage
{
    Console.BackgroundColor = ConsoleColor.Black;
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.Clear();
}

var namespaces = new SortedSet<string>
    {
        "imgui.NET",
        "implot.NET",
        "System",
        "System.Collections.Concurrent",
        "System.Numerics",
        "System.Runtime.CompilerServices",
        "System.Runtime.InteropServices",
        "System.Security",
        "System.Text"
    }
    .ToImmutableSortedSet();

var classes = new SortedSet<KeyValuePair<string, string>>
    {
        new("implot", "ImPlot")
    }
    .ToImmutableSortedSet();

var aliases = new SortedSet<Type>(TypeComparer.Instance)
    {
        typeof(CallingConvention),
        typeof(IntPtr)
    }
    .ToImmutableSortedSet(TypeComparer.Instance);

var library = new ImPlotLibrary
{
    Namespaces = namespaces
};

const string module = "implot";

var path = Path.Combine(Environment.CurrentDirectory, Path.ChangeExtension(module, "cs"));

var copy = File.Exists(path) ? File.ReadAllText(path) : string.Empty;

ConsoleDriver.Run(library);

var text = File.ReadAllText(path);

RenameClasses(ref text, classes);

RenameVectors(ref text);

ShortenQualifiers(ref text, namespaces);

RepairUsingAliases(ref text, aliases);

RepairGenericMethods(ref text);

File.WriteAllText(path, text);

if (copy != string.Empty && copy != text)
{
    // create a backup history so we can diff them
    // imgui-backup.cs -> imgui-backup-date.cs
    // imgui.cs -> imgui-backup.cs

    var backupDirectory = Path.GetDirectoryName(path) ?? string.Empty;
    var backupExtension = Path.GetExtension(path);
    var backupPath = Path.Combine(backupDirectory, Path.ChangeExtension($"{module}-backup", backupExtension));

    if (File.Exists(backupPath))
    {
        var backupTime = File.GetLastWriteTimeUtc(backupPath);
        var backupName = backupTime.ToString("O").Replace(':', '-').Replace('.', '-');
        var backupFile = Path.GetFileNameWithoutExtension(backupPath);
        var backupDest = Path.Combine(backupDirectory, Path.ChangeExtension($"{backupFile}-{backupName}", backupExtension));

        File.Copy(backupPath, backupDest, true);
    }

    File.WriteAllText(backupPath, copy);
}

Console.WriteLine("Generation finished.");

static void RepairUsingAliases(ref string input, ImmutableSortedSet<Type> aliases)
{
    foreach (var item in aliases)
    {
        input = input.Replace($"__{item.Name}", item.Name);
    }
}

static void RenameClasses(ref string input, ImmutableSortedSet<KeyValuePair<string, string>> classes)
{
    foreach (var item in classes)
    {
        input = input.Replace($"class {item.Key}", $"class {item.Value}");
    }
}

static void RenameVectors(ref string input)
{
    input = input.Replace(
        "new global::implot.NET.ImVec2.__Internal()",
        "new global::System.Numerics.Vector2()"
    );

    input = input.Replace(
        "new global::implot.NET.ImVec4.__Internal()",
        "new global::System.Numerics.Vector4()"
    );
}

static void ShortenQualifiers(ref string input, ImmutableSortedSet<string> namespaces)
{
    foreach (var item in namespaces.Reverse())
    {
        input = input.Replace($"global::{item}.", string.Empty);
    }
}

static void RepairGenericMethods(ref string input)
{
    // add generic type parameter with unmanaged constraint

    // TODO see if that can't be done upstream anyhow
    input = Regex.Replace(input,
        @"^(\s+public\s+static\s+\w+\s+\w+)(\(.*T\s+\w+.*\))",
        @"$1<T>$2 where T : unmanaged",
        RegexOptions.Multiline
    );

    // set generic type parameters as arrays

    input = Regex.Replace(input,
        @"(,\s+T)(\s+\w+)(?=.*where\s+T\s+:\s+unmanaged\s*$)",
        @"$1[]$2",
        RegexOptions.Multiline
    );
}

internal sealed class TypeComparer : Comparer<Type>
{
    public static TypeComparer Instance { get; } = new();

    public override int Compare(Type? x, Type? y)
    {
        return string.Compare(x?.Name, y?.Name, StringComparison.Ordinal);
    }
}