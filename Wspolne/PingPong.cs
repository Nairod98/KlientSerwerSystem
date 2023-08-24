using System;

namespace Common
{
    public class PingPong
    {
        
        //Metoda do wykonywania komendy ping, która tworzy ¿¹danie do serwera. Generuje bajty wg. losowych znaków ASCII
        public static string Ping(int requestSize, int responseSize)
        {
            string request = string.Format("ping {0} ", responseSize);
            int length = (requestSize - request.Length - 2) > 0 ? (requestSize - request.Length - 2) : requestSize;
            request += GenerateBytes(length) + "\n";

            return request;
        }

        //Metoda do generowania bajtów wg. ASCII
        private static string GenerateBytes(int count)
        {
            Random r = new Random();

            char[] arr = new char[count];
            for (int i = 0; i < count; i++) arr[i] = (char)r.Next(48, 122); //znaki '0-9', ':;<=>?@', 'A-Z', '[\]^_'', 'a-z'

            return new string(arr);
        }

        //OdpowiedŸ na zawo³anie 'ping'
        public static string Pong(string line)
        {
            string[] answer = line.Split();
            string response = "pong ";
            int length = (int.Parse(answer[1]) - response.Length - 1 > 0) ? (int.Parse(answer[1]) - response.Length - 1) : 
                int.Parse(answer[1]);
            response += GenerateBytes(length) + "\n";

            return response;
        }
    }
}