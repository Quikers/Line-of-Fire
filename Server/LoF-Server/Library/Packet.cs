﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Library {
    public enum PacketType {
        None = 0,

        Login = 1,
        LoginFailed = 2,
        LoginSuccess = 3,
        Logout = 4,
        Broadcast = 5,
        ChatMessage = 6,
        PlayerList = 7,
        RequestGameSlot = 8
    }

    public static class PacketParser {
        public static Packet Parse(string data) {
            Packet packet = new Packet();

            if (!data.Contains("\\1\\"))
                throw new Exception("String does not contain a valid TcpMessage Packet.");

            string[] variables = data.Split(new[] { "\\1\\" }, StringSplitOptions.None);
            foreach (string pair in variables) {
                if (!pair.Contains("\\2\\"))
                    throw new Exception("String does not contain a valid TcpMessage Packet.");

                string[] keyValue = pair.Split(new[] { "\\2\\" }, StringSplitOptions.None);
                switch (keyValue[0]) {
                    default:
                        packet.Variables.Add(keyValue[0], keyValue[1]);
                        break;
                    case "From":
                        packet.From = keyValue[1];
                        break;
                    case "To":
                        packet.To = keyValue[1];
                        break;
                    case "Type":
                        try {
                            packet.Type = (PacketType)Enum.Parse(typeof(PacketType), keyValue[1]);
                        } catch (Exception) {
                            throw new Exception("Type \"" + keyValue[1] + "\" is not an underlying value of TcpMessageType.");
                        }
                        break;
                }
            }

            return packet;
        }
    }

    public class Packet {
        public string From = "";
        public string To = "";
        public PacketType Type = PacketType.None;
        public Dictionary<string, string> Variables = new Dictionary<string, string>();

        public Packet() { }
        public Packet(string data) {
            Packet p = PacketParser.Parse(data);

            if (p != null) {
                From = p.From;
                To = p.To;
                Type = p.Type;
                Variables = p.Variables;
            } else
                Console.WriteLine("Packet was not parsed correctly.\n{0} contains errors", this);
        }

        public Packet(string from, string to, PacketType type, Dictionary<string, string> variables = null) {
            From = from;
            To = to;
            Type = type;
            Variables = variables;
        }

        public Packet(string from, string to, PacketType type, string[] variable) {
            From = from;
            To = to;
            Type = type;

            if (variable.Length <= 0)
                return;
            for (int i = 0; i < variable.Length; i += 2) {
                Variables.Add(variable[i], variable[i + 1]);
            }
        }

        public override string ToString() {
            string str = "From\\2\\" + From + "\\1\\To\\2\\" + To + "\\1\\Type\\2\\" + Type;

            if (Variables.Count <= 0)
                return str;
            str += "\\1\\" + string.Join("\\1\\", Variables.Select(pair => pair.Key + "\\2\\" + pair.Value).ToArray());

            return str;
        }
    }
}
