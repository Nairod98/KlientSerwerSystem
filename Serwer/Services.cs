using Common;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    public class PingPongServiceModule : IServiceModule
    {
        public string AnswerCommand(string command)
        {
            return PingPong.Pong(command.Split('\n')[0]);
        }
    }

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
                    return Chat.Msg(_args[2], _args[3], _message[1], ref Messages);
                case "get":
                    List<Message> userMessages = Chat.Get(_args[2], Messages);
                    return userMessages.Count > 0 ? String.Join("; ", userMessages.Select(m => m.ToString())) + "\n" : "There are no messages for this user.\n";
                default:
                    return "Unknown command";
            }
        }
    }

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
                    return "Unknown command";
            }
        }
    }
}
