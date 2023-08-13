using Common;

using System.Net.Sockets;

namespace Server.UDP
{
    public class UDPListener : IListener
    {
        private UdpClient _udpListener;

        public void Start(CommunicatorD onConnect)
        {
            _udpListener = new UdpClient(Config.PORT_UDP);
            UDPCommunicator _communicator = new UDPCommunicator(_udpListener);
            onConnect(_communicator);
        }

        public void Stop()
        {
            _udpListener.Close();
        }
    }
}