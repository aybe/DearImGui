using System.Collections.Immutable;
using System.Diagnostics;
using CppSharp;

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
    ///     The set of namespaces used.
    /// </summary>
    public abstract ImmutableSortedSet<string> Namespaces { get; }

    /// <summary>
    ///     The set of classes to be renamed.
    /// </summary>
    public abstract ImmutableSortedSet<KeyValuePair<string, string>> Classes { get; }

    /// <summary>
    ///     The set of aliases to be renamed.
    /// </summary>
    public abstract ImmutableSortedSet<Type> Aliases { get; }

    private string ModuleName { get; }

    private string ModulePath { get; }

    private string ModuleText { get; }

    public static void Run(ILibrary library, ConsoleGenerator generator)
    {
        if (Debugger.IsAttached) // cleanup garbage
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();
        }

        ConsoleDriver.Run(library);

        generator.Process();

        Console.WriteLine("Generation finished.");
    }

    public void Process()
    {
        var text = File.ReadAllText(ModulePath);

        Process(ref text);

        Write(text);
    }

    protected virtual void Process(ref string text)
    {
        ProcessClasses(ref text);
        ProcessNamespaces(ref text);
        ProcessAliases(ref text);
    }

    private void ProcessAliases(ref string input)
    {
        foreach (var item in Aliases)
        {
            input = input.Replace($"__{item.Name}", item.Name);
        }
    }

    private void ProcessClasses(ref string input)
    {
        foreach (var (key, val) in Classes)
        {
            input = input.Replace($"class {key}", $"class {val}");
            input = input.Replace($"{key}()", $"{val}()");
            input = input.Replace($"{key}._", $"{val}._");
        }
    }

    private void ProcessNamespaces(ref string input)
    {
        foreach (var item in Namespaces.Reverse())
        {
            input = input.Replace($"global::{item}.", string.Empty);
        }
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