using Common;

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server.TCP
{
    public class TCPListener : IListener
    {
        private TcpListener _tcpListener;
        private bool isRunning = true;
        private Thread _thread;

        public void Start(CommunicatorD onConnect)
        {
            _tcpListener = new TcpListener(IPAddress.Any, Config.PORT_TCP);
            _tcpListener.Start();

            _thread = new Thread(t =>
            {
                while (isRunning)
                {
                    try
                    {
                        TcpClient _client = _tcpListener.AcceptTcpClient();
                        TCPCommunicator _communicator = new TCPCommunicator(_client);
                        onConnect(_communicator);
                    }
                    catch (Exception) { };
                }
            });

            _thread.Start();
        }

        public void Stop()
        {
            isRunning = false;
            _tcpListener.Stop();
        }
    }
}
