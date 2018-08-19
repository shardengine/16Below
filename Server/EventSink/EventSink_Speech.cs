using System;
using Server.Network;

namespace Server
{
    public delegate void SpeechEventHandler(SpeechEventArgs e);

    public static partial class EventSink
    {
        public static event SpeechEventHandler Speech;

        public static void InvokeSpeech(SpeechEventArgs e)
        {
            Speech?.Invoke(e);
        }

        private static void ResetSpeech() 
        {
            Speech = null;
        }
    }

    public class SpeechEventArgs : EventArgs
    {
        private readonly Mobile m_Mobile;
        private readonly MessageType m_Type;
        private readonly int m_Hue;
        private readonly int[] m_Keywords;

        public Mobile Mobile { get { return m_Mobile; } }
        public string Speech { get; set; }
        public MessageType Type { get { return m_Type; } }
        public int Hue { get { return m_Hue; } }
        public int[] Keywords { get { return m_Keywords; } }
        public bool Handled { get; set; }
        public bool Blocked { get; set; }

        public bool HasKeyword(int keyword)
        {
            for (int i = 0; i < m_Keywords.Length; ++i)
            {
                if (m_Keywords[i] == keyword)
                {
                    return true;
                }
            }

            return false;
        }

        public SpeechEventArgs(Mobile mobile, string speech, MessageType type, int hue, int[] keywords)
        {
            m_Mobile = mobile;
            Speech = speech;
            m_Type = type;
            m_Hue = hue;
            m_Keywords = keywords;
        }
    }
}
