using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.TCP
{
    public class TCPCommunicator : ICommunicator
    {
        private readonly TcpClient _client;
        private readonly string _clientAddress;
        private Task _task;

        public TCPCommunicator(TcpClient client)
        {
            _client = client;
            _clientAddress = client.Client.RemoteEndPoint.ToString();
        }

        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            _task = new Task(() =>
            {
                byte[] bytes = new byte[256];
                string data = "";
                int len, nl;
                NetworkStream stream = _client.GetStream();

                try
                {
                    while ((len = stream.Read(bytes, 0, bytes.Length)) > 0)
                    {
                        data += Encoding.ASCII.GetString(bytes, 0, len);

                        while ((nl = data.IndexOf('\n')) != -1)
                        {
                            Console.Write(string.Format("{0} - TCP | get: {1}", _clientAddress, data));
                            string line = data.Substring(0, nl + 1);
                            data = data.Substring(nl + 1);

                            byte[] msg = Encoding.ASCII.GetBytes(onCommand(line));
                            stream.Write(msg, 0, msg.Length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("{0} - {1}", _clientAddress, ex.Message));
                }

                Stop();
            });

            _task.Start();
        }

        public void Stop()
        {
            Console.WriteLine(string.Format("{0} - Closed connection", _clientAddress));
            _client.Close();
        }
    }
}