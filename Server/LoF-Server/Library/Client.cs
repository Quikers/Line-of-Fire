using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Client {
        public int UserID;
        public string Username;
        public TcpClient Socket;
        public Task ListenTask;
    }

    public class User {
        public string id;
        public string username;
        public string account_type;
        public string email;
        public string created;
        public string editted;
    }
}
