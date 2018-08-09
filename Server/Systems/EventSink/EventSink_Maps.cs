using System;

namespace Server
{
    public delegate void MapChangedEventHandler(MapChangedEventArgs e);

    public static partial class EventSink
    {
        public static event MapChangedEventHandler MapChanged;

        public static void InvokeMapChanged(MapChangedEventArgs e)
        {
            MapChanged?.Invoke(e);
        }

        private static void ResetMaps()
        {
            MapChanged = null;
        }
    }

    public class MapChangedEventArgs : EventArgs
    {
        private IEntity m_Entity;
        private Map m_OldMap;

        public IEntity Entity { get { return m_Entity; } }
        public Map OldMap { get { return m_OldMap; } }

        public MapChangedEventArgs(IEntity entity, Map oldMap)
        {
            m_Entity = entity;
            m_OldMap = oldMap;
        }
    }
}
