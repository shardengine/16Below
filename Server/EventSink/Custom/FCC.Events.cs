using System;
using Server.Network;

namespace Server
{
    public delegate void NpcSpeechCommandEventHandler(NpcSpeechCommandEventArgs e);
    public delegate void ContextMenuDisplayEventHandler(ContextMenuDisplayEventArgs e);

    public static partial class EventSink
    {
        public static event NpcSpeechCommandEventHandler NpcSpeechCommand;
        public static event ContextMenuDisplayEventHandler ContextMenuDisplay;

        public static void InvokeNpcSpeechCommand(NpcSpeechCommandEventArgs e)
        {
            NpcSpeechCommand?.Invoke(e);
        }

        public static void InvokeContextMenuDisplay(ContextMenuDisplayEventArgs e)
        {
            ContextMenuDisplay?.Invoke(e);
        }

        private static void ResetFCC()
        {
            NpcSpeechCommand = null;
            ContextMenuDisplay = null;
        }
    }

    public class NpcSpeechCommandEventArgs : EventArgs
    {
        private NetState state;

        // public Mobile Player { get; private set; }
        // public Mobile Npc { get; private set; }

        //    private Item m_tool;
        //    private int m_resource_type;

        public NetState NetState
        {
            get { return state; }
        }

        /*
        public Item Tool
        {
            get { return m_tool; }
        }
        public int ResourceType
        {
            get { return m_resource_type; }
        }
        */

        public NpcSpeechCommandEventArgs(NetState state/*, Item tool, int type*/)
        {
            this.state = state;
        //    this.m_tool = tool;
        //    this.m_resource_type = type;
        }
    }

    public class ContextMenuDisplayEventArgs : EventArgs
    {
        private NetState state;

        // public Mobile Player { get; private set; }
        // public Mobile Npc { get; private set; }

        //    private Item m_tool;
        //    private int m_resource_type;

        public NetState NetState
        {
            get { return state; }
        }

        /*
        public Item Tool
        {
            get { return m_tool; }
        }
        public int ResourceType
        {
            get { return m_resource_type; }
        }
        */

        public ContextMenuDisplayEventArgs(NetState state/*, Item tool, int type*/)
        {
            this.state = state;
            //    this.m_tool = tool;
            //    this.m_resource_type = type;
        }
    }

}
