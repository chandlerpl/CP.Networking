﻿using CP.Common.Commands;
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
                client.Connect();

                timer = new Timer(a => { client.Send("Ping"); }, null, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(100));
            } else
            {
                client.Disconnect();
                timer.Dispose();
                timer = null;
                client = null;
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