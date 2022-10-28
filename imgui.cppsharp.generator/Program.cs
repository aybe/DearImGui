using CppSharp;

namespace imgui.cppsharp.generator;

internal static class Program
{
    private static void Main()
    {
        ConsoleDriver.Run(new MyLibrary());
    }
}