using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using CppSharp;
using JetBrains.Annotations;

namespace im.NET.Generator;

public abstract class ConsoleGenerator
{
    protected ConsoleGenerator(string moduleName, string? directory = null)
    {
        ModuleName = moduleName;
        ModulePath = Path.Combine(directory ?? Environment.CurrentDirectory, Path.ChangeExtension(ModuleName, "cs"));
        ModuleText = File.Exists(ModulePath) ? File.ReadAllText(ModulePath) : string.Empty;
    }

    /// <summary>
    ///     The set of aliases to be renamed.
    /// </summary>
    [PublicAPI]
    public abstract ImmutableSortedSet<Type> Aliases { get; }

    /// <summary>
    ///     The set of classes to be renamed.
    /// </summary>
    [PublicAPI]
    public abstract ImmutableSortedSet<KeyValuePair<string, string>> Classes { get; }

    /// <summary>
    ///     The set of namespaces used.
    /// </summary>
    [PublicAPI]
    public abstract ImmutableSortedSet<string> Namespaces { get; }

    private string ModuleName { get; }

    private string ModulePath { get; }

    private string ModuleText { get; }

    protected static ImmutableSortedSet<Type> GetDefaultAliases()
    {
        return new SortedSet<Type>(TypeNameComparer.Instance)
            {
                typeof(CallingConvention),
                typeof(IntPtr)
            }
            .ToImmutableSortedSet(TypeNameComparer.Instance);
    }

    protected static ImmutableSortedSet<string> GetDefaultNamespaces()
    {
        return new SortedSet<string>
            {
                "imgui.NET",
                "System",
                "System.Collections.Concurrent",
                "System.Numerics",
                "System.Runtime.CompilerServices",
                "System.Runtime.InteropServices",
                "System.Security",
                "System.Text"
            }
            .ToImmutableSortedSet();
    }

    private void Process()
    {
        var text = File.ReadAllText(ModulePath);

        Process(ref text);

        Write(text);
    }

    protected virtual void Process(ref string input)
    {
        // apply module-wide changes stuff first

        ProcessNamespaces(ref input);
        ProcessAliases(ref input);
        ProcessClasses(ref input);
        ProcessEnumerations(ref input);

        // apply fine-grained stuff

        ProcessVectors(ref input);
        ProcessPointers(ref input);
        ProcessSummaries(ref input);
        ProcessSymbols(ref input);
        ProcessVisibility(ref input);
        ProcessDefaultParameters(ref input);
    }

    private void ProcessAliases(ref string input)
    {
        foreach (var item in Aliases)
        {
            var name = item.Name;

            // remove using alias

            input = Regex.Replace(input,
                $@"^using __{name} = .*;\r?$\n",
                string.Empty,
                RegexOptions.Multiline
            );

            // remove double underscore prefix

            input = input.Replace($"__{name}", name);
        }
    }

    private void ProcessNamespaces(ref string input)
    {
        // simplify fully qualified names

        foreach (var item in Namespaces.Reverse())
        {
            input = input.Replace($"global::{item}.", string.Empty);
        }
    }

    private void ProcessClasses(ref string input)
    {
        // rename classes and references to their internals

        foreach (var (key, val) in Classes)
        {
            input = input.Replace($"class {key}", $"class {val}");
            input = input.Replace($"{key}()",     $"{val}()");
            input = input.Replace($"{key}._",     $"{val}._");
        }
    }

    protected virtual void ProcessDefaultParameters(ref string input)
    {
        // fix invalid syntax 'string[] name = 0'

        input = Regex.Replace(input,
            @"string\s*\[\]\s+(\w+)\s*=\s*0\s*,",
            "string[] $1 = null,",
            RegexOptions.Multiline
        );
    }

    private static void ProcessEnumerations(ref string input)
    {
        // enumerations default values other than zero must be cast

        input = Regex.Replace(input,
            @"(?<!(?://\s*DEBUG:|///\s*<summary>).*)(\w+)\s+(\w+)\s+=\s+([-+]?\d+)(?=[,)])",
            @"$1 $2 = ($1)($3)",
            RegexOptions.Multiline
        );

        // enumerations cast to int while there's no reason to

        input = Regex.Replace(input,
            @"(?<type>\w+)\s+(\w+)\s+=\s+\(int\)\s*\k<type>\.(\w+)",
            "${type} $1 = ${type}.$2",
            RegexOptions.Multiline
        );
    }

