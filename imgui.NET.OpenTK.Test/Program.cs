using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace imgui.NET.OpenTK.Test;

internal static class Program
{
    private static void Main(string[] args)
    {
        var nws = new NativeWindowSettings
        {
            Size = new Vector2i(1920, 1080)
        };

        using var window = new MyGameWindow(GameWindowSettings.Default, nws);

        window.Run();
    }
}