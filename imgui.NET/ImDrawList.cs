using System.Numerics;
using JetBrains.Annotations;

namespace imgui.NET;

/// <summary>
///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2505
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial struct ImDrawList
{
    private ImVector<Vector4> ClipRectStack
    {
        get
        {
            var source = __Instance._ClipRectStack;
            var vector = Marshalling.ReadVector<Vector4>(ref source);
            return vector;
        }
    }

    /// <summary>
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2535
    /// </summary>
    public Vector2 ClipRectMin
    {
        get
        {
            var vec = ClipRectStack[^1];
            return new Vector2(vec.X, vec.Y);
        }
    }

    /// <summary>
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2536
    /// </summary>
    public Vector2 ClipRectMax
    {
        get
        {
            var vec = ClipRectStack[^1];
            return new Vector2(vec.Z, vec.W);
        }
    }
}