namespace ImGuiNET.OpenTK;

public readonly struct ImGuiTreeNode : IDisposable
{
    public bool Expanded { get; }

    public ImGuiTreeNode(string label, ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.FramePadding)
    {
        Expanded = ImGuiNET.ImGui.TreeNodeEx(label, flags);
    }

    public void Dispose()
    {
        if (Expanded)
        {
            ImGuiNET.ImGui.TreePop();
        }
    }
}