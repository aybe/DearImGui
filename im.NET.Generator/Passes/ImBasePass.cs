using CppSharp.Passes;
using im.NET.Generator.Logging;

namespace im.NET.Generator.Passes;

public abstract class ImBasePass : TranslationUnitPass
{
    protected const string Indent = "    "; // Microsoft Visual Studio Debug Console sucks

    protected static ConsoleColorScope GetConsoleColorScope(ConsoleColor? backgroundColor = null, ConsoleColor? foregroundColor = ConsoleColor.Yellow)
    {
        return new ConsoleColorScope(backgroundColor, foregroundColor);
    }
}