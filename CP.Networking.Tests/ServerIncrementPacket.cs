using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CP.Networking.Packets;

namespace CP.Networking.Tests
{
    public class ServerIncrementPacket : Packet
    {
        public int Number { get; set; }

        public override int getId()
        {
            return 1;
        }

        public override void Handle(Client client, ByteBuffer buffer)
        {
            Number += buffer.ReadVarInt();

            ServerCommand.server.Send(Write());
            Loggers.Logger.Log("Sending increment packet");
        }

        public override ByteBuffer Write()
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteVarInt(getId());
            buffer.WriteVarInt(Number);

            return buffer;
        }
    }
}
