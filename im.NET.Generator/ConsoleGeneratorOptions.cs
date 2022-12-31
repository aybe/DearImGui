using System.Runtime.InteropServices;
using CommandLine;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp;

namespace im.NET.Generator;

public sealed class ConsoleGeneratorOptions
{
    [Option('d', "dir", Required = true, HelpText = "Output directory.")]
    public string Directory { get; [UsedImplicitly] set; } = null!;

    [Option('c', "cpu", Required = true, HelpText = "Output architecture.")]
    public Architecture Architecture { get; [UsedImplicitly] set; }

    public static void Generate(Action<ConsoleGeneratorOptions> action)
    {
        const string args1 = @"--cpu X86 --dir .\x86";
        const string args2 = @"--cpu X64 --dir .\x64";

        foreach (var args in new[] { args1, args2 })
        {
            Console.WriteLine($@"Generating with arguments ""{args}""");

            var temp = args.Split();

            Parser
                .Default
                .ParseArguments<ConsoleGeneratorOptions>(temp)
                .WithParsed(action);
        }
    }

    public static void Rewrite(string csPath32, string csPath64, string csPathAnyCpu)
    {
        if (string.IsNullOrWhiteSpace(csPath32))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(csPath32));

        if (string.IsNullOrWhiteSpace(csPath64))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(csPath64));

        if (string.IsNullOrWhiteSpace(csPathAnyCpu))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(csPathAnyCpu));

        var tree32 = CSharpSyntaxTree.ParseText(File.ReadAllText(csPath32));
        var tree64 = CSharpSyntaxTree.ParseText(File.ReadAllText(csPath64));

        var root32 = tree32.GetRoot();
        var root64 = tree64.GetRoot();

        var rewriter = new ConsoleGeneratorRewriter(root32, root64);

        var visit = rewriter.Visit(root32);

        File.WriteAllText(csPathAnyCpu, visit.ToFullString());
    }
}