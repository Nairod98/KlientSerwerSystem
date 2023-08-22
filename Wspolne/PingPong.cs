using System;

namespace Common
{
    public class PingPong
    {
        /// <summary>
        /// Metoda do wykonywania komendy ping, która tworzy ¿¹danie do serwera. Generuje bajty wg. losowych znaków ASCII je¿eli 
        /// d³ugoœæ komendy jest za ma³a to zostanie przyjêta pierwotna wartoœæ
        /// </summary>
        /// <param name="requestSize"></param>
        /// <param name="responseSize"></param>
        /// <returns></returns>
        public static string Ping(int requestSize, int responseSize)
        {
            string request = string.Format("ping {0} ", responseSize);
            int length = (requestSize - request.Length - 2) > 0 ? (requestSize - request.Length - 2) : requestSize;
            request += GenerateBytes(length) + "\n";

            return request;
        }

        /// <summary>
        /// Metoda do generowania bajtów wg. ASCII
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private static string GenerateBytes(int count)
        {
            Random r = new Random();

            char[] arr = new char[count];
            for (int i = 0; i < count; i++) arr[i] = (char)r.Next(48, 122);

            return new string(arr);
        }

        public static string Pong(string line)
        {
            string[] answer = line.Split();
            string response = "pong ";
            int length = (int.Parse(answer[1]) - response.Length - 1 > 0) ? (int.Parse(answer[1]) - response.Length - 1) : int.Parse(answer[1]);
            response += GenerateBytes(length) + "\n";

            return response;
        }
    }
}