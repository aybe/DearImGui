﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

#pragma warning disable CS1591

namespace DearImGui;

/// <summary>
///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L1769
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public readonly struct ImVector<T>
{
    private readonly __Internal Internal;

    internal ImVector(__Internal @internal)
    {
        Internal = @internal;
    }

    [SuppressMessage("ReSharper", "InvertIf")]
    public unsafe T this[Index index]
    {
        get
        {
            var i = index.Value;

            if (i < 0 || i >= Size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }

            var type = typeof(T);

            if (type == typeof(ImDrawCmd))
            {
                var source = ImDrawCmd.__CreateInstance(Data + sizeof(ImDrawCmd.__Internal) * i);
                var result = Unsafe.As<ImDrawCmd, T>(ref source);
                return result;
            }

            if (type == typeof(ImDrawChannel))
            {
                var source = ImDrawChannel.__GetOrCreateInstance(Data + sizeof(ImDrawChannel.__Internal) * i);
                var result = Unsafe.As<ImDrawChannel, T>(ref source);
                return result;
            }

            if (type == typeof(ImFont))
            {
                var source = ImFont.__GetOrCreateInstance(Unsafe.Read<IntPtr>((Data + sizeof(ImFont.__Internal) * i).ToPointer()));
                var result = Unsafe.As<ImFont, T>(ref source);
                return result;
            }

            if (type == typeof(ImFontAtlasCustomRect))
            {
                var source = ImFontAtlasCustomRect.__GetOrCreateInstance(Data + sizeof(ImFontAtlasCustomRect.__Internal) * i);
                var result = Unsafe.As<ImFontAtlasCustomRect, T>(ref source);
                return result;
            }

            if (type == typeof(ImFontConfig))
            {
                var source = ImFontConfig.__GetOrCreateInstance(Data + sizeof(ImFontConfig.__Internal) * i);
                var result = Unsafe.As<ImFontConfig, T>(ref source);
                return result;
            }

            if (type == typeof(ImFontGlyph))
            {
                var source = ImFontGlyph.__GetOrCreateInstance(Data + sizeof(ImFontGlyph.__Internal) * i);
                var result = Unsafe.As<ImFontGlyph, T>(ref source);
                return result;
            }

            if (type == typeof(ImGuiStorage.ImGuiStoragePair))
            {
                var source = ImGuiStorage.ImGuiStoragePair.__GetOrCreateInstance(Data + sizeof(ImGuiStorage.ImGuiStoragePair.__Internal) * i);
                var result = Unsafe.As<ImGuiStorage.ImGuiStoragePair, T>(ref source);
                return result;
            }

            if (type == typeof(ImGuiTextFilter.ImGuiTextRange))
            {
                var source = ImGuiTextFilter.ImGuiTextRange.__GetOrCreateInstance(Data + sizeof(ImGuiTextFilter.ImGuiTextRange.__Internal) * i);
                var result = Unsafe.As<ImGuiTextFilter.ImGuiTextRange, T>(ref source);
                return result;
            }

            throw new NotImplementedException(type.ToString());
        }
    }

    /// <summary>
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L1771
    /// </summary>
    public int Size => Internal.Size;

    /// <summary>
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L1772
    /// </summary>
    public int Capacity => Internal.Capacity;

    /// <summary>
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L1773
    /// </summary>
    public IntPtr Data => Internal.Data;

    #region Nested type: __Internal

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    internal struct __Internal
    {
#pragma warning disable CS0649
        internal int Size;
        internal int Capacity;
        internal IntPtr Data;
#pragma warning restore CS0649
    }

    #endregion

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{typeof(T).Name}, {nameof(Size)}: {Size}, {nameof(Capacity)}: {Capacity}, {nameof(Data)}: 0x{Data.ToString(IntPtr.Size == 4 ? "X8" : "X16")}";
    }
}