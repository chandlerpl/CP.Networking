using CP.Networking.Loggers;
using CP.Networking.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CP.Networking.DataHandler
{
    class DefaultDataHandler : IDataHandler
    {
        /// <summary>
        /// This is used if an incomplete message has been received.
        /// </summary>
        public List<byte> messageBuffer = new List<byte>();

        public void Handle(Client client, byte[] data)
        {
            if(messageBuffer.Count != 0)
            {
                messageBuffer.AddRange(data);
                data = messageBuffer.ToArray();
                messageBuffer.Clear();
            }

            ByteBuffer buffer = new ByteBuffer(data);
            int length;
            try
            {
                length = buffer.ReadVarInt(out int varLen) + varLen;

                if (data.Length < length)
                {
                    messageBuffer.AddRange(data);
                    Logger.Log("Invalid Packet: " + data.Length + " Expected: " + (length + varLen));
                    return;
                } else if(data.Length > length)
                {
                    byte[] firstData = new byte[length];
                    Array.Copy(data, 0, firstData, 0, length);
                    byte[] secondData = new byte[data.Length - length];
                    Array.Copy(data, length, secondData, 0, data.Length - length);

                    data = firstData;
                    Task.Factory.StartNew(() => Handle(client, secondData));
                    return;
                }
            } catch (IOException ex)
            {
                Logger.Log(ex);
                messageBuffer.AddRange(data);
                return;
            }

            int packetId = buffer.ReadVarInt();

            PacketManager.HandlePacket(packetId, client, buffer);
        }

        public byte[] Prepare(byte[] data)
        {
            List<byte> rData = new List<byte>();

            rData.AddRange(data.Length.ToVarIntArray(out int _));
            rData.AddRange(data);

            return rData.ToArray();
        }
    }
}
