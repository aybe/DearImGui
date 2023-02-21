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

    protected override string HeaderUrl { get; } = @"https://github.com/ocornut/imgui/blob/9cd9c2eff99877a3f10a7f9c2a3a5b9c15ea36c6/imgui.h";
}