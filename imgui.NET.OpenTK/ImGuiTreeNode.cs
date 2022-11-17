namespace imgui.NET.OpenTK;

public readonly struct ImGuiTreeNode : IDisposable
{
    public bool Expanded { get; }

    public ImGuiTreeNode(string label, ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.FramePadding)
    {
        Expanded = ImGui.TreeNodeEx(label, flags);
    }

    public void Dispose()
    {
        if (Expanded)
        {
            ImGui.TreePop();
        }
    }
}