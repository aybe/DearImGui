namespace ImGuiNET.OpenTK;

internal readonly struct ImGuiContextScope : IDisposable
{
    private readonly ImGuiContext Context;

    public ImGuiContextScope(ImGuiContext context)
    {
        Context = ImGui.GetCurrentContext();

        ImGui.SetCurrentContext(context);
    }

    public void Dispose()
    {
        ImGui.SetCurrentContext(Context);
    }
}