using System;

namespace Server
{
    public delegate void BandageTargetRequestEventHandler(BandageTargetRequestEventArgs e);
    public delegate void ResourceHarvestAttemptEventHandler(ResourceHarvestAttemptEventArgs e);
    public delegate void ResourceHarvestSuccessEventHandler(ResourceHarvestSuccessEventArgs e);
    public delegate void CraftCreateItemAttemptEventHandler(CraftCreateItemAttemptEventArgs e); // Fraz Custom.. maybe move later

    public static partial class EventSink
    {
        public static event BandageTargetRequestEventHandler BandageTargetRequest;
        public static event ResourceHarvestAttemptEventHandler ResourceHarvestAttempt;
        public static event ResourceHarvestSuccessEventHandler ResourceHarvestSuccess;
        public static event CraftCreateItemAttemptEventHandler CraftCreateItemAttempt; // Fraz Custom.. maybe move later

        public static void InvokeBandageTargetRequest(BandageTargetRequestEventArgs e)
        {
            BandageTargetRequest?.Invoke(e);
        }

        public static void InvokeResourceHarvestAttempt(ResourceHarvestAttemptEventArgs e)
        {
            ResourceHarvestAttempt?.Invoke(e);
        }

        public static void InvokeResourceHarvestSuccess(ResourceHarvestSuccessEventArgs e)
        {
            ResourceHarvestSuccess?.Invoke(e);
        }

        public static void InvokeCraftCreateItemAttempt(CraftCreateItemAttemptEventArgs e) // Fraz Custom.. maybe move later
        {
            CraftCreateItemAttempt?.Invoke(e);
        }

        private static void ResetSkills()
        {
            BandageTargetRequest = null;
            ResourceHarvestAttempt = null;
            ResourceHarvestSuccess = null;
            CraftCreateItemAttempt = null; // Fraz Custom.. maybe move later
        }
    }

    public class BandageTargetRequestEventArgs : EventArgs
    {
        private readonly Mobile m_Mobile;
        private readonly Item m_Bandage;
        private readonly Mobile m_Target;

        public Mobile Mobile { get { return m_Mobile; } }
        public Item Bandage { get { return m_Bandage; } }
        public Mobile Target { get { return m_Target; } }

        public BandageTargetRequestEventArgs(Mobile m, Item bandage, Mobile target)
        {
            m_Mobile = m;
            m_Bandage = bandage;
            m_Target = target;
        }
    }

    public class ResourceHarvestAttemptEventArgs : EventArgs
    {
        public Mobile Harvester { get; private set; }
        public Item Tool { get; private set; }
        public object HarvestSystem { get; private set; }

        public ResourceHarvestAttemptEventArgs(Mobile m, Item i, object o)
        {
            Harvester = m;
            Tool = i;
            HarvestSystem = o;
        }
    }

    public class ResourceHarvestSuccessEventArgs : EventArgs
    {
        public Mobile Harvester { get; private set; }
        public Item Tool { get; private set; }
        public Item Resource { get; private set; }
        public object HarvestSystem { get; private set; }

        public ResourceHarvestSuccessEventArgs(Mobile m, Item i, Item r, object o)
        {
            Harvester = m;
            Tool = i;
            Resource = r;
            HarvestSystem = o;
        }
    }

    public class CraftCreateItemAttemptEventArgs : EventArgs // Fraz Custom.. maybe move later
    {
        public Mobile Crafter { get; private set; }
        public Type type { get; private set; }
        public object tool { get; private set; }
        public object craftSystem { get; private set; }
        public object craftItem { get; private set; }

        public CraftCreateItemAttemptEventArgs(Mobile m, Type t, object i, object o, object c)
        {
            Crafter = m;
            type = t;
            tool = i;
            craftSystem = o;
            craftItem = c;
        }
    }

}
