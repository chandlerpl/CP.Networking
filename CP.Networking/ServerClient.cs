using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace CP.Networking
{
    public class ServerClient : Client
    {
        private Server _server;

        public ServerClient(Server server, TcpClient client, int bufferSize)
        {
            _server = server;
            _client = client;
            _client.ReceiveBufferSize = _bufferSize;
            _client.SendBufferSize = _bufferSize;

            _stream = _client.GetStream();

            _bufferSize = bufferSize;
            _buffer = new byte[_bufferSize];

            _stream.BeginRead(_buffer, 0, _bufferSize, Receive, null);
        }

        public override void Connect()
        {

        }

        public override void Disconnect()
        {
            base.Disconnect();
            _server.RemoveClient(this);
        }
    }
}
