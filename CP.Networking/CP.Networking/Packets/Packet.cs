using System;
using System.Collections.Generic;
using System.Text;

namespace CP.Networking.Packets
{
    public abstract class Packet
    {
        public abstract int getId();

        public abstract ByteBuffer Write();

        public abstract void Handle(Client client, ByteBuffer buffer);
    }
}
