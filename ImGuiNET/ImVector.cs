using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ImGuiNET;

public readonly struct ImVector<T>
{
    private readonly __Internal Internal;

    internal ImVector(__Internal @internal)
    {
        Internal = @internal;
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= Size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }

            var size = Unsafe.SizeOf<T>();

            if (typeof(T) == typeof(ImDrawCmd))
            {
                var source = ImDrawCmd.__GetOrCreateInstance(Data + size * index);
                var result = Unsafe.As<ImDrawCmd, T>(ref source);
                return result;
            }

            throw new NotImplementedException();
        }
    }

    public int Size => Internal.Size;

    public int Capacity => Internal.Capacity;

    public IntPtr Data => Internal.Data;

    #region Nested type: __Internal

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    [UsedImplicitly]
    internal struct __Internal
    {
#pragma warning disable CS0649
        internal int Size;
        internal int Capacity;
        internal IntPtr Data;
#pragma warning restore CS0649
    }

    #endregion
}