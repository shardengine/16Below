using System;

namespace Server
{
    public delegate void SetAbilityEventHandler(SetAbilityEventArgs e);
    public delegate void RacialAbilityRequestEventHandler(RacialAbilityRequestEventArgs e);
    public delegate void DisarmRequestEventHandler(DisarmRequestEventArgs e);
    public delegate void StunRequestEventHandler(StunRequestEventArgs e);

    public static partial class EventSink
    {
        public static event SetAbilityEventHandler SetAbility;
        public static event RacialAbilityRequestEventHandler RacialAbilityRequest;
        public static event DisarmRequestEventHandler DisarmRequest;
        public static event StunRequestEventHandler StunRequest;

        public static void InvokeSetAbility(SetAbilityEventArgs e)
        {
            SetAbility?.Invoke(e);
        }

        public static void InvokeRacialAbilityRequest(RacialAbilityRequestEventArgs e)
        {
            RacialAbilityRequest?.Invoke(e);
        }

        public static void InvokeDisarmRequest(DisarmRequestEventArgs e)
        {
            DisarmRequest?.Invoke(e);
        }

        public static void InvokeStunRequest(StunRequestEventArgs e)
        {
            StunRequest?.Invoke(e);
        }

        private static void ResetAbilities()
        {
            SetAbility = null;
            RacialAbilityRequest = null;
            DisarmRequest = null;
            StunRequest = null;
        }
    }

    public class SetAbilityEventArgs : EventArgs
    {
        private readonly Mobile m_Mobile;
        private readonly int m_Index;

        public Mobile Mobile { get { return m_Mobile; } }
        public int Index { get { return m_Index; } }

        public SetAbilityEventArgs(Mobile mobile, int index)
        {
            m_Mobile = mobile;
            m_Index = index;
        }
    }

    public class RacialAbilityRequestEventArgs : EventArgs
    {
        private Mobile m_Mobile;
        private int m_AbilityID;

        public Mobile Mobile { get { return m_Mobile; } }
        public int AbilityID { get { return m_AbilityID; } }

        public RacialAbilityRequestEventArgs(Mobile m, int abilityID)
        {
            m_Mobile = m;
            m_AbilityID = abilityID;
        }
    }

    public class StunRequestEventArgs : EventArgs
    {
        private readonly Mobile m_Mobile;

        public Mobile Mobile { get { return m_Mobile; } }

        public StunRequestEventArgs(Mobile m)
        {
            m_Mobile = m;
        }
    }

    public class DisarmRequestEventArgs : EventArgs
    {
        private readonly Mobile m_Mobile;

        public Mobile Mobile { get { return m_Mobile; } }

        public DisarmRequestEventArgs(Mobile m)
        {
            m_Mobile = m;
        }
    }
}
