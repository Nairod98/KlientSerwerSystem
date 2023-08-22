using Common;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Client
{
    class Client
    {
        public Client() { }

        private void PingCommander(Medium medium, string[] message)
        {
            int pingCount = 1;
            string request;
            string response;
            List<TimeSpan> elapsed = new List<TimeSpan>();

            if (message.Length < 3)
            {
                Console.WriteLine("Not enough arguments. Try 'ping' 'requestSize' 'responseSize'");
                return;
            }

            if (!int.TryParse(message[1], out int result) || !int.TryParse(message[2], out int result2))
            {
                Console.WriteLine("One or more of the arguments is of invalid type.");
                return;
            }

            if (message.Length >= 4)
            {
                bool success = int.TryParse(message[3], out pingCount);
                if (!success) 
                {
                    Console.WriteLine("One or more of the arguments is of invalid type.");
                    return;
                }
            }

            for (int i = 0; i < pingCount; i++)
            {
                request = PingPong.Ping(int.Parse(message[1]), int.Parse(message[2]));

                Stopwatch timer = Stopwatch.StartNew();
                response = medium.QA(request);
                timer.Stop();

                if (response == "") return;

                Console.WriteLine(string.Format("{0} ({1} bytes) - {2} ", response.Split(' ')[0], Encoding.ASCII.GetByteCount(response), timer.Elapsed));
                elapsed.Add(timer.Elapsed);
            }

            Console.WriteLine(string.Format("Average time: {0}", PingAverageTime(elapsed)));
        }

        private TimeSpan PingAverageTime(List<TimeSpan> elapsed)
        {
            return TimeSpan.FromMilliseconds(elapsed.Select(s => s.TotalMilliseconds).Average());
        }

        private void ChatCommander(Medium medium, string message)
        {
            string response;

            response = medium.QA(message + " \n");

            Console.WriteLine(response);
        }

        private void FileCommander(Medium medium, string[] message)
        {
            string response;
            string request;

            if (message.Length < 2)
            {
                Console.WriteLine("Not enough arguments. Try 'file list', 'file get' or 'file put'");
                return;
            }

            switch (message[1])
            {
                case "list":
                    request = string.Format("{0} {1}", message[0], message[1]);
                    break;
                case "get":
                    if (message.Length != 3)
                    {
                        Console.WriteLine("Not enough arguments. Try 'file get fileName'");
                        return;
                    }

                    request = string.Format("{0} {1} {2}", message[0], message[1], message[2]);
                    break;
                case "put":
                    if (message.Length != 3)
                    {
                        Console.WriteLine("Not enough arguments. Try 'file put fileName'");
                        return;
                    }

                    string filePath = string.Format("{0}\\{1}", Config.CLIENT_FILES_DIR, message[2]);
                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine("File not found\n");
                        return;
                    }
                    byte[] fileBytes = File.ReadAllBytes(filePath);
                    string fileBase64 = Convert.ToBase64String(fileBytes);
                    request = string.Format("{0} {1} {2} {3}", message[0], message[1], message[2], fileBase64);
                    break;
                default:
                    Console.WriteLine("Unknown command. Available commands: list, get, put.");
                    return;
            }

            response = medium.QA(request + "\n");

            if (message[1] == "get" && response != "File not found\n")
            {
                string fileServerPath = string.Format("{0}\\{1}", Config.CLIENT_FILES_DIR, message[2]);
                File.WriteAllBytes(fileServerPath, Convert.FromBase64String(response.Split('\n')[0]));
                Console.WriteLine("File downloaded successfully");
            }
            else Console.WriteLine(response); 
        }

        static void Main()
        {
            Console.WriteLine("Client");
            Client _client = new Client();
            bool flag = true;
            bool command = true;
            string strMessage;
            string[] strMessageSplit;

            Medium medium = null;

            while (flag)
            {
                Console.WriteLine("Choose protocol (tcp/udp/files/rs232/.netr) or 'exit' to close client:");

                strMessageSplit = Console.ReadLine().Split(' ');
                command = true;

                switch (strMessageSplit[0])
                {
                    case "tcp":
                        try
                        {
                            TcpClient tcpClient = new TcpClient(Config.SERVER_IP, Config.PORT_TCP);
                            NetworkStream stream = tcpClient.GetStream();
                            medium = new MediumTCP(stream);
                            flag = false;
                            break;
                        }
                        catch (SocketException)
                        {
                            Console.WriteLine("Connection failed");
                            break;
                        }
                    case "udp":
                        UdpClient udpClient = new UdpClient(Config.SERVER_IP, Config.PORT_UDP);
                        medium = new MediumUDP(udpClient);
                        flag = false;
                        break;
                    case "files":
                        medium = new MediumFiles(Config.SERVER_FILES_PROTOCOL_DIR + "\\file.txt");
                        flag = false;
                        break;
                    case "rs232":
                        medium = new MediumRS232(new SerialPort(Config.CLIENT_SERIAL_PORT, 9600, Parity.None, 8, StopBits.One));
                        flag = false;
                        break;
                    case ".netr":
                        medium = new MediumDotNetRemoting((DotNetRemotingMarshalingObj)Activator.GetObject(
                            typeof(DotNetRemotingMarshalingObj), string.Format("tcp://{0}:{1}/command", Config.SERVER_IP, Config.DOTNETREMOTING_PORT)));
                        flag = false;
                        break;
                    case "exit":
                        return;
                    default:
                        Console.WriteLine("Unknown command: {0}", strMessageSplit[0].ToString());
                        command = false;
                        break;
                }

                while (command)
                {
                
                    strMessage = Console.ReadLine();
                    strMessageSplit = strMessage.Split(' ');

                    switch (strMessageSplit[0])
                    {
                        case "ping":
                            command = true;
                            _client.PingCommander(medium, strMessageSplit);
                            break;
                        case "chat":
                            command = true;
                            _client.ChatCommander(medium, strMessage);
                            break;
                        case "file":
                            command = true;
                            _client.FileCommander(medium, strMessageSplit);
                            break;
                        case "exit":
                            command = false;
                            flag = true;
                            break;
                    }
                };
            }
        }
    }
}
