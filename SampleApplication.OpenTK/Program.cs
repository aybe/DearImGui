using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SampleApplication.OpenTK;

internal static class Program
{
    private static void Main(string[] args)
    {
        static unsafe NativeWindowSettings GetNativeWindowSettings()
        {
            GLFW.Init();

            var pm = GLFW.GetPrimaryMonitor();
            var vm = GLFW.GetVideoMode(pm);
            var sw = vm->Width;
            var sh = vm->Height;
            var ww = sw * 2 / 3;
            var wh = sh * 2 / 3;
            var wx = sw / 2 - ww / 2;
            var wy = sh / 2 - wh / 2;
            var vs = new Vector2i(ww, wh);
            var vl = new Vector2i(wx, wy);

            return new NativeWindowSettings
            {
                Size = vs,
                Location = vl,
                Title = "DearImGui"
            };
        }

        var nws = GetNativeWindowSettings();

        using var window = new MyGameWindow(GameWindowSettings.Default, nws);

        window.Run();
    }
}