    protected virtual void ProcessPointers(ref string input)
    {
        // hide pointers to internal classes

        input = input.Replace(
            "public IntPtr __Instance { get; protected set; }",
            "internal IntPtr __Instance { get; set; }"
        );
    }

    protected virtual void ProcessSummaries(ref string input)
    {
        // for some reason, summaries have wrong syntax, repair

        input = input.Replace("// <summary>", "/// <summary>");

        // add few inherit doc to reduce CS1591 warnings count

        input = Regex.Replace(
            input,
            @"^(\s+)(public void Dispose\(\))",
            @"$1/// <inheritdoc />$1$2",
            RegexOptions.Multiline
        );

        input = Regex.Replace(
            input,
            @"^(\s+)(~\w+\(\))",
            @"$1/// <inheritdoc />$1$2",
            RegexOptions.Multiline
        );
    }

    private void ProcessSymbols(ref string text)
    {
        // hide public symbols that ought to be internal

        text = Regex.Replace(text,
            @"public\s+static\s+(\w+)\s+(_(?!_)\w+)",
            @"internal static $1 $2",
            RegexOptions.Multiline
        );
    }

    protected virtual void ProcessVectors(ref string input)
    {
        // type maps aren't enough to pass vectors directly

        input = Regex.Replace(input,
            @"new ImVec(\d)\.__Internal\(\)",
            @"new Vector$1()",
            RegexOptions.Multiline
        );

        // make vectors as obsolete so it's crystal clear

        input = Regex.Replace(input,
            @"public (unsafe partial struct ImVec\d)",
            @"[Obsolete(null, true)] internal $1",
            RegexOptions.Multiline
        );

        // setting value type in type maps doesn't work

        input = Regex.Replace(input,
            @"__element is null \? new .*Vector\d.*;",
            @"__element;",
            RegexOptions.Multiline
        );
    }

    protected virtual void ProcessVisibility(ref string input)
    {
        // hide internal structs and vectors

        input = Regex.Replace(input,
            @"public ((?:unsafe )?partial struct (?:__Internal|ImVec\d))",
            @"internal $1",
            RegexOptions.Multiline
        );

        // hide protected members to remove more CS1591

        input = Regex.Replace(input,
            @"(internal\s+)*protected",
            "private protected",
            RegexOptions.Multiline
        );
    }

    public static void Run(ILibrary library, ConsoleGenerator generator)
    {
        if (Debugger.IsAttached)
            // using color between runs confuses console
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();
        }

        ConsoleDriver.Run(library);

        generator.Process();

        Console.WriteLine("Generation finished.");
    }

    private void Write(string text)
    {
        File.WriteAllText(ModulePath, text);

        if (ModuleText == string.Empty)
        {
            return;
        }

        if (ModuleText == text)
        {
            return;
        }

        // create a backup history so we can diff them
        // imgui-backup.cs -> imgui-backup-date.cs
        // imgui.cs -> imgui-backup.cs

        var backupDirectory = Path.GetDirectoryName(ModulePath) ?? string.Empty;
        var backupExtension = Path.GetExtension(ModulePath);
        var backupPath = Path.Combine(backupDirectory, Path.ChangeExtension($"{ModuleName}-backup", backupExtension));

        if (File.Exists(backupPath))
        {
            var backupTime = File.GetLastWriteTimeUtc(backupPath);
            var backupName = backupTime.ToString("O").Replace(':', '-').Replace('.', '-');
            var backupFile = Path.GetFileNameWithoutExtension(backupPath);
            var backupDest = Path.Combine(backupDirectory, Path.ChangeExtension($"{backupFile}-{backupName}", backupExtension));

            File.Copy(backupPath, backupDest, true);
        }

        File.WriteAllText(backupPath, ModuleText);
    }
}