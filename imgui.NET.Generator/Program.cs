using im.NET.Generator;

namespace imgui.NET.Generator;

internal static class Program
{
    private static void Main(string[] args)
    {
        var generator = new ConsoleGeneratorImGui("imgui");

        var library = new ImGuiLibrary
        {
            Namespaces = generator.Namespaces
        };

        ConsoleGenerator.Run(library, generator);
    }
}