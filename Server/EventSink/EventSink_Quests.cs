using System;

namespace Server
{
    public delegate void QuestGumpRequestHandler(QuestGumpRequestArgs e);

    public static partial class EventSink
    {
        public static event QuestGumpRequestHandler QuestGumpRequest;

        public static void InvokeQuestGumpRequest(QuestGumpRequestArgs e)
        {
            QuestGumpRequest?.Invoke(e);
        }

        private static void ResetQuests()
        {
            QuestGumpRequest = null;
        }
    }

    public class QuestGumpRequestArgs : EventArgs
    {
        private readonly Mobile m_Mobile;

        public Mobile Mobile { get { return m_Mobile; } }

        public QuestGumpRequestArgs(Mobile mobile)
        {
            m_Mobile = mobile;
        }
    }
}
