using CP.Networking.Loggers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CP.Networking
{
    public class Server
    {
        private TcpListener _server;
        private int _maxPlayers;
        private int _port;
        private int _bufferSize;

        private List<ServerClient> _clients;
        public int ClientCount {  get { return _clients.Count; } }

        public Server(int port, int maxPlayers, int bufferSize = 16384)
        {
            _bufferSize = bufferSize;
            _maxPlayers = maxPlayers;
            _clients = new List<ServerClient>(maxPlayers);

            _server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            _server.Start();

            _server.BeginAcceptTcpClient(AcceptClient, null);
        }

        public void AcceptClient(IAsyncResult ar)
        {
            try
            {
                TcpClient client = _server.EndAcceptTcpClient(ar);
                _server.BeginAcceptTcpClient(AcceptClient, null);

                if (_clients.Count < _maxPlayers)
                {
                    ServerClient sClient = new ServerClient(this, client, _bufferSize);

                    _clients.Add(sClient);
                }
                else
                {

                    client.Close();
                    client.Dispose();
                }
            }
            catch (ObjectDisposedException ex) { }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void RemoveClient(ServerClient client)
        {
            _clients.Remove(client);
        }

        public void Send(string msg)
        {
            foreach(ServerClient client in _clients)
            {
                client.Send(msg);
            }
        }

        public void SendToClient(int id, string msg)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            for(int i = 0; i < ClientCount;)
            {
                _clients[0].Disconnect();
            }

            _server.Stop();
        }
    }
}
