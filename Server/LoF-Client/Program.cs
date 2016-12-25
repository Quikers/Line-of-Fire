using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Sockets;
using Newtonsoft.Json;
using Library;

namespace LoF_Client {
    public class Program {

        private static Client client;
        private static bool programActive;
        private static List<string> _playerList = new List<string>();

        private static void Main(string[] args) {
            programActive = true;

            Console.Write("Username: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            client = new Client {
                Username = username,
                Socket = new TcpClient()
            };

            client.Socket.Connect("137.74.168.49", 25001);
            Networking.SendData(client.Socket, new Packet(
                username,
                "Server",
                PacketType.Login,
                new [] { "Username", username, "Password", password }
            ));

            Task listen = new Task(ListenForServerInput);
            listen.Start();
            
            while (programActive)
                HandleCommands();

            while (client.Socket.Connected) client.Socket.Close();
            Console.WriteLine("Client closed.");
        }

        private static void HandleCommands() {
            string result = Console.ReadLine();
            if (result == null) return;

            string[] parameters = result.Split(' ');
            string command = parameters[0];
            parameters[0] = result;

            switch (command) {
                default: {
                        Console.WriteLine("\"{0}\" was not recognized as a LoF server command", command);
                    }
                    break;
                case "exit": case "quit": case "close": {
                        programActive = false;
                    }
                    break;
                case "online": case "players": case "list": case "ls": {
                        if (_playerList.Count == 0) {
                            Console.WriteLine("No users online to display");
                            break;
                        }

                        Console.WriteLine("\nCurrent connected players\n:");
                        foreach (string username in _playerList) {
                            Console.WriteLine("{0}", username);
                        }

                        Console.WriteLine("");
                    }
                    break;
            }
        }

        private static void HandlePackets(Packet packet) {
            if (packet == null || packet.Type == PacketType.None) return;

            switch (packet.Type) {
                default:
                    Console.WriteLine("Received a packet with an unknown type: {0}", packet.Type);
                    break;
                case PacketType.Broadcast: case PacketType.ChatMessage: case PacketType.LoginFailed: case PacketType.LoginSuccess:
                    Console.WriteLine("{0} > {1}: {2}", packet.From, packet.To, packet.Variables["Message"]);
                    break;
                case PacketType.PlayerList:
                    _playerList = new List<string>(packet.Variables["List"].Split(';'));
                    break;
            }
        }

        private static void ListenForServerInput() {
            while (client.Socket.Connected) {
                Packet packet = Networking.ReceiveData(client.Socket);

                HandlePackets(packet);
            }
        }
    }
}
