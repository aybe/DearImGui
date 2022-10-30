// ReSharper disable StringLiteralTypo

using System.Diagnostics;

namespace ImGuiNET.Test;

internal static class Program
{
    private static unsafe void Main(string[] args)
    {
        ImGui.SetCurrentContext(ImGui.CreateContext(null)); // for glyph ranges

        using (var io = ImGui.GetIO())
        {
            var ranges = io.Fonts.GlyphRangesDefault;

            var atlas = new ImFontAtlas(); // TODO destroy

            using (var font = atlas.AddFontFromFileTTF("Roboto-Regular.ttf", 12.0f, null, ref *ranges))
            {
                var build = atlas.Build();

                Debug.Assert(build);
            }

            ImGui.DestroyContext(ImGui.GetCurrentContext());

            var context = ImGui.CreateContext(atlas);

            ImGui.SetCurrentContext(context);
        }

        using (var io = ImGui.GetIO())
        {
            io.DisplaySize = new ImVec2(100, 100);

            ImGui.NewFrame();

            var pOpen = true;

            ImGui.ShowDemoWindow(ref pOpen);
        }

        ImGui.DestroyContext(ImGui.GetCurrentContext());
    }
}