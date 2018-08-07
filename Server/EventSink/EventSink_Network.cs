using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using Server.Accounting;
using Server.Network;

namespace Server
{
    public delegate void SocketConnectEventHandler(SocketConnectEventArgs e);
    public delegate void ConnectedEventHandler(ConnectedEventArgs e);
    public delegate void DisconnectedEventHandler(DisconnectedEventArgs e);
    public delegate void GameLoginEventHandler(GameLoginEventArgs e);
    public delegate void LoginEventHandler(LoginEventArgs e);
    public delegate void LogoutEventHandler(LogoutEventArgs e);
    public delegate void ServerListEventHandler(ServerListEventArgs e);

    public static partial class EventSink
    {
        public static event SocketConnectEventHandler SocketConnect;
        public static event ConnectedEventHandler Connected;
        public static event DisconnectedEventHandler Disconnected;
        public static event GameLoginEventHandler GameLogin;
        public static event LoginEventHandler Login;
        public static event LogoutEventHandler Logout;
        public static event ServerListEventHandler ServerList;

        public static void InvokeSocketConnect(SocketConnectEventArgs e)
        {
            SocketConnect?.Invoke(e);
        }

        public static void InvokeConnected(ConnectedEventArgs e)
        {
            Connected?.Invoke(e);
        }

        public static void InvokeDisconnected(DisconnectedEventArgs e)
        {
            Disconnected?.Invoke(e);
        }

        public static void InvokeGameLogin(GameLoginEventArgs e)
        {
            GameLogin?.Invoke(e);
        }

        public static void InvokeLogin(LoginEventArgs e)
        {
            Login?.Invoke(e);
        }

        public static void InvokeLogout(LogoutEventArgs e)
        {
            Logout?.Invoke(e);
        }

        public static void InvokeServerList(ServerListEventArgs e)
        {
            ServerList?.Invoke(e);
        }

        private static void ResetNetwork()
        {
            SocketConnect = null;
            Connected = null;
            Disconnected = null;
            GameLogin = null;
            Login = null;
            Logout = null;
            ServerList = null;
        }
    }

    public class SocketConnectEventArgs : EventArgs
    {
        private readonly Socket m_Socket;

        public Socket Socket { get { return m_Socket; } }
        public bool AllowConnection { get; set; }

        public SocketConnectEventArgs(Socket s)
        {
            m_Socket = s;
            AllowConnection = true;
        }
    }

    public class ConnectedEventArgs : EventArgs
    {
        private readonly Mobile m_Mobile;

        public Mobile Mobile { get { return m_Mobile; } }

        public ConnectedEventArgs(Mobile m)
        {
            m_Mobile = m;
        }
    }

    public class DisconnectedEventArgs : EventArgs
    {
        private readonly Mobile m_Mobile;

        public Mobile Mobile { get { return m_Mobile; } }

        public DisconnectedEventArgs(Mobile m)
        {
            m_Mobile = m;
        }
    }

    public class GameLoginEventArgs : EventArgs
    {
        private readonly NetState m_State;
        private readonly string m_Username;
        private readonly string m_Password;

        public NetState State { get { return m_State; } }
        public string Username { get { return m_Username; } }
        public string Password { get { return m_Password; } }
        public bool Accepted { get; set; }
        public CityInfo[] CityInfo { get; set; }

        public GameLoginEventArgs(NetState state, string un, string pw)
        {
            m_State = state;
            m_Username = un;
            m_Password = pw;
        }
    }

    public class LoginEventArgs : EventArgs
    {
        private readonly Mobile m_Mobile;

        public Mobile Mobile { get { return m_Mobile; } }

        public LoginEventArgs(Mobile mobile)
        {
            m_Mobile = mobile;
        }
    }

    public class LogoutEventArgs : EventArgs
    {
        private readonly Mobile m_Mobile;

        public Mobile Mobile { get { return m_Mobile; } }

        public LogoutEventArgs(Mobile m)
        {
            m_Mobile = m;
        }
    }

    public class ServerListEventArgs : EventArgs
    {
        private readonly NetState m_State;
        private readonly IAccount m_Account;
        private readonly List<ServerInfo> m_Servers;

        public NetState State { get { return m_State; } }
        public IAccount Account { get { return m_Account; } }
        public bool Rejected { get; set; }
        public List<ServerInfo> Servers { get { return m_Servers; } }

        public void AddServer(string name, IPEndPoint address)
        {
            AddServer(name, 0, TimeZone.CurrentTimeZone, address);
        }

        public void AddServer(string name, int fullPercent, TimeZone tz, IPEndPoint address)
        {
            m_Servers.Add(new ServerInfo(name, fullPercent, tz, address));
        }

        public ServerListEventArgs(NetState state, IAccount account)
        {
            m_State = state;
            m_Account = account;
            m_Servers = new List<ServerInfo>();
        }
    }
}
