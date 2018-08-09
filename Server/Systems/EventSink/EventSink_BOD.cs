using System;

namespace Server
{
    public delegate void BODUsedEventHandler(BODUsedEventArgs e);
    public delegate void BODOfferEventHandler(BODOfferEventArgs e);

    public static partial class EventSink
    {
        public static event BODUsedEventHandler BODUsed;
        public static event BODOfferEventHandler BODOffered;

        public static void InvokeBODUsed(BODUsedEventArgs e)
        {
            BODUsed?.Invoke(e);
        }

        public static void InvokeBODOffered(BODOfferEventArgs e)
        {
            BODOffered?.Invoke(e);
        }

        private static void ResetBOD()
        {
            BODUsed = null;
            BODOffered = null;
        }
    }
    
    public class BODUsedEventArgs : EventArgs
    {
        public Mobile User { get; private set; }
        public Item BODItem { get; private set; }

        public BODUsedEventArgs(Mobile m, Item i)
        {
            User = m;
            BODItem = i;
        }
    }

    public class BODOfferEventArgs : EventArgs
    {
        public Mobile Player { get; private set; }
        public Mobile Vendor { get; private set; }

        public BODOfferEventArgs(Mobile p, Mobile v)
        {
            Player = p;
            Vendor = v;
        }
    }

}
