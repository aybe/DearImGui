using OpenTK.Windowing.Desktop;

namespace ImGuiNET.Test.OpenTK;

internal static class Program
{
    private static void Main(string[] args)
    {
        using var window = new MyGameWindow(GameWindowSettings.Default, NativeWindowSettings.Default);

        window.Run();
    }
}