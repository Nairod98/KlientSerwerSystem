using System;
using System.Collections.Generic;

namespace Common
{
    public class Message
    {
        private readonly string _receiver;
        private readonly string _sender;
        private readonly string _messageText;
        private readonly DateTime _dateTimeSent;

        public string Receiver { get { return _receiver; } }

        public Message(string receiver, string sender, string messageText)
        {
            _receiver = receiver;
            _sender = sender;
            _messageText = messageText;
            _dateTimeSent = DateTime.Now;
        }

        public override string ToString()
        {
            return string.Format("{0} | From {1}: {2}", _dateTimeSent, _sender, _messageText);
        }
    }

    public class Chat
    {
        public static string Msg(string nicknames, string myNickname, string message, ref List<Message> _messages)
        {
            string[] _nicknames = nicknames.Split(';');

            foreach (string nickname in _nicknames) _messages.Add(new Message(nickname, myNickname, message));

            return string.Format("Message sent from {0} to {1}\n", myNickname, nicknames);
        }

        public static List<Message> Get(string nickname, List<Message> _messages)
        {
            return _messages.FindAll(_message => _message.Receiver == nickname);
        }
    }
}