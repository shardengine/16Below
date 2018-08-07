using System;

namespace Server
{
    public delegate void HelpRequestEventHandler(HelpRequestEventArgs e);

    public static partial class EventSink
    {
        public static event HelpRequestEventHandler HelpRequest;

        public static void InvokeHelpRequest(HelpRequestEventArgs e)
        {
            HelpRequest?.Invoke(e);
        }

        private static void ResetHelp()
        {
            HelpRequest = null;
        }
    }

    public class HelpRequestEventArgs : EventArgs
    {
        private readonly Mobile m_Mobile;

        public Mobile Mobile { get { return m_Mobile; } }

        public HelpRequestEventArgs(Mobile m)
        {
            m_Mobile = m;
        }
    }
}
