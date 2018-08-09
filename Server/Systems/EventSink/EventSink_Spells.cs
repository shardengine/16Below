using System;

namespace Server
{
    public delegate void CastSpellRequestEventHandler(CastSpellRequestEventArgs e);
    public delegate void OpenSpellbookRequestEventHandler(OpenSpellbookRequestEventArgs e);

    public static partial class EventSink
    {
        public static event CastSpellRequestEventHandler CastSpellRequest;
        public static event OpenSpellbookRequestEventHandler OpenSpellbookRequest;

        public static void InvokeCastSpellRequest(CastSpellRequestEventArgs e)
        {
            CastSpellRequest?.Invoke(e);
        }

        public static void InvokeOpenSpellbookRequest(OpenSpellbookRequestEventArgs e)
        {
            OpenSpellbookRequest?.Invoke(e);
        }
        
        private static void ResetSpells()
        {
            CastSpellRequest = null;
            OpenSpellbookRequest = null;
        }
    }

    public class CastSpellRequestEventArgs : EventArgs
    {
        private readonly Mobile m_Mobile;
        private readonly Item m_Spellbook;
        private readonly int m_SpellID;

        public Mobile Mobile { get { return m_Mobile; } }
        public Item Spellbook { get { return m_Spellbook; } }
        public int SpellID { get { return m_SpellID; } }

        public CastSpellRequestEventArgs(Mobile m, int spellID, Item book)
        {
            m_Mobile = m;
            m_Spellbook = book;
            m_SpellID = spellID;
        }
    }

    public class OpenSpellbookRequestEventArgs : EventArgs
    {
        private readonly Mobile m_Mobile;
        private readonly int m_Type;

        public Mobile Mobile { get { return m_Mobile; } }
        public int Type { get { return m_Type; } }

        public OpenSpellbookRequestEventArgs(Mobile m, int type)
        {
            m_Mobile = m;
            m_Type = type;
        }
    }
}
