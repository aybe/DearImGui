using System.Drawing;
using DearImGui;
using DearImGui.OpenTK;
using DearImGui.OpenTK.Extensions;
using DearImPlot;
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

    private Color4 Color1 = Color4.Crimson;

    private Color4 Color2 = Color4.DeepSkyBlue;

    private bool ShowImGuiDemo = true;

    private bool ShowImPlotDemo = true;

    public MyGameWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        Controller = new ImGuiController(this, "Roboto-Regular.ttf", 10.0f);

        ImPlotContext = ImPlot.CreateContext();

        ImPlot.SetCurrentContext(ImPlotContext);

        ImPlot.SetImGuiContext(Controller.Context);
    }

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
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        GL.ClearColor(Color.CornflowerBlue);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        ImGui.SetNextWindowSize(new Vector2(800, 500), ImGuiCond.Once);

        if (ImGui.Begin("Hello, world!"))
        {
            if (ImPlot.BeginPlot("Sample plot"))
            {
                ImPlot.SetupAxes("X", "Y");

                ImPlot.SetNextLineStyle(Color1.ToVector4());
                ImPlot.PlotLine("Sample data 1", ref SampleData1[0], SampleData1.Length);

                ImPlot.SetNextLineStyle(Color2.ToVector4());
                ImPlot.PlotLine("Sample data 2", ref SampleData2[0], SampleData2.Length);

                ImPlot.EndPlot();
            }

            ImGui.ColorEdit4("Color 1", Color1.AsSpan(), ImGuiColorEditFlags.NoInputs);
            ImGui.ColorEdit4("Color 2", Color2.AsSpan(), ImGuiColorEditFlags.NoInputs);

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