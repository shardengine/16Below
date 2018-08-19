using System;

namespace Server
{
    public delegate void AnimateRequestEventHandler(AnimateRequestEventArgs e);
    public delegate void OpenDoorMacroEventHandler(OpenDoorMacroEventArgs e);

    public static partial class EventSink
    {
        public static event AnimateRequestEventHandler AnimateRequest;
        public static event OpenDoorMacroEventHandler OpenDoorMacroUsed;

        public static void InvokeAnimateRequest(AnimateRequestEventArgs e)
        {
            AnimateRequest?.Invoke(e);
        }

        public static void InvokeOpenDoorMacroUsed(OpenDoorMacroEventArgs e)
        {
            OpenDoorMacroUsed?.Invoke(e);
        }

        private static void ResetMacro()
        {
            AnimateRequest = null;
            OpenDoorMacroUsed = null;
        }
    }

    public class AnimateRequestEventArgs : EventArgs
    {
        private readonly Mobile m_Mobile;
        private readonly string m_Action;

        public Mobile Mobile { get { return m_Mobile; } }
        public string Action { get { return m_Action; } }

        public AnimateRequestEventArgs(Mobile m, string action)
        {
            m_Mobile = m;
            m_Action = action;
        }
    }

    public class OpenDoorMacroEventArgs : EventArgs
    {
        private readonly Mobile m_Mobile;

        public Mobile Mobile { get { return m_Mobile; } }

        public OpenDoorMacroEventArgs(Mobile mobile)
        {
            m_Mobile = mobile;
        }
    }
}
