using System.Collections.Immutable;

namespace im.NET.Generator;

public abstract class GeneratorBase
{
    protected GeneratorBase(string moduleName, string? directory = null)
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

    public void Process()
    {
        var text = File.ReadAllText(ModulePath);

        Process(ref text);

        Write(text);
    }

    protected abstract void Process(ref string text);

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