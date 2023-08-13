using System;
using System.IO;

namespace Server.FilesProtocol
{
    public class FilesProtocolCommunicator : ICommunicator
    {
        private readonly string fileName;
        public FilesProtocolCommunicator(string fileName)
        {
            this.fileName = fileName;
        }

        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            StreamReader streamReader = null;
            while (streamReader == null)
            {
                try
                {
                    streamReader = new StreamReader(fileName);
                }
                catch (Exception) { }
            }

            while (!streamReader.EndOfStream)
            {
                File.WriteAllText(fileName.Replace(".txt", ".data"), onCommand(streamReader.ReadLine()));
                Console.Write(string.Format("FilesProtocol | get: {0}", fileName));
            }
            streamReader.Close();
        }

        public void Stop() { }
    }
}