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
        //Polecenie 'msg'
        public static string Msg(string nicknames, string myNickname, string message, ref List<Message> _messages)
        {
            string[] _nicknames = nicknames.Split(';');

            //Mo¿na wysy³aæ wiadomoœci do wielu osób jednoczeœnie
            foreach (string nickname in _nicknames) _messages.Add(new Message(nickname, myNickname, message));

            return string.Format("Message sent from {0} to {1}\n", myNickname, nicknames);
        }

        //Polecenie 'get'
        public static List<Message> Get(string nickname, List<Message> _messages)
        {
            //Zwraca otrzymane wiadomoœci
            return _messages.FindAll(_message => _message.Receiver == nickname);
        }
    }
}