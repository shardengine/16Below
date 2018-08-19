using System;

namespace Server
{
    public delegate void ChatRequestEventHandler(ChatRequestEventArgs e);

    public static partial class EventSink
    {
        public static event ChatRequestEventHandler ChatRequest;

        public static void InvokeChatRequest(ChatRequestEventArgs e)
        {
            ChatRequest?.Invoke(e);
        }

        private static void ResetChat()
        {
            ChatRequest = null;
        }
    }

    public class ChatRequestEventArgs : EventArgs
    {
        private readonly Mobile m_Mobile;

        public Mobile Mobile { get { return m_Mobile; } }

        public ChatRequestEventArgs(Mobile mobile)
        {
            m_Mobile = mobile;
        }
    }
}
