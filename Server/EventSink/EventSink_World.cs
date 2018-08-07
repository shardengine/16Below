using System;

namespace Server
{
    public delegate void WorldLoadEventHandler();
    public delegate void BeforeWorldSaveEventHandler(BeforeWorldSaveEventArgs e);
    public delegate void WorldSaveEventHandler(WorldSaveEventArgs e);
    public delegate void AfterWorldSaveEventHandler(AfterWorldSaveEventArgs e);

    public static partial class EventSink
    {
        public static event WorldLoadEventHandler WorldLoad;
        public static event BeforeWorldSaveEventHandler BeforeWorldSave;
        public static event WorldSaveEventHandler WorldSave;
        public static event AfterWorldSaveEventHandler AfterWorldSave;

        public static void InvokeWorldLoad()
        {
            WorldLoad?.Invoke();
        }

        public static void InvokeBeforeWorldSave(BeforeWorldSaveEventArgs e)
        {
            BeforeWorldSave?.Invoke(e);
        }

        public static void InvokeWorldSave(WorldSaveEventArgs e)
        {
            WorldSave?.Invoke(e);
        }

        public static void InvokeAfterWorldSave(AfterWorldSaveEventArgs e)
        {
            AfterWorldSave?.Invoke(e);
        }

        private static void ResetWorld()
        {
            WorldLoad = null;
            BeforeWorldSave = null;
            WorldSave = null;
            AfterWorldSave = null;
        }
    }
    
    public class WorldSaveEventArgs : EventArgs
    {
        private readonly bool m_Msg;

        public bool Message { get { return m_Msg; } }

        public WorldSaveEventArgs(bool msg)
        {
            m_Msg = msg;
        }
    }

    public class BeforeWorldSaveEventArgs : EventArgs
    {
        public BeforeWorldSaveEventArgs()
        {
        }
    }

    public class AfterWorldSaveEventArgs : EventArgs
    {
        public AfterWorldSaveEventArgs()
        {
        }
    }
}
