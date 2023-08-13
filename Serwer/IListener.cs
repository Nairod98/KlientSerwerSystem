namespace Server
{
    public interface IListener
    {
        void Start(CommunicatorD onConnect);
        void Stop();
    }
}
