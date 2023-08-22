using Common;
using System;
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

        /// <summary>
        /// Metoda do zamiany requestu na strumień danych w bajtach, a następnie jest wykonywane wysyłanie dopóki są one dostępne
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override string QA(string request)
        {
            byte[] data = Encoding.ASCII.GetBytes(request);
            _networkStream.Write(data, 0, data.Length); //wysyłanie tablicy bajtów do serwera

            byte[] msg = new byte[256]; //rozmiar jednego pakietu do odebrania z serwera
            string response = "";
            int bytes;

            do
            {
                bytes = _networkStream.Read(msg, 0, msg.Length); //odczytywanie bajtów z serwera
                response += Encoding.ASCII.GetString(msg, 0, bytes); //zamiana bajtów na string
            }
            while (_networkStream.DataAvailable);

            return response; //odpowiedź z serwera jako string
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
            try
            {
                File.WriteAllText(_fileName, request);

                Thread.Sleep(1000);
                StreamReader streamReader = new StreamReader(_fileName.Replace(".txt", ".data"));
                string result = streamReader.ReadToEnd() + "\n";
                streamReader.Close();

                File.Delete(_fileName);

                return result;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message + " Did you enable the required medium?");
                return "";
            }
        }
    }

    public class MediumRS232 : Medium
    {
        private readonly SerialPort _serialPort;
        private string _newData = "";

        //Otwieranie serial portu i odbiór danych
        public MediumRS232(SerialPort serialPort)
        {
            _serialPort = serialPort;
            _serialPort.Open();
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(Handler);
        }

        public void Handler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = (SerialPort)sender;
            _newData = serialPort.ReadExisting();
        }

        public override string QA(string request)
        {
            int triesCounter = 0;
            string response = "";

            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }

            _newData = "";
            _serialPort.Write(request);

            while (response.Equals(string.Empty) && triesCounter < 1000) 
            {
                response = _newData;
                triesCounter++;
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
            try
            {
                string response = _dotNetRemotingMarshaling.Generate(request);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }
    }
}
