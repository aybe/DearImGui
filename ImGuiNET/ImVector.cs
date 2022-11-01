using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ImGuiNET;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public readonly struct ImVector<T>
{
    private readonly __Internal Internal;

    internal ImVector(__Internal @internal)
    {
        Internal = @internal;
    }

    public ref T this[int index]
    {
        get
        {
            unsafe
            {
                if (index < 0 || index >= Internal.Size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
                }

                var zero = (byte*)Internal.Data.ToPointer();
                var size = Unsafe.SizeOf<T>();
                return ref Unsafe.AsRef<T>(zero + size * index);
            }
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