using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;

using Library;
using Newtonsoft.Json;

namespace LoF_Server {
    class Program {
        
        private static bool _serverActive = true;
        private static TcpListener _server;

        private static Dictionary<int, Client> _playerList = new Dictionary<int, Client>();

        static void Main(string[] args) {
            Console.WriteLine("Starting up Line of Fire Social Server...");

            _server = new TcpListener(IPAddress.Any, 25001);
            _server.Start();

            Task listenForClients = new Task(Listen);
            listenForClients.Start();

            while (_serverActive)
                HandleCommands();

            Console.WriteLine("Sever closed.");
        }

        private static int GetUserIDByString(string identifier) {
            int UserID;
            if (!int.TryParse(identifier, out UserID)) {
                foreach (KeyValuePair<int, Client> pair in _playerList) {
                    if (pair.Value.Username != identifier) {
                        return -1;
                    }

                    UserID = pair.Value.UserID;
                }
            }

            if (!_playerList.ContainsKey(UserID)) {
                return -1;
            }

            return UserID;
        }

        private static void ValidateClientSockets() {
            try {
                foreach (KeyValuePair<int, Client> pair in _playerList) {
                    if (!pair.Value.Socket.Connected)
                        DisconnectClient(pair.Value, pair.Value.Username + "(" + pair.Value.UserID + ") has left the server", true);
                }
            }
            catch (Exception ex) {
                if (ex.ToString().Contains("modified"))
                    ValidateClientSockets();
                else
                    Console.WriteLine("\n" + ex + "\n");
            }
        }

        private static void HandleCommands() {
            string result = Console.ReadLine();
            ValidateClientSockets();

            if (result == null) return;
            string[] parameters = result.Split(' ');
            string command = parameters[0];
            parameters[0] = result;

            switch (command) {
                default: {
                    Console.WriteLine("\"{0}\" was not recognized as a LoF server command", command);
                } break;
                case "newclient": case "nc": {
                    Process.Start("LoF-Client.exe");
                } break;
                case "exit": case "quit": case "close": {
                    _serverActive = false;
                } break;
                case "pm": case "whisper": {
                    int UserID = GetUserIDByString(parameters[1]);
                    if (UserID == -1) {
                        Console.WriteLine("User \"{0}\" is not online", parameters[1]);
                        break;
                    } 

                    List<string> message = new List<string>();
                    for (int i = 2; i < parameters.Length; i++)
                        message.Add(parameters[i]);

                    Networking.SendData(_playerList[UserID].Socket, new Packet(
                        "Server",
                        UserID.ToString(),
                        PacketType.ChatMessage,
                        new[] { "Message", string.Join(" ", message.ToArray()) }
                    ));
                } break;
                case "broadcast": case "say": case "message": {
                    List<string> message = new List<string>();

                    for (int i = 1; i < parameters.Length; i++)
                        message.Add(parameters[i]);

                    Broadcast(new Packet(
                        "Server",
                        "Everyone",
                        PacketType.Broadcast,
                        new[] {"Message", string.Join(" ", message.ToArray())}
                    ));
                } break;
                case "online": case "players": case "list": case "ls": {
                    if (_playerList.Count == 0) {
                        Console.WriteLine("No users online to display");
                        break;
                    }

                    Console.WriteLine("\nCurrent connected players:\n");
                    foreach (KeyValuePair<int, Client> pair in _playerList) {
                        Console.WriteLine("{0}({1}) - {2}", pair.Value.Username, pair.Value.UserID, pair.Value.Socket.Client.RemoteEndPoint);
                    }

                    Console.WriteLine("");
                } break;
                case "kick": {
                    int UserID = GetUserIDByString(parameters[1]);
                    if (UserID == -1) {
                        Console.WriteLine("User \"{0}\" is not online", parameters[1]);
                        break;
                    }

                    DisconnectClient(_playerList[UserID], "\"" + _playerList[UserID].Username + "\" was kicked from the server", true);
                } break;
            }
        }

        private static void Listen() {
            Console.WriteLine("Waiting for incoming connections...");

            _serverActive = true;
            while (_serverActive) {
                TcpClient client = _server.AcceptTcpClient();
                Packet packet = Networking.ReceiveData(client);

                HandlePacket(packet, client);
            }
        }

