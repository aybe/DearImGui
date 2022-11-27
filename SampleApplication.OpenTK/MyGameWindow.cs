using imgui.NET;
using imgui.NET.OpenTK;
using imgui.NET.OpenTK.Extensions;
using implot.NET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace SampleApplication.OpenTK;

internal sealed class MyGameWindow : GameWindowBaseWithDebugContext
{
    private readonly ImGuiController Controller;

    private readonly ImPlotContext ImPlotContext;

    private bool ShowImGuiDemo = true;

    private bool ShowImPlotDemo = true;

    public MyGameWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        Controller = new ImGuiController(this, new ImGuiFontConfig("Roboto-Regular.ttf", 10.0f));

        ImPlotContext = ImPlot.CreateContext();

        ImPlot.SetCurrentContext(ImPlotContext);

        ImPlot.SetImGuiContext(Controller.Context);
    }

    private double ElapsedTime { get; set; }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Controller.Dispose();
            ImPlot.DestroyContext(ImPlotContext);
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

        if (ImGui.Begin("Hello, world!"))
        {
            ImGui.Checkbox("Show ImGui Demo",  ref ShowImGuiDemo);
            ImGui.Checkbox("Show ImPlot Demo", ref ShowImPlotDemo);
        }

        ImGui.End();

        if (ShowImGuiDemo)
        {
            ImGui.ShowDemoWindow(ref ShowImGuiDemo);
        }

        if (ShowImPlotDemo)
        {
            ImPlot.ShowDemoWindow(ref ShowImPlotDemo);
        }

        Controller.Render();

        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }
}