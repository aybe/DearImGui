using JetBrains.Annotations;

namespace imgui.NET;

/// <summary>
///     Scope for temporarily switching imgui context.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public readonly struct ImGuiContextScope : IDisposable
{
    private readonly ImGuiContext Context;

#pragma warning disable CS1591
    public ImGuiContextScope(ImGuiContext context)
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