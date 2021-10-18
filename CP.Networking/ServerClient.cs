using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
            _receiveBuffer = new byte[_bufferSize];

            Task.Factory.StartNew(() => Receive());
        }
    }
}
