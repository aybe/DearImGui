namespace imgui.NET.OpenTK;

public readonly struct ImGuiIdScope : IDisposable
{
    public ImGuiIdScope(int id)
    {
        ImGui.PushID(id);
    }

    public ImGuiIdScope(IntPtr id)
    {
        ImGui.PushID(id);
    }

    public ImGuiIdScope(string id)
    {
        ImGui.PushID(id);
    }

    public void Dispose()
    {
        ImGui.PopID();
    }
}