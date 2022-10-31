namespace ImGuiNET.OpenTK;

public readonly struct ImGuiIdScope : IDisposable
{
    public ImGuiIdScope(int id)
    {
        ImGuiNET.ImGui.PushID(id);
    }

    public ImGuiIdScope(IntPtr id)
    {
        ImGuiNET.ImGui.PushID(id);
    }

    public ImGuiIdScope(string id)
    {
        ImGuiNET.ImGui.PushID(id);
    }

    public void Dispose()
    {
        ImGuiNET.ImGui.PopID();
    }
}