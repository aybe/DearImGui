#pragma warning disable CS1591
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace DearImGui.OpenTK.Extensions;

public static class KeyboardStateExtensions
{
    public static bool IsKeyDown(this KeyboardState state, params Keys[] keys)
    {
        return keys.Any(state.IsKeyDown);
    }

    public static bool IsKeyPressed(this KeyboardState state, params Keys[] keys)
    {
        return keys.Any(state.IsKeyPressed);
    }

    public static bool IsKeyReleased(this KeyboardState state, params Keys[] keys)
    {
        return keys.Any(state.IsKeyReleased);
    }
}