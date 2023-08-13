using Common;

using System.IO.Ports;

namespace Server.RS232
{
    public class RS232Listener : IListener
    {
        private readonly SerialPort _serialPort;

        public RS232Listener()
        {
            _serialPort = new SerialPort(Config.SERVER_SERIAL_PORT, 9600, Parity.None, 8, StopBits.One);
        }

        public RS232Listener(string config)
        {
            if (config != "")
            {
                string[] splitted = config.Split(' ');
                _serialPort = new SerialPort(splitted[0]);
            }
        }

        public void Start(CommunicatorD onConnect)
        {
            if (_serialPort != null) onConnect(new RS232Communicator(_serialPort));
        }

        public void Stop()
        {
            _serialPort?.Close();
        }
    }
}