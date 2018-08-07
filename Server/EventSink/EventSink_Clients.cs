using System;
using Server.Network;

namespace Server
{
    public delegate void ClientVersionReceivedHandler(ClientVersionReceivedArgs e);

    public static partial class EventSink
    {
        public static event ClientVersionReceivedHandler ClientVersionReceived;

        public static void InvokeClientVersionReceived(ClientVersionReceivedArgs e)
        {
            ClientVersionReceived?.Invoke(e);
        }

        private static void ResetClients() 
        {
            ClientVersionReceived = null;
        }
    }

    public class ClientVersionReceivedArgs : EventArgs
    {
        private readonly NetState m_State;
        private readonly ClientVersion m_Version;

        public NetState State { get { return m_State; } }
        public ClientVersion Version { get { return m_Version; } }

        public ClientVersionReceivedArgs(NetState state, ClientVersion cv)
        {
            m_State = state;
            m_Version = cv;
        }
    }
}
