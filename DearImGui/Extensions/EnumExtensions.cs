#pragma warning disable CS1591
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace DearImGui.Extensions;

[SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
public static class EnumExtensions
{
    public static bool HasFlags<T>(this ref T value, in T flags) where T : unmanaged, Enum, IConvertible
    {
        var code = Type.GetTypeCode(typeof(T));

        switch (code)
        {
            case TypeCode.SByte:
            {
                var a = value.ToSByte(null);
                var b = flags.ToSByte(null);
                return (a & b) == b;
            }
            case TypeCode.Byte:
            {
                var a = value.ToByte(null);
                var b = flags.ToByte(null);
                return (a & b) == b;
            }
            case TypeCode.Int16:
            {
                var a = value.ToInt16(null);
                var b = flags.ToInt16(null);
                return (a & b) == b;
            }
            case TypeCode.UInt16:
            {
                var a = value.ToUInt16(null);
                var b = flags.ToUInt16(null);
                return (a & b) == b;
            }
            case TypeCode.Int32:
            {
                var a = value.ToInt32(null);
                var b = flags.ToInt32(null);
                return (a & b) == b;
            }
            case TypeCode.UInt32:
            {
                var a = value.ToUInt32(null);
                var b = flags.ToUInt32(null);
                return (a & b) == b;
            }
            case TypeCode.Int64:
            {
                var a = value.ToInt64(null);
                var b = flags.ToInt64(null);
                return (a & b) == b;
            }
            case TypeCode.UInt64:
            {
                var a = value.ToUInt64(null);
                var b = flags.ToUInt64(null);
                return (a & b) == b;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(value));
        }
    }

    public static void SetFlags<T>(this ref T value, in T flags, in bool apply) where T : unmanaged, Enum, IConvertible
    {
        var code = Type.GetTypeCode(typeof(T));

        switch (code)
        {
            case TypeCode.SByte:
            {
                var a = value.ToSByte(null);
                var b = flags.ToSByte(null);
                var c = (sbyte)(apply ? a | b : a & ~b);
                value = Unsafe.As<sbyte, T>(ref c);
                break;
            }
            case TypeCode.Byte:
            {
                var a = value.ToByte(null);
                var b = flags.ToByte(null);
                var c = (byte)(apply ? a | b : a & ~b);
                value = Unsafe.As<byte, T>(ref c);
                break;
            }
            case TypeCode.Int16:
            {
                var a = value.ToInt16(null);
                var b = flags.ToInt16(null);
                var c = (short)(apply ? a | b : a & ~b);
                value = Unsafe.As<short, T>(ref c);
                break;
            }
            case TypeCode.UInt16:
            {
                var a = value.ToUInt16(null);
                var b = flags.ToUInt16(null);
                var c = (ushort)(apply ? a | b : a & ~b);
                value = Unsafe.As<ushort, T>(ref c);
                break;
            }
            case TypeCode.Int32:
            {
                var a = value.ToInt32(null);
                var b = flags.ToInt32(null);
                var c = apply ? a | b : a & ~b;
                value = Unsafe.As<int, T>(ref c);
                break;
            }
            case TypeCode.UInt32:
            {
                var a = value.ToUInt32(null);
                var b = flags.ToUInt32(null);
                var c = apply ? a | b : a & ~b;
                value = Unsafe.As<uint, T>(ref c);
                break;
            }
            case TypeCode.Int64:
            {
                var a = value.ToInt64(null);
                var b = flags.ToInt64(null);
                var c = apply ? a | b : a & ~b;
                value = Unsafe.As<long, T>(ref c);
                break;
            }
            case TypeCode.UInt64:
            {
                var a = value.ToUInt64(null);
                var b = flags.ToUInt64(null);
                var c = apply ? a | b : a & ~b;
                value = Unsafe.As<ulong, T>(ref c);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(value));
        }
    }
}