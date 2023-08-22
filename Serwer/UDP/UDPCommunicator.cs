using Common;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.UDP
{
    public class UDPCommunicator : ICommunicator
    {
        private readonly UdpClient _client;
        private Task _task;
        IPEndPoint groupEP;
        private bool isRunning = true;

        public UDPCommunicator(UdpClient client)
        {
            _client = client;
        }

        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            _task = new Task(() =>
            {
                while (isRunning)
                {
                    groupEP = new IPEndPoint(IPAddress.Any, Config.PORT_UDP);

                    byte[] bytes = null;
                    string data = "";

                    try
                    {
                        bytes = _client.Receive(ref groupEP);

                        try
                        {
                            data = Encoding.ASCII.GetString(bytes);
                            Console.Write(string.Format("{0} - UDP | get: {1}", groupEP.Address, data));

                            byte[] msg = Encoding.ASCII.GetBytes(onCommand(data));

                            _client.Send(msg, msg.Length, groupEP);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(string.Format("{0} - {1}", groupEP.Address, ex.Message));
                        }
                    }
                    catch (Exception) { };
                }
                Stop();
            });

            _task.Start();
        }

        public void Stop()
        {
            Console.WriteLine(string.Format("{0} - Closed connection", groupEP.Address));
            isRunning = false;
            _client.Close();
        }
    }
}