using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace imgui.NET.Test.OpenTK;

internal static class Program
{
    private static void Main(string[] args)
    {
        var nws = new NativeWindowSettings
        {
            Location = new Vector2i(500, 500),
            Size = new Vector2i(1000, 1000)
        };

        using var window = new MyGameWindow(GameWindowSettings.Default, nws);

        window.Run();
    }
}