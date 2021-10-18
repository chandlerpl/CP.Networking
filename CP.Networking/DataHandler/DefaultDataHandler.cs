using CP.Networking.Loggers;
using CP.Networking.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CP.Networking.DataHandler
{
    class DefaultDataHandler : IDataHandler
    {
        /// <summary>
        /// This is used if an incomplete message has been received.
        /// </summary>
        public List<byte> messageBuffer = new List<byte>();

        private static readonly byte[] MESSAGE_END = Encoding.UTF8.GetBytes("$");
        private static readonly int MESSAGE_END_LENGTH = MESSAGE_END.Length;

        public void Handle(Client client, byte[] data)
        {
            int len = data.Length;
            string val;
            try
            {
                val = Encoding.UTF8.GetString(data, len - MESSAGE_END_LENGTH, MESSAGE_END_LENGTH);

                if(val != "$")
                {
                    messageBuffer.AddRange(data);
                    return;
                }
            } catch (Exception ex)
            {
                Logger.Log(ex);
                messageBuffer.AddRange(data);
                return;
            }

            ByteBuffer buffer = new ByteBuffer();
            buffer.SetBuffer(data);

            int length = buffer.ReadVarInt();
            int packetId = buffer.ReadVarInt();

            PacketManager.HandlePacket(packetId, client, buffer);
        }

        public byte[] Prepare(byte[] data)
        {
            List<byte> rData = new List<byte>();

            rData.AddRange(WriteVarInt(data.Length + MESSAGE_END_LENGTH));
            rData.AddRange(data);
            rData.AddRange(MESSAGE_END);

            return rData.ToArray();
        }

        private byte[] WriteVarInt(int value)
        {
            List<byte> buffer = new List<byte>(5);

            while ((value & 128) != 0)
            {
                buffer.Add((byte)(value & 127 | 128));
                value = (int)((uint)value) >> 7;
            }
            buffer.Add((byte)value);

            return buffer.ToArray();
        }
    }
}
