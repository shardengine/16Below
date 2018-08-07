using System;

namespace Server
{
    public delegate void ServerStartedEventHandler();
    public delegate void CrashedEventHandler(CrashedEventArgs e);
    public delegate void ShutdownEventHandler(ShutdownEventArgs e);

    public static partial class EventSink
    {
        public static event ServerStartedEventHandler ServerStarted;
        public static event CrashedEventHandler Crashed;
        public static event ShutdownEventHandler Shutdown;

        public static void InvokeServerStarted()
        {
            ServerStarted?.Invoke();
        }

        public static void InvokeCrashed(CrashedEventArgs e)
        {
            Crashed?.Invoke(e);
        }

        public static void InvokeShutdown(ShutdownEventArgs e)
        {
            Shutdown?.Invoke(e);
        }

        private static void ResetServer()
        {
            ServerStarted = null;
            Crashed = null;
            Shutdown = null;
        }
    }
    
    public class ShutdownEventArgs : EventArgs
    { }

    public class CrashedEventArgs : EventArgs
    {
        private readonly Exception m_Exception;

        public Exception Exception { get { return m_Exception; } }
        public bool Close { get; set; }

        public CrashedEventArgs(Exception e)
        {
            m_Exception = e;
        }
    }

}
