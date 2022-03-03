using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public static class VariableLengthExtension
{
    public static byte[] ToVarIntArray(this int value, out int length)
    {
        List<byte> result = new List<byte>(5);
        length = 1;

        while ((value & 0xFFFFFF80) != 0)
        {
            result.Add((byte)(value & 0x7F | 0x80));
            value = (int)((uint)value) >> 7;
            ++length;
        }
        result.Add((byte)value);

        return result.ToArray();
    }

    public static int GetVarInt(this byte[] buffer, int offset, out int length)
    {
        var value = 0;
        var size = 0;
        int b;
        length = 0;

        while (((b = buffer[offset + length++]) & 0x80) == 0x80)
        {
            value |= (b & 0x7F) << (size++ * 7);
            if (size > 5)
            {
                throw new IOException("This VarInt is an imposter!");
            }
        }
        return value | ((b & 0x7F) << (size * 7));
    }

    public static int GetVarInt(this List<byte> buffer, int offset, out int length)
    {
        var value = 0;
        var size = 0;
        int b;
        length = 0;

        while (((b = buffer[offset + length++]) & 0x80) == 0x80)
        {
            value |= (b & 0x7F) << (size++ * 7);
            if (size > 5)
            {
                throw new IOException("This VarInt is an imposter!");
            }
        }
        return value | ((b & 0x7F) << (size * 7));
    }

    public static byte[] ToVarLongArray(this long value, out int length)
    {
        List<byte> result = new List<byte>(10);
        length = 1;

        while (((ulong)value & 0xFFFFFFFFFFFFFF80) != 0)
        {
            Console.WriteLine("{0:X}", value);
            Console.WriteLine((ulong)value & 0xFFFFFFFFFFFFFF80);
            result.Add((byte)(value & 0x7F | 0x80));
            value = (long)((ulong)value) >> 7;
            ++length;
        }
        result.Add((byte)value);

        return result.ToArray();
    }

    public static long GetVarLong(this byte[] buffer, int offset, out int length)
    {
        long value = 0;
        int size = 0;
        byte b;
        length = 0;

        while (((b = buffer[offset + length++]) & 0x80) == 0x80)
        {
            value |= (b & (long)0x7F) << (size++ * 7);
            if (size > 10)
            {
                throw new IOException("This VarInt is an imposter!");
            }
        }
        return value | ((b & (long)0x7F) << (size * 7));
    }

    public static long GetVarLong(this List<byte> buffer, int offset, out int length)
    {
        long value = 0;
        int size = 0;
        byte b;
        length = 0;

        while (((b = buffer[offset + length++]) & 0x80) == 0x80)
        {
            value |= (b & (long)0x7F) << (size++ * 7);
            if (size > 10)
            {
                throw new IOException("This VarInt is an imposter!");
            }
        }
        return value | ((b & (long)0x7F) << (size * 7));
    }
}
