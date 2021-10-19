using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CP.Networking
{
    public class ByteBuffer
    {
        protected List<byte> _buffer = new List<byte>();
        protected int _offset = 0;

        public ByteBuffer()
        {

        }

        public ByteBuffer(byte[] buffer)
        {
            SetBuffer(buffer);
        }

        public void SetBuffer(byte[] buffer)
        {
            _buffer.Clear();
            _buffer.AddRange(buffer);

            _offset = 0;
        }

        public byte[] GetBuffer()
        {
            byte[] buffer = _buffer.ToArray();
            _buffer.Clear();
            
            return buffer;
        }

        public byte ReadByte()
        {
            var b = _buffer[_offset];
            _offset += 1;
            return b;
        }

        public void WriteByte(byte b)
        {
            _buffer.Add(b);
        }

        public void WriteVarInt(int value)
        {
            _buffer.AddRange(value.ToVarIntArray(out _));
        }


        public int ReadVarInt()
        {
            return ReadVarInt(out _);
        }

        public int ReadVarInt(out int length)
        {
            int value = _buffer.GetVarInt(_offset, out length);
            _offset += length;

            return value;
        }

        public void WriteVarLong(long value)
        {
            _buffer.AddRange(value.ToVarLongArray(out _));
        }
        
        public long ReadVarLong()
        {
            return ReadVarLong(out _);
        }

        public long ReadVarLong(out int length)
        {
            long value = _buffer.GetVarLong(_offset, out length);
            _offset += length;

            return value;
        }

        public void WriteShort(short value)
        {
            _buffer.Add((byte)(((ushort)value >> 8) & 0xFF));
            _buffer.Add((byte)value);
        }

        public short ReadShort()
        {
            return (short)((ReadByte() << 8) | ReadByte());
        }

        public void WriteUShort(ushort value)
        {
            _buffer.Add((byte)((value >> 8) & 0xFF));
            _buffer.Add((byte)value);
        }

        public ushort ReadUShort()
        {
            return (ushort)((ReadByte() << 8) | ReadByte());
        }

        public void WriteInt(int value)
        {
            _buffer.Add((byte)(((uint)value >> 24) & 0xFF));
            _buffer.Add((byte)(((uint)value >> 16) & 0xFF));
            _buffer.Add((byte)(((uint)value >> 8) & 0xFF));
            _buffer.Add((byte)value);
        }

        public int ReadInt()
        {
            return (ReadByte() << 24) | (ReadByte() << 16) | (ReadByte() << 8) | ReadByte();
        }

        public void WriteLong(long value)
        {
            _buffer.Add((byte)(((ulong)value >> 56) & 0xFF));
            _buffer.Add((byte)(((ulong)value >> 48) & 0xFF));
            _buffer.Add((byte)(((ulong)value >> 40) & 0xFF));
            _buffer.Add((byte)(((ulong)value >> 32) & 0xFF));
            _buffer.Add((byte)(((ulong)value >> 24) & 0xFF));
            _buffer.Add((byte)(((ulong)value >> 16) & 0xFF));
            _buffer.Add((byte)(((ulong)value >> 8) & 0xFF));
            _buffer.Add((byte)value);
        }

        public int ReadLong()
        {
            return (ReadByte() << 56) | (ReadByte() << 48) | (ReadByte() << 40) | (ReadByte() << 32) | (ReadByte() << 24) | (ReadByte() << 16) | (ReadByte() << 8) | ReadByte();
        }

        public void WriteBool(bool value)
        {
            byte b;
            if(value)
                b = 0x01;
            else
                b = 0x00;

            WriteByte(b);
        }

        public bool ReadBool()
        {
            byte b = ReadByte();

            if(b == 0x00)
            {
                return false;
            } else if(b == 0x01)
            {
                return true;
            } else
            {
                throw new InvalidDataException("Received byte does not match a boolean value.");
            }
        }

        public void WriteFloat(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            _buffer.AddRange(bytes);
        }

        public float ReadFloat()
        {
            byte[] bytes = new byte[4];
            Array.Copy(_buffer.ToArray(), _offset, bytes, 0, 4);
            _offset += 4;

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToSingle(bytes, 0);
        }

        public void WriteDouble(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            _buffer.AddRange(bytes);
        }

        public double ReadDouble()
        {
            byte[] bytes = new byte[8];
            Array.Copy(_buffer.ToArray(), _offset, bytes, 0, 8);
            _offset += 8;

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToSingle(bytes, 0);
        }

        public void WriteString(string data)
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            WriteVarInt(buffer.Length);
            _buffer.AddRange(buffer);
        }

        public string ReadString(int length)
        {
            var data = Read(length);
            return Encoding.UTF8.GetString(data);
        }

        public byte[] Read(int length)
        {
            var data = new byte[length];
            Array.Copy(_buffer.ToArray(), _offset, data, 0, length);
            _offset += length;
            return data;
        }
    }
}
