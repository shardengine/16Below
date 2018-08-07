using System;

namespace Server
{
    public delegate void OnKilledByEventHandler(OnKilledByEventArgs e);
    public delegate void PlayerDeathEventHandler(PlayerDeathEventArgs e);

    public static partial class EventSink
    {
        public static event OnKilledByEventHandler OnKilledBy;
        public static event PlayerDeathEventHandler PlayerDeath;

        public static void InvokeOnKilledBy(OnKilledByEventArgs e)
        {
            OnKilledBy?.Invoke(e);
        }

        public static void InvokePlayerDeath(PlayerDeathEventArgs e)
        {
            PlayerDeath?.Invoke(e);
        }

        private static void ResetDeath()
        {
            OnKilledBy = null;
            PlayerDeath = null;
        }
    }

    public class OnKilledByEventArgs : EventArgs
    {
        private readonly Mobile m_Killed;
        private readonly Mobile m_KilledBy;

        public OnKilledByEventArgs(Mobile killed, Mobile killedBy)
        {
            m_Killed = killed;
            m_KilledBy = killedBy;
        }

        public Mobile Killed { get { return m_Killed; } }
        public Mobile KilledBy { get { return m_KilledBy; } }
    }

    public class PlayerDeathEventArgs : EventArgs
    {
        private readonly Mobile m_Mobile;

        public Mobile Mobile { get { return m_Mobile; } }

        public PlayerDeathEventArgs(Mobile mobile)
        {
            m_Mobile = mobile;
        }
    }
}
