using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ImGuiNET;

public readonly struct ImVector<T> : IReadOnlyList<T>
{
    private readonly __Internal Internal;

    [UsedImplicitly] // seen as unused by R# when analysis is disabled
    internal ImVector(__Internal @internal)
    {
        Internal = @internal;
    }

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "InvertIf")]
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= Size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }

            var type = typeof(T);
            var size = Unsafe.SizeOf<T>();
            var data = Data + size * index;

            if (type == typeof(ImDrawCmd))
            {
                var source = ImDrawCmd.__GetOrCreateInstance(data);
                var result = Unsafe.As<ImDrawCmd, T>(ref source);
                return result;
            }

            if (type == typeof(ImDrawVert))
            {
                var source = ImDrawVert.__GetOrCreateInstance(data);
                var result = Unsafe.As<ImDrawVert, T>(ref source);
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

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
// BUG The debugger is unable to evaluate this expression
    {
        for (var i = 0; i < Count; i++)
        {
            var item = this[i];
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public int Count => Internal.Size;
    public override string ToString()
    {
        return $"{nameof(Size)}: {Size}, {nameof(Capacity)}: {Capacity}, {nameof(Data)}: 0x{Data.ToString(IntPtr.Size == 4 ? "X8" : "X16")}";
    }
}