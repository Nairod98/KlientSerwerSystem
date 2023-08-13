using Common;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Server.DotNetRemoting
{
    public class DotNetRemotingCommunicator : ICommunicator
    {
        private readonly TcpChannel _channel;

        public DotNetRemotingCommunicator(TcpChannel channel)
        {
            _channel = channel;
        }

        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            ChannelServices.RegisterChannel(_channel, false);
            DotNetRemotingMarshalingObj _dotNetRemotingMarshalingObj = new DotNetRemotingMarshalingObj(new DotNetRemotingMarshalingObj.CommandD(onCommand));
            RemotingServices.Marshal(_dotNetRemotingMarshalingObj, "command");
        }

        public void Stop()
        {
            ChannelServices.UnregisterChannel(_channel);
        }
    }
}