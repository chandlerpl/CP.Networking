using CP.Networking.DataHandler;
using CP.Networking.Loggers;
using CP.Networking.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        protected Stream _stream;
        protected IDataHandler _handler;
        public IDataHandler DataHandler {
        get {
                if (_handler == null)
                    _handler = new DefaultDataHandler();

                return _handler;
            }
            set => _handler = value; 
        }

        protected int _bufferSize;
        protected byte[] _receiveBuffer;

        protected Client() { }

        /// <summary>
        /// Initialises a new Client instance with the provided host and port.
        /// </summary>
        /// <param name="host">The hostname/IP address that the Client will connect to.</param>
        /// <param name="port">The port that the Client will connect through.</param>
        /// <param name="bufferSize">The size of the buffer for incoming and outgoing messages.</param>
        public Client(string host, int port, int bufferSize = 16384, IDataHandler handler = null) 
        {
            _host = host;
            _port = port;
            _bufferSize = bufferSize;
            _handler = handler;
            _client = new TcpClient(AddressFamily.InterNetwork);
            
            _receiveBuffer = new byte[_bufferSize];
        }

        /// <summary>
        /// Connects the Client to the server with the host and port provided in the constructor.
        /// </summary>
        public virtual void Connect()
        {
            if (_host == null || _port == 0)
                return;

            if (_client == null)
                _client = new TcpClient();

            if (!_client.Connected)
            {
                try
                {
                    _client.Connect(_host, _port);
                    _client.SendBufferSize = _bufferSize;

                    _stream = _client.GetStream();

                    onConnect?.Invoke();
                    Task.Factory.StartNew(() => Receive());
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
        }

        public virtual void Connect(string host, int port, int bufferSize = 16384, IDataHandler handler = null)
        {
            _host = host;
            _port = port;
            _bufferSize = bufferSize;
            _handler = handler;
            _client = new TcpClient(AddressFamily.InterNetwork);

            _receiveBuffer = new byte[_bufferSize];

            Connect();
        }

        /// <summary>
        /// Disconnects the Client from the server.
        /// </summary>
        public virtual void Disconnect()
        {
            if(_client != null)
            {
                _stream.Close();
                _client.Close();
                _client.Dispose();

                _client = null;

                onDisconnect?.Invoke();
            }
        }

        /// <summary>
        /// Sends a string through the network stream.
        /// </summary>
        /// <param name="msg">The message to send in the packet.</param>
        public void Send(string msg)
        {
            if (!_client.Connected) return;

            byte[] data = Encoding.UTF8.GetBytes(msg);
            Send(data);
        }

        public void Send(ByteBuffer buffer)
        {
            if (!_client.Connected) return;

            byte[] data = buffer.GetBuffer();
            Send(data);
        }

        public void Send(byte[] data)
        {
            if (!_client.Connected) return;

            data = DataHandler.Prepare(data);
            try
            {
                var _ = _stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Logger.Log("Error sending message.", ex);
            }
        }

        protected async Task Receive()
        {
            while(true)
            {
                try
                {
                    int byteLength = await _stream.ReadAsync(_receiveBuffer, 0, _bufferSize);
                    if (byteLength <= 0)
                    {
                        Disconnect();
                        break;
                    }
                    byte[] info = new byte[byteLength];
                    Array.Copy(_receiveBuffer, 0, info, 0, byteLength);

                    DataHandler.Handle(this, info);
                }
                catch (Exception ex)
                {
                    if(ex is ObjectDisposedException || ex is SocketException || ex is IOException)
                    {
                        Disconnect();
                        break;
                    }

                    Logger.Log("Error receiving data.", ex);
                }
            }
        }
    }
}
