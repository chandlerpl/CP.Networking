using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CP.Networking.Packets;

namespace CP.Networking.Tests
{
    public class ServerPingPacket : Packet
    {
        public long PingTime { get; set; }

        public override int getId()
        {
            return 0;
        }

        public override void Handle(Client client, ByteBuffer buffer)
        {
            long timestamp = buffer.ReadVarLong();

            ServerPingPacket ping = new();
            ping.PingTime = timestamp;

            client.Send(ping.Write());
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
