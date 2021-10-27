using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CP.Networking.Packets;

namespace CP.Networking.Tests
{
    public class ClientPingPacket : Packet
    {
        public long PingTime { get; set; }

        public override int getId()
        {
            return 0;
        }

        public override void Handle(Client client, ByteBuffer buffer)
        {
            long timestamp = buffer.ReadVarLong();

            long currTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            Loggers.Logger.Log(Loggers.LogLevel.INFO, "Ping: " + (currTime - timestamp) + "ms");
        }

        public override ByteBuffer Write()
        {
            ByteBuffer buffer = new();
            buffer.WriteVarInt(getId());
            buffer.WriteVarLong(PingTime);

            return buffer;
        }
    }
}
