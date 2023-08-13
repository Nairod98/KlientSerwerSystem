using Common;

using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Server.DotNetRemoting
{
    public class DotNetRemotingListener : IListener
    {
        private readonly TcpChannel _channel;

        public DotNetRemotingListener()
        {
            _channel = new TcpChannel(Config.DOTNETREMOTING_PORT);
        }

        public void Start(CommunicatorD onConnect)
        {
            onConnect(new DotNetRemotingCommunicator(_channel));
        }

        public void Stop()
        {
            ChannelServices.UnregisterChannel(_channel);
        }
    }
}