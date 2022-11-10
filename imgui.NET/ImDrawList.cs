﻿using System.Numerics;

namespace imgui.NET;

unsafe partial class ImDrawList
{
    private ImVector<Vector4> ClipRectStack => Marshalling.Vector<Vector4>(ref ((__Internal*)__Instance)->_ClipRectStack);

    public Vector2 ClipRectMin
    {
        get
        {
            var vec = ClipRectStack[^1];
            return new Vector2(vec.X, vec.Y);
        }
    }

    public Vector2 ClipRectMax
    {
        get
        {
            var vec = ClipRectStack[^1];
            return new Vector2(vec.Z, vec.W);
        }
    }
}