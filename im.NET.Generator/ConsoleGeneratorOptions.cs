using System.Runtime.InteropServices;
using CommandLine;
using JetBrains.Annotations;

namespace im.NET.Generator;

public sealed class ConsoleGeneratorOptions
{
    [Option('d', "dir", Required = true, HelpText = "Output directory.")]
    public string Directory { get; [UsedImplicitly] set; } = null!;

    [Option('c', "cpu", Required = true, HelpText = "Output architecture.")]
    public Architecture Architecture { get; [UsedImplicitly] set; }

    public static void SetDefaultArgumentsIfEmpty(ref string[] args)
    {
        if (args.Length is not 0)
        {
            return;
        }

        var arc = (Environment.Is64BitProcess ? Architecture.X64 : Architecture.X86).ToString();

        args = new[] { "--cpu", arc, "--dir", $".\\{arc}" };
    }
}