using ImGuiNET.OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace ImGuiNET.Test.OpenTK;

internal sealed class MyGameWindow : GameWindowBaseWithDebugContext
{
    private readonly ImGuiController Controller;

    public MyGameWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        Controller = new ImGuiController(this, new ImGuiFontConfig("Roboto-Regular.ttf", 10.0f));
    }

    private double ElapsedTime { get; set; }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Controller.Dispose();
        }

        base.Dispose(disposing);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        Controller.Update((float)args.Time);

        ElapsedTime += args.Time;
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        GL.ClearColor(Color4.FromHsv(new Vector4((float)(ElapsedTime % 10.0f / 10.0f), 1.0f, 0.25f, 1.0f)));
        GL.Clear(ClearBufferMask.ColorBufferBit);
        var b = true;
        ImGui.ShowDemoWindow(ref b);
        Controller.Render();
        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }
}