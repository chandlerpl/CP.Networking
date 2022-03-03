using CP.Common.Commands;
using CP.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CP.Networking.Tests
{
    public class ServerCommand : Command
    {
        public static Server server;
        Timer timer;

        public override bool Execute(object obj, List<string> args)
        {
            if(server == null)
            {
                Loggers.Logger.Log("Establishing a server connection.");

                PacketManager.RegisterPacket(new ServerPingPacket());
                PacketManager.RegisterPacket(new ServerIncrementPacket());
                server = new Server(43435, 2);
                
                timer = new Timer(a => { Loggers.Logger.Log("Connected Clients: " + server.ClientCount); }, null, TimeSpan.FromMilliseconds(0), TimeSpan.FromSeconds(1));
            } else
            {
                PacketManager.UnregisterPacket(0);
                PacketManager.UnregisterPacket(1);
                server.Close();
                timer.Dispose();
                server = null;
                timer = null;
            }
            
            return true;
        }

        public override bool Init()
        {
            Name = "Server";
            Desc = "Starts running a server!";
            Aliases = new List<string>() { "server" };
            ProperUse = "server";

            return true;
        }
    }
}
