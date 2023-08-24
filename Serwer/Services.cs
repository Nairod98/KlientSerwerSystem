using Common;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    //Odpowiadanie na ping
    public class PingPongServiceModule : IServiceModule
    {
        public string AnswerCommand(string command)
        {
            return PingPong.Pong(command.Split('\n')[0]);
        }
    }

    //Obsługa asynchronicznego czatu
    public class ChatServiceModule : IServiceModule
    {
        private List<Message> Messages = new List<Message>();
        public string AnswerCommand(string command)
        {
            string[] _args = command.Split(' ');

            switch (_args[1])
            {
                case "msg":
                    string[] _message = command.Split('"');

                    if (_args.Length < 5 || _message[0].Split(' ').Length != 5)
                    {
                        return "Invalid command. Try according to this pattern: 'chat msg receiver sender \"message\"'";
                    }

                    return Chat.Msg(_args[2], _args[3], _message[1], ref Messages);

                case "get":
                    if (_args.Length != 4)
                    {
                        return "Invalid command. Try 'chat get username'";
                    }

                    List<Message> userMessages = Chat.Get(_args[2], Messages);
                    return userMessages.Count > 0 ? String.Join("\n", userMessages.Select(m => m.ToString())) + "\n" : 
                        "There are no messages for this user.\n";

                default:
                    return "Invalid command. Try 'chat msg' or 'chat get'";
            }
        }
    }

    //Obsługa plików
    public class FileServiceModule : IServiceModule
    {
        public string AnswerCommand(string command)
        {
            string[] _args = command.Split(' ');

            switch (_args[1].Split('\n')[0])
            {
                case "list":
                    return Files.List();

                case "get":
                    return Files.Get(_args[2].Split('\n')[0]);

                case "put":
                    return Files.Put(_args[2], _args[3].Split('\n')[0]);

                default:
                    return "Invalid command. Try 'file list', 'file get' or 'file put'";
            }
        }
    }
}
