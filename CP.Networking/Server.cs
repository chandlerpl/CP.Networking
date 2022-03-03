using CP.Networking.Loggers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CP.Networking
{
    /**
     * Need to tidy this up and implement some way for client-server-client communication routing.
     * 
     * Going to put this here for now but will move to do list later: Potentially add a mesh-capable networking solution with client-client routing (not multiplayer related).
     */
    public class Server
    {
        public Action onClientConnected;
        public Action onClientDisconnected;

        private TcpListener _server;
        private int _maxClients;
        private int _port;
        private int _bufferSize;

        public bool UseEncryption { get; set; } = false;
        public bool Compression { get; set; } = false;

        private List<ServerClient> _clients;
        public int ClientCount {  get { return _clients.Count; } }

        /// <summary>
        /// Creates a new instance for the TCP Server with the given port.
        /// </summary>
        /// <param name="port">The port that the server listens for clients on.</param>
        /// <param name="maxClients">The maximum number of clients that can be connected at one time.</param>
        /// <param name="bufferSize">The size, in bytes, of the buffer for incoming and outgoing messages.</param>
        public Server(int port, int maxClients, int bufferSize = 16384)
        {
            _bufferSize = bufferSize;
            _maxClients = maxClients;
            _clients = new List<ServerClient>(maxClients);

            _server = new TcpListener(IPAddress.Any, port);
            _server.AllowNatTraversal(true);

            _server.Start();

            Task.Factory.StartNew(() => AcceptClient());
        }

        protected void AcceptClient()
        {
            while(true)
            {
                try
                {
                    TcpClient client = _server.AcceptTcpClient();

                    Task.Factory.StartNew(() => ConnectClient(client));
                }
                catch (ObjectDisposedException ex) { }
                catch(SocketException ex)
                {
                    Close();
                    break;
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    Close();
                    break;
                }
            }
        }

        private void ConnectClient(TcpClient client)
        {
            if (_clients.Count < _maxClients)
            {
                ServerClient sClient = new ServerClient(this, client, _bufferSize);
                sClient.onDisconnect += () => { RemoveClient(sClient); };

                onClientConnected?.Invoke();

                _clients.Add(sClient);
            }
            else
            {
                client.Close();
                client.Dispose();
            }
        }

        /// <summary>
        /// Removes a client from the servers internal list, this should only be called when a client connection has been closed.
        /// </summary>
        /// <param name="client">The client to remove from the list.</param>
        public void RemoveClient(ServerClient client)
        {
            onClientDisconnected?.Invoke();
            _clients.Remove(client);
        }

        /// <summary>
        /// Sends a packet to all connected clients.
        /// </summary>
        /// <param name="msg">The message to be to all clients.</param>
        public void Send(string msg)
        {
            foreach(ServerClient client in _clients)
            {
                client.Send(msg);
            }
        }

        public void Send(ByteBuffer msg)
        {
            foreach (ServerClient client in _clients)
            {
                client.Send(msg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        public void SendToClient(int id, string msg)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Closes all existing client connections and shuts down the server listener.
        /// </summary>
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
