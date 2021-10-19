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
    public class ClientCommand : Command
    {
        Client client;
        Timer timer;

        public override bool Execute(object obj, List<string> args)
        {
            if(client == null)
            {
                Console.WriteLine("Establishing a client connection.");

                client = new Client("127.0.0.1", 43435);
                client.onConnect += () => {
                    Loggers.Logger.Log("Established a connection to the server.");
                    timer = new Timer(a => {
                        ClientPingPacket ping = new();
                        ping.PingTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                        client.Send(ping.Write());
                    }, null, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(100));
                };
                client.onDisconnect += () =>
                {
                    Loggers.Logger.Log("Connection to the server has been lost.");
                    timer.Dispose();
                    timer = null;
                    client = null;
                };

                PacketManager.RegisterPacket(new ClientPingPacket());
                client.Connect();
            } else
            {
                PacketManager.UnregisterPacket(0);
                client.Disconnect();
            }
            
            return true;
        }

        public override bool Init()
        {
            Name = "Client";
            Desc = "Starts running a client!";
            Aliases = new List<string>() { "client" };
            ProperUse = "client";

            return true;
        }
    }
}
