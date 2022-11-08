using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace imgui.NET;

public readonly struct ImVector<T> : IEnumerable<T>
{
    private readonly __Internal Internal;

    [UsedImplicitly] // seen as unused by R# when analysis is disabled
    internal ImVector(__Internal @internal)
    {
        Internal = @internal;
    }

    [SuppressMessage("ReSharper", "InvertIf")]
    public unsafe T this[int index]
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

            if (type.IsValueType)
            {
                return Unsafe.AsRef<T>(data.ToPointer()); // works for structs only
            }

            if (type == typeof(ImDrawCmd))
            {
                var source = ImDrawCmd.__GetOrCreateInstance(Data + sizeof(ImDrawCmd.__Internal) * index);
                var result = Unsafe.As<ImDrawCmd, T>(ref source);
                return result;
            }

            throw new NotImplementedException(type.ToString());
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
        for (var i = 0; i < Size; i++)
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
    public override string ToString()
    {
        return $"{nameof(Size)}: {Size}, {nameof(Capacity)}: {Capacity}, {nameof(Data)}: 0x{Data.ToString(IntPtr.Size == 4 ? "X8" : "X16")}";
    }
}