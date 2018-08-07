using System;

namespace Server
{
    public delegate void OnEnterRegionEventHandler(OnEnterRegionEventArgs e);

    public static partial class EventSink
    {
        public static event OnEnterRegionEventHandler OnEnterRegion;

        public static void InvokeOnEnterRegion(OnEnterRegionEventArgs e)
        {
            OnEnterRegion?.Invoke(e);
        }

        private static void ResetRegions() 
        {
            OnEnterRegion = null;
        }
    }

    public class OnEnterRegionEventArgs : EventArgs
    {
        private readonly Mobile m_From;
        private readonly Region m_Region;

        public OnEnterRegionEventArgs(Mobile from, Region region)
        {
            m_From = from;
            m_Region = region;
        }

        public Mobile From { get { return m_From; } }
        public Region Region { get { return m_Region; } }
    }
}
