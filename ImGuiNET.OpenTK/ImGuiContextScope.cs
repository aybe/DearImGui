namespace ImGuiNET.OpenTK;

internal readonly struct ImGuiContextScope : IDisposable
{
    private readonly IntPtr Context;

    public ImGuiContextScope(IntPtr context)
    {
        Context = ImGuiNET.ImGui.GetCurrentContext();

        ImGuiNET.ImGui.SetCurrentContext(context);
    }

    public void Dispose()
    {
        ImGuiNET.ImGui.SetCurrentContext(Context);
    }
}