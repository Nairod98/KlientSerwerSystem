namespace Server
{
    public interface ICommunicator
    {
        void Start(CommandD onCommand, CommunicatorD onDisconnect);
        void Stop();
    }
}
