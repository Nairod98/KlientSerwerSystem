using Common;

using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    public abstract class Medium
    {
        public abstract string QA(string request);
    }

    public class MediumTCP : Medium
    {
        private readonly NetworkStream _networkStream;
        public MediumTCP(NetworkStream stream)
        {
            _networkStream = stream;
        }

        public override string QA(string request)
        {
            byte[] data = Encoding.ASCII.GetBytes(request);
            _networkStream.Write(data, 0, data.Length);

            byte[] msg = new byte[256];
            string response = "";
            int bytes;

            do
            {
                bytes = _networkStream.Read(msg, 0, msg.Length);
                response += Encoding.ASCII.GetString(msg, 0, bytes);
            }
            while (_networkStream.DataAvailable);

            return response;
        }
    }

    public class MediumUDP : Medium
    {
        private readonly UdpClient _client;
        private IPEndPoint _ipEndPoint;

        public MediumUDP(UdpClient udpClient)
        {
            _client = udpClient;
            _ipEndPoint = new IPEndPoint(IPAddress.Parse(Config.SERVER_IP), Config.PORT_UDP);
        }

        public override string QA(string request)
        {
            byte[] data = Encoding.ASCII.GetBytes(request);
            _client.Send(data, data.Length);

            byte[] msg;
            string response = "";

            do
            {
                msg = _client.Receive(ref _ipEndPoint);
                response += Encoding.ASCII.GetString(msg);
            }
            while (response.IndexOf('\n') < 0);

            return response;
        }
    }

    public class MediumFiles : Medium
    {
        private readonly string _fileName;

        public MediumFiles(string fileName)
        {
            _fileName = fileName;
        }

        public override string QA(string request)
        {
            File.WriteAllText(_fileName, request);

            Thread.Sleep(10);
            StreamReader streamReader = new StreamReader(_fileName.Replace(".txt", ".data"));
            string result = streamReader.ReadToEnd() + "\n";
            streamReader.Close();

            return result;
        }
    }

    public class MediumRS232 : Medium
    {
        private readonly SerialPort _serialPort;
        private string _newData = "";

        public MediumRS232(SerialPort serialPort)
        {
            _serialPort = serialPort;
            _serialPort.Open();
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(Handler);
        }

        public void Handler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = (SerialPort)sender;
            _newData = serialPort.ReadLine();
        }

        public override string QA(string request)
        {
            string response = "";

            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }

            _serialPort.Write(request);

            while (response.Equals(string.Empty)) 
            {
                response = _newData;
            }

            return response;
        }
    }

    public class MediumDotNetRemoting : Medium
    {
        private readonly DotNetRemotingMarshalingObj _dotNetRemotingMarshaling;

        public MediumDotNetRemoting(DotNetRemotingMarshalingObj dotNetRemotingMarshaling)
        {
            _dotNetRemotingMarshaling = dotNetRemotingMarshaling;
        }

        public override string QA(string request)
        {
            return _dotNetRemotingMarshaling.Generate(request);
        }
    }
}
