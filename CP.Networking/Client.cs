using CP.Networking.Loggers;
using System;
using System.Net.Sockets;
using System.Text;

namespace CP.Networking
{
    public class Client
    {
        /// <summary>
        /// An action fired when the Client successfully connects to the server.
        /// </summary>
        public Action onConnect;
        /// <summary>
        /// An action that is fired when the Client gets disconnected from the server.
        /// </summary>
        public Action onDisconnect;

        private string _host;
        private int _port;
        protected TcpClient _client;
        protected NetworkStream _stream;

        protected int _bufferSize;
        protected byte[] _buffer;
        protected ASCIIEncoding _encoding = new ASCIIEncoding();

        protected Client() { }

        /// <summary>
        /// Initialises a new Client instance with the provided host and port.
        /// </summary>
        /// <param name="host">The hostname/IP address that the Client will connect to.</param>
        /// <param name="port">The port that the Client will connect through.</param>
        /// <param name="bufferSize">The size of the buffer for incoming and outgoing messages.</param>
        public Client(string host, int port, int bufferSize = 16384) 
        {
            _host = host;
            _port = port;
            _bufferSize = bufferSize;
            _client = new TcpClient();
            
            _buffer = new byte[_bufferSize];
        }

        /// <summary>
        /// Connects the Client to the server with the host and port provided in the constructor.
        /// </summary>
        public virtual void Connect()
        {
            if (!_client.Connected)
            {
                try
                {
                    _client.Connect(_host, _port);

                    _client.ReceiveBufferSize = _bufferSize;
                    _client.SendBufferSize = _bufferSize;

                    _stream = _client.GetStream();

                    onConnect?.Invoke();
                    _stream.BeginRead(_buffer, 0, _bufferSize, Receive, null);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
        }

        /// <summary>
        /// Disconnects the Client from the server.
        /// </summary>
        public virtual void Disconnect()
        {
            if(_client != null && _client.Connected)
            {
                _stream.Close();
                _client.Close();
                _client.Dispose();

                onDisconnect?.Invoke();
            }
        }

        /// <summary>
        /// Sends a packet to the server.
        /// </summary>
        /// <param name="msg">The message to send in the packet.</param>
        public void Send(string msg)
        {
            if (!_client.Connected) return;

            try
            {
                byte[] data = _encoding.GetBytes(msg);
                _stream.BeginWrite(data, 0, data.Length, null, null);
            }  catch(Exception ex)
            {
                Logger.Log("Error sending message.", ex);
            }
        }

        protected void Receive(IAsyncResult ar)
        {
            try
            {
                int byteLength = _stream.EndRead(ar);
                if (byteLength <= 0)
                {
                    Disconnect();
                    return;
                }

                string s = _encoding.GetString(_buffer, 0, byteLength);

                //TODO: Add a proper system for handling incoming messages, command system seems most plausible.
                Logger.Log(s);

                _stream.BeginRead(_buffer, 0, _bufferSize, Receive, null);
            } 
            catch(ObjectDisposedException ex) { }
            catch (Exception ex)
            {
                Logger.Log("Error receiving data.", ex);
            }
        }
    }
}
