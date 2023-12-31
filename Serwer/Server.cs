﻿using Server.DotNetRemoting;
using Server.FilesProtocol;
using Server.RS232;
using Server.TCP;
using Server.UDP;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    class Server
    {
        readonly List<IListener> _listeners = new List<IListener>();
        readonly List<ICommunicator> _communicators = new List<ICommunicator>();
        readonly Dictionary<string, IServiceModule> _services = new Dictionary<string, IServiceModule>();

        public Server() { }

        //Dodawanie serwisu
        void AddService(string name, IServiceModule service)
        {
            _services.Add(name, service);
        }

        //Dodawanie komunikatora
        void AddCommunicator(ICommunicator communicator)
        {
            _communicators.Add(communicator);
            communicator.Start(GetService, RemoveCommunicator);
        }

        //Dodawanie nasłuchiwacza
        void AddListener(IListener listener)
        {
            _listeners.Add(listener);
            listener.Start(new CommunicatorD(AddCommunicator));
        }

        //Usuwanie serwisu
        void RemoveService(string name, IServiceModule service)
        {
            _services.Remove(name);
            Console.WriteLine(string.Format("Stopped {0} service", service));
        }

        //Usuwanie komunikatora
        void RemoveCommunicator(ICommunicator communicator)
        {
            communicator.Stop();
            _communicators.Remove(communicator);
        }

        //Usuwanie nasłuchiwacza
        void RemoveListener(IListener listener)
        {
            listener.Stop();
            _listeners.Remove(listener);
        }

        //Tworzenie serwisu
        string GetService(string serviceName)
        {
            string[] service = serviceName.Split(' ');
            string _serviceName = service[0];
            string _serviceParams = string.Join(" ", service.Skip(1));
            _services.TryGetValue(_serviceName, out IServiceModule serviceModule);

            if (serviceModule != null) return serviceModule.AnswerCommand(string.Format("{0} {1}", _serviceName, _serviceParams));
            else
            {
                Console.WriteLine("{0} is inaccesible", _serviceName);
                return "";
            }
        }

        //Wydawanie komend serwerowi
        void ConfigCommander(string[] command)
        {
            if (command[0] != "conf")
            {
                Console.WriteLine("Unknown command");
                return;
            }

            switch (command[1])
            {
                case "start-all":
                    StartService("all");
                    StartMedium("all");
                    break;

                case "start-service":
                    StartService(command[2]);
                    break;

                case "stop-service":
                    StopService(command[2]);
                    break;

                case "start-medium":
                    StartMedium(command[2]);
                    break;

                case "stop-medium":
                    StopMedium(command[2]);
                    break;

                default:
                    Console.WriteLine("Unknown command");
                    break;
            }
        }

        //Uruchamianie serwisu
        void StartService(string serviceName)
        {
            switch (serviceName)
            {
                case "ping":
                    AddService("ping", new PingPongServiceModule());
                    Console.WriteLine("Ping service started");
                    break;

                case "chat":
                    AddService("chat", new ChatServiceModule());
                    Console.WriteLine("Chat service started");
                    break;

                case "file":
                    AddService("file", new FileServiceModule());
                    Console.WriteLine("File service started");
                    break;

                case "all":
                    AddService("ping", new PingPongServiceModule());
                    Console.WriteLine("Ping service started");
                    AddService("chat", new ChatServiceModule());
                    Console.WriteLine("Chat service started");
                    AddService("file", new FileServiceModule());
                    Console.WriteLine("File service started");
                    break;

                default:
                    Console.WriteLine("Unknown service");
                    break;
            }
        }

        //Zatrzymywanie serwisu
        void StopService(string serviceName)
        {
            if (_services.ContainsKey(serviceName)) RemoveService(serviceName, _services[serviceName]);
            else Console.WriteLine("Unknown service");
        }

        //Uruchamianie medium
        void StartMedium(string mediumName)
        {
            switch (mediumName)
            {
                case "tcp":
                    AddListener(new TCPListener());
                    Console.WriteLine("Tcp medium started");
                    break;

                case "udp":
                    AddListener(new UDPListener());
                    Console.WriteLine("Udp medium started");
                    break;

                case "files":
                    AddListener(new FilesProtocolListener());
                    Console.WriteLine("Files medium started");
                    break;

                case "RS232":
                    AddListener(new RS232Listener());
                    Console.WriteLine("Rs232 medium started");
                    break;

                case ".netr":
                    AddListener(new DotNetRemotingListener());
                    Console.WriteLine(".net remoting medium started");
                    break;

                case "all":
                    AddListener(new TCPListener());
                    Console.WriteLine("Tcp medium started");
                    AddListener(new UDPListener());
                    Console.WriteLine("Udp medium started");
                    AddListener(new FilesProtocolListener());
                    Console.WriteLine("Files medium started");
                    AddListener(new RS232Listener());
                    Console.WriteLine("Rs232 medium started");
                    AddListener(new DotNetRemotingListener());
                    Console.WriteLine(".net remoting medium started");
                    break;

                default:
                    Console.WriteLine("Unknown service");
                    break;
            }
        }

        //Zatrzymywanie medium
        void StopMedium(string mediumName)
        {
            switch (mediumName)
            {
                case "tcp":
                    RemoveListener(_listeners.Where(listener => listener is TCPListener).First());
                    break;

                case "udp":
                    RemoveListener(_listeners.Where(listener => listener is UDPListener).First());
                    break;

                case "files":
                    RemoveListener(_listeners.Where(listener => listener is FilesProtocolListener).First());
                    break;

                case "RS232":
                    RemoveListener(_listeners.Where(listener => listener is RS232Listener).First());
                    break;

                case ".netr":
                    RemoveListener(_listeners.Where(listener => listener is DotNetRemotingListener).First());
                    break;

                default:
                    Console.WriteLine("Unknown service");
                    break;
            }
        }

        //Zamykanie całego serwera
        void StopServer()
        {
            _listeners.ForEach(listener => listener.Stop());
            _communicators.ForEach(communicator => communicator.Stop());

            _listeners.Clear();
            _communicators.Clear();
            _services.Clear();

            Console.WriteLine("Server stopped");
        }

        static void Main()
        {
            Console.WriteLine("Server logs");
            Server _server = new Server();
            string command;
            string[] commandParams;
            
            //DEBUG
            //string[] commandParams = {"conf", "start-all"};
            //_server.ConfigCommander(commandParams);

            while (true)
            {
                command = Console.ReadLine();
                commandParams = command.Split(' ');

                if (commandParams[0] == "exit")
                {
                    _server.StopServer();
                    return;
                }

                _server.ConfigCommander(commandParams);
            }
        }
    }
}
