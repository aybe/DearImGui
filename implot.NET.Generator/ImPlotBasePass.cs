using CppSharp.Passes;
using im.NET.Generator;

namespace implot.NET.Generator;

internal abstract class ImPlotBasePass : TranslationUnitPass
{
    protected const string Indent = "    "; // Microsoft Visual Studio Debug Console sucks

    protected ImPlotBasePass(GeneratorType generatorType)
    {
        GeneratorType = generatorType;
    }

    protected GeneratorType GeneratorType { get; }

    protected static ConsoleColorScope GetConsoleColorScope(ConsoleColor? backgroundColor = ConsoleColor.Red, ConsoleColor? foregroundColor = ConsoleColor.White)
    {
        return new ConsoleColorScope(backgroundColor, foregroundColor);
    }
}