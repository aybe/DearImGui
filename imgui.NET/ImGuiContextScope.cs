using JetBrains.Annotations;

#pragma warning disable CS1591

namespace imgui.NET;

/// <summary>
///     Scope for temporarily switching imgui context.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
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