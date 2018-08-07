using System;
using System.Collections.Generic;

namespace Server
{
    public delegate void AggressiveActionEventHandler(AggressiveActionEventArgs e);
    public delegate void BeforeDamageEventHandler(BeforeDamageEventArgs e);

    public static partial class EventSink
    {
        public static event AggressiveActionEventHandler AggressiveAction;
        public static event BeforeDamageEventHandler BeforeDamage;

        public static void InvokeAggressiveAction(AggressiveActionEventArgs e)
        {
            AggressiveAction?.Invoke(e); //.Aggressor, e.Aggressed, e.Criminal
        }

        public static void InvokeBeforeDamage(BeforeDamageEventArgs e)
        {
            BeforeDamage?.Invoke(e);
        }

        private static void ResetCombat()
        {
            AggressiveAction = null;
            BeforeDamage = null;
        }
    }

    public class AggressiveActionEventArgs : EventArgs
    {
        private Mobile m_Aggressed;
        private Mobile m_Aggressor;
        private bool m_Criminal;

        public Mobile Aggressed { get { return m_Aggressed; } }
        public Mobile Aggressor { get { return m_Aggressor; } }
        public bool Criminal { get { return m_Criminal; } }

        private static readonly Queue<AggressiveActionEventArgs> m_Pool = new Queue<AggressiveActionEventArgs>();

        public static AggressiveActionEventArgs Create(Mobile aggressed, Mobile aggressor, bool criminal)
        {
            AggressiveActionEventArgs args;

            if (m_Pool.Count > 0)
            {
                args = m_Pool.Dequeue();

                args.m_Aggressed = aggressed;
                args.m_Aggressor = aggressor;
                args.m_Criminal = criminal;
            }
            else
            {
                args = new AggressiveActionEventArgs(aggressed, aggressor, criminal);
            }

            return args;
        }

        private AggressiveActionEventArgs(Mobile aggressed, Mobile aggressor, bool criminal)
        {
            m_Aggressed = aggressed;
            m_Aggressor = aggressor;
            m_Criminal = criminal;
        }

        public void Free()
        {
            m_Pool.Enqueue(this);
        }
    }

    public class BeforeDamageEventArgs : EventArgs
    {
        private Mobile m_Mobile;
        private Mobile m_From;
        private int m_Amount;

        public Mobile Mobile { get { return m_Mobile; } }
        public Mobile From { get { return m_From; } }
        public int Amount { get { return m_Amount; } set { m_Amount = value; } }

        public BeforeDamageEventArgs(Mobile mobile, Mobile from, int amount)
        {
            m_Mobile = mobile;
            m_From = from;
            m_Amount = amount;
        }
    }
}
