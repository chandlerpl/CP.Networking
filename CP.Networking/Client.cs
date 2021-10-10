using CP.Networking.Loggers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CP.Networking
{
    public class Client
    {
        private string _host;
        private int _port;
        protected TcpClient _client;
        protected NetworkStream _stream;

        protected int _bufferSize;
        protected byte[] _buffer;
        protected ASCIIEncoding _encoding = new ASCIIEncoding();

        protected Client() { }

        public Client(string host, int port, int bufferSize = 16384) 
        {
            _host = host;
            _port = port;
            _bufferSize = bufferSize;
            _client = new TcpClient();
            
            _buffer = new byte[_bufferSize];
        }

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

                    _stream.BeginRead(_buffer, 0, _bufferSize, Receive, null);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
        }

        public virtual void Disconnect()
        {
            _stream.Close();
            _client.Close();
            _client.Dispose();
        }

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

        public void Receive(IAsyncResult ar)
        {
            try
            {
                int byteLength = _stream.EndRead(ar);
                if (byteLength <= 0)
                {
                    Disconnect();
                    //TODO: Add a connection ended function for easily handling disconnects, Callbacks are most likely.
                    Logger.Log("Disconnected from the server!");
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
