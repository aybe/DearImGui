using System.Collections.Immutable;
using DearGenerator.Passes;

namespace DearImGui.Generator.Passes;

public sealed class ImGuiSummaryPass : ImSummaryPass
{
    protected override ImmutableSortedSet<string> HeaderNames { get; } =
        new[]
            {
                "imgui.h"
            }
            .ToImmutableSortedSet();

    protected override string HeaderUrl { get; } = @"https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h";
}