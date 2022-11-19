using im.NET.Generator.Passes;

namespace imgui.NET.Generator.Passes;

public sealed class ImGuiSummaryPass:ImSummaryPass
{
    protected override string HeaderName { get; } = "imgui.h";

    protected override string HeaderUrl { get; } = @"https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h";
}