        private static void HandlePacket(Packet packet, TcpClient client) {
            ValidateClientSockets();
            
            if (packet == null || packet.Type == PacketType.None) return;
            try {
                switch (packet.Type) {
                    default:
                        Console.WriteLine("Received a packet with an unknown type: {0}", packet.Type);
                        break;
                    case PacketType.Login: {
                        Console.WriteLine("User login requested");
                        
                        // Check if user is already logged in
                        if (GetUserIDByString(packet.Variables["Username"]) != -1) {
                            Networking.SendData(client, new Packet(
                                "Server", 
                                packet.Variables["Username"], 
                                PacketType.LoginFailed, 
                                new [] { "Message", "Login failed, user is already logged in."}
                            ));
                            DisconnectClient(new Client { Socket = client}, "User \"" + packet.Variables["Username"] + "\" is already logged in");
                            break;
                        }

                        // Use given credentials to log user in and get user info
                        using (var httpClient = new HttpClient()) {
                            string json = httpClient.GetStringAsync("http://137.74.168.49/api/checklogin/" + packet.Variables["Username"] + "/" + packet.Variables["Password"]).Result;
                            
                            // If credentials are false, disconnect the client
                            if (json == "false") {
                                Networking.SendData(client, new Packet(
                                    "Server",
                                    packet.Variables["Username"],
                                    PacketType.LoginFailed,
                                    new [] { "Message", "Invalid credentials" }
                                ));

                                DisconnectClient(new Client { Socket = client }, "Login failed, invalid credentials");
                                break;
                            }

                            // If credentials are true, create a new client
                            User user = JsonConvert.DeserializeObject<User>(json);
                            Client player = new Client {
                                UserID = int.Parse(user.id),
                                Username = user.username,
                                Socket = client
                            };
                            
                            // Add the new client to the list of connected clients and listen for input
                            _playerList.Add(player.UserID, player);

                            _playerList[player.UserID].ListenTask = new Task( () => ListenForClientInput(_playerList[player.UserID]) );
                            _playerList[player.UserID].ListenTask.Start();
                        }
                    } break;
                    case PacketType.Logout: {
                        int UserID = int.Parse(packet.Variables["UserID"]);

                        if (_playerList.ContainsKey(UserID)) {
                            DisconnectClient(_playerList[UserID], "\"" + _playerList[UserID].Username + "\" left the server", true);
                        }
                    } break;
                }
            }
            catch (Exception ex) {
                Console.WriteLine("\n" + ex + "\n");
            }
        }

        private static void ListenForClientInput(Client client) {
            Networking.SendData(client.Socket, new Packet(
                "Server",
                client.Username,
                PacketType.LoginSuccess,
                new [] { "Message", "Login successful"}
            ));

            BroadcastPlayerList();
            Console.WriteLine("{0}({1}) succcessfully joined the server", client.Username, client.UserID);
            while (client.Socket.Connected) {
                Packet packet = Networking.ReceiveData(client.Socket);

                HandlePacket(packet, client.Socket);
            }
        }

        private static void Broadcast(Packet packet, bool logMessage = true) {
            if (_playerList.Count == 0) {
                Console.WriteLine("No players connected to broadcast to");
                return;
            }

            foreach (KeyValuePair<int, Client> pair in _playerList)
                Networking.SendData(pair.Value.Socket, packet);

            if (!logMessage) return;
            if (packet.Variables.ContainsKey("Message"))
                Console.WriteLine("{0} > {1}: {2}", packet.From, packet.To, packet.Variables["Message"]);
            else
                Console.WriteLine("{0} > {1}:\n{2}", packet.From, packet.To, packet);
        }

        private static void BroadcastPlayerList() {
            List<string> list = new List<string>();
            foreach (KeyValuePair<int, Client> pair in _playerList)
                list.Add(pair.Value.Username);

            Broadcast(new Packet(
                "Server",
                "Everyone",
                PacketType.PlayerList,
                new[] { "List", string.Join(";", list.ToArray()) }
            ));
        }

        private static void DisconnectClient(Client client, string message = "", bool publicMessage = false) {
            while (client.Socket.Connected) client.Socket.Close();
            if (_playerList.ContainsKey(client.UserID))
                _playerList.Remove(client.UserID);

            BroadcastPlayerList();
            if (message == "") return;

            if (publicMessage) {
                Broadcast(new Packet(
                    "Server",
                    "Everyone",
                    PacketType.Broadcast,
                    new[] {"Message", message}
                ));
            } else Console.WriteLine(message);
        }
    }
}
