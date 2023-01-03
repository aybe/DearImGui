using JetBrains.Annotations;

namespace DearImGui;

/// <summary>
///     Scope for temporarily switching imgui context.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public readonly struct ImGuiContextScope : IDisposable
{
    private readonly IntPtr Context;

#pragma warning disable CS1591
    public ImGuiContextScope(IntPtr context)
#pragma warning restore CS1591
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