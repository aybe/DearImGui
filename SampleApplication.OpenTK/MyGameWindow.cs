using imgui.NET;
using imgui.NET.OpenTK;
using imgui.NET.OpenTK.Extensions;
using implot.NET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Vector2 = System.Numerics.Vector2;

namespace SampleApplication.OpenTK;

internal sealed class MyGameWindow : GameWindowBaseWithDebugContext
{
    private static readonly double[] SampleData1 = Enumerable.Range(0, 256).Select(s => Math.Cos(s / 2.0d / Math.PI)).ToArray();

    private static readonly double[] SampleData2 = Enumerable.Range(0, 256).Select(s => Math.Sin(s / 2.0d / Math.PI)).ToArray();

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

        ImGui.SetNextWindowSize(new Vector2(1000, 400), ImGuiCond.Once);

        if (ImGui.Begin("Hello, ImPlot!"))
        {
            if (ImPlot.BeginPlot("Hello, PlotLine!"))
            {
                ImPlot.SetupAxes("X", "Y");
                ImPlot.PlotLine("Sample data 1", ref SampleData1[0], SampleData1.Length);
                ImPlot.PlotLine("Sample data 2", ref SampleData2[0], SampleData2.Length);
                ImPlot.EndPlot();
            }
        }

        ImGui.End();

        Controller.Render();

        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }
}