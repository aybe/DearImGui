namespace imgui.NET.OpenTK;

public readonly struct ImGuiIdScope : IDisposable
{
    public ImGuiIdScope(int id)
    {
        imgui.NET.ImGui.PushID(id);
    }

    public ImGuiIdScope(IntPtr id)
    {
        imgui.NET.ImGui.PushID(id);
    }

    public ImGuiIdScope(string id)
    {
        imgui.NET.ImGui.PushID(id);
    }

    public void Dispose()
    {
        imgui.NET.ImGui.PopID();
    }
}