using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace ImGuiNET.Test.OpenTK;

internal sealed class MyGameWindow : GameWindowBaseWithDebugContext
{
    public MyGameWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
    }

    private double ElapsedTime { get; set; }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        ElapsedTime += args.Time;
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        GL.ClearColor(Color4.FromHsv(new Vector4((float)(ElapsedTime % 10.0f / 10.0f), 1.0f, 1.0f, 1.0f)));
        GL.Clear(ClearBufferMask.ColorBufferBit);
        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }
}