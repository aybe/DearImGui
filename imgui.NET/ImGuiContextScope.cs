namespace imgui.NET;

public readonly struct ImGuiContextScope : IDisposable
{
    private readonly ImGuiContext Context;

    public ImGuiContextScope(ImGuiContext context)
    {
        Context = ImGui.GetCurrentContext();

        ImGui.SetCurrentContext(context);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        ImGui.SetCurrentContext(Context);
    }
}