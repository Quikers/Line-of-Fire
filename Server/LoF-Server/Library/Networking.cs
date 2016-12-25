using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Library {

    public class Networking {
        public const int MAXBUFFER = 1024;

        public static Packet ReceiveData(TcpClient client) {
            try {
                byte[] buffer = new byte[MAXBUFFER];
                NetworkStream stream = client.GetStream();

                int read = stream.Read(buffer, 0, buffer.Length);
                string data = Encoding.ASCII.GetString(new List<byte>(buffer).GetRange(0, read).ToArray());

                return new Packet(data);
            }
            catch (Exception ex) {
                if (!ex.ToString().Contains("WSACancelBlockingCall") &&
                    !ex.ToString().Contains("connection reset") &&
                    !ex.ToString().Contains("valid TcpMessage"))
                    Console.WriteLine("\n" + ex + "\n");

                client.Close();
                return new Packet();
            }
        }

        public static void SendData(TcpClient client, Packet packet) {
            try {
                byte[] buffer = Encoding.ASCII.GetBytes(packet.ToString());
                NetworkStream stream = client.GetStream();

                stream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex) {
                if (!ex.ToString().Contains("WSACancelBlockingCall") &&
                    !ex.ToString().Contains("connection reset") &&
                    !ex.ToString().Contains("valid TcpMessage"))
                    Console.WriteLine("\n" + ex + "\n");

                client.Close();
            }
        }
    }
}
