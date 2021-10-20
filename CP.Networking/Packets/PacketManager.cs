using CP.Networking.Loggers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CP.Networking.Packets
{
    public class PacketManager
    {
        private static Dictionary<int, Packet> registeredPackets = new Dictionary<int, Packet>();

        private PacketManager() { }

        public static void HandlePacket(int id, Client client, ByteBuffer buffer)
        {
            if(registeredPackets.ContainsKey(id))
            {
                registeredPackets[id].Handle(client, buffer);
            }
            else
            {
                Logger.Log("Unhandled packet! ID: + " + id + " \nData: " + buffer.ToString());
            }
        }

        public static bool RegisterPacket(Packet packet)
        {
            int packetId = packet.getId();

            if(registeredPackets.ContainsKey(packetId))
            {
                Loggers.Logger.Log("Failed to add packet");
                return false;
            }

            registeredPackets.Add(packetId, packet);
            return true;
        }

        public static bool UnregisterPacket(int packetId)
        {
            if(registeredPackets.ContainsKey(packetId))
            {
                registeredPackets.Remove(packetId);
                return true;
            }

            return false;
        }
    }
}
