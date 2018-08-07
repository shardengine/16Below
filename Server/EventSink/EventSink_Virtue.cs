using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Server.Accounting;
using Server.Commands;
using Server.Guilds;
using Server.Network;

namespace Server
{
    public delegate void VirtueGumpRequestEventHandler(VirtueGumpRequestEventArgs e);
    public delegate void VirtueItemRequestEventHandler(VirtueItemRequestEventArgs e);
    public delegate void VirtueMacroRequestEventHandler(VirtueMacroRequestEventArgs e);

    public static partial class EventSink
    {
        public static event VirtueGumpRequestEventHandler VirtueGumpRequest;
        public static event VirtueItemRequestEventHandler VirtueItemRequest;
        public static event VirtueMacroRequestEventHandler VirtueMacroRequest;

        public static void InvokeVirtueItemRequest(VirtueItemRequestEventArgs e)
        {
            VirtueItemRequest?.Invoke(e);
        }

        public static void InvokeVirtueGumpRequest(VirtueGumpRequestEventArgs e)
        {
            VirtueGumpRequest?.Invoke(e);
        }

        public static void InvokeVirtueMacroRequest(VirtueMacroRequestEventArgs e)
        {
            VirtueMacroRequest?.Invoke(e);
        }

        private static void ResetVirtue()
        {
            VirtueGumpRequest = null;
            VirtueItemRequest = null;
            VirtueMacroRequest = null;
        }
    }

    public class VirtueItemRequestEventArgs : EventArgs
    {
        private readonly Mobile m_Beholder;
        private readonly Mobile m_Beheld;
        private readonly int m_GumpID;

        public Mobile Beholder { get { return m_Beholder; } }
        public Mobile Beheld { get { return m_Beheld; } }
        public int GumpID { get { return m_GumpID; } }

        public VirtueItemRequestEventArgs(Mobile beholder, Mobile beheld, int gumpID)
        {
            m_Beholder = beholder;
            m_Beheld = beheld;
            m_GumpID = gumpID;
        }
    }

    public class VirtueGumpRequestEventArgs : EventArgs
    {
        private readonly Mobile m_Beholder;
        private readonly Mobile m_Beheld;

        public Mobile Beholder { get { return m_Beholder; } }
        public Mobile Beheld { get { return m_Beheld; } }

        public VirtueGumpRequestEventArgs(Mobile beholder, Mobile beheld)
        {
            m_Beholder = beholder;
            m_Beheld = beheld;
        }
    }

    public class VirtueMacroRequestEventArgs : EventArgs
    {
        private readonly Mobile m_Mobile;
        private readonly int m_VirtueID;

        public Mobile Mobile { get { return m_Mobile; } }
        public int VirtueID { get { return m_VirtueID; } }

        public VirtueMacroRequestEventArgs(Mobile mobile, int virtueID)
        {
            m_Mobile = mobile;
            m_VirtueID = virtueID;
        }
    }

}
