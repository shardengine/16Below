using System;
using System.Collections.Generic;
using Server.Network;

namespace Server
{
    public delegate void FastWalkEventHandler(FastWalkEventArgs e);
    public delegate void MovementEventHandler(MovementEventArgs e);

    public static partial class EventSink
    {
        public static event FastWalkEventHandler FastWalk;
        public static event MovementEventHandler Movement;

        public static void InvokeFastWalk(FastWalkEventArgs e)
        {
            FastWalk?.Invoke(e);
        }

        public static void InvokeMovement(MovementEventArgs e)
        {
            Movement?.Invoke(e);
        }

        private static void ResetMovement()
        {
            FastWalk = null;
            Movement = null;
        }
    }

    public class FastWalkEventArgs : EventArgs
    {
        private readonly NetState m_State;

        public FastWalkEventArgs(NetState state)
        {
            m_State = state;
            Blocked = false;
        }

        public NetState NetState { get { return m_State; } }
        public bool Blocked { get; set; }
    }

    public class MovementEventArgs : EventArgs
    {
        private Mobile m_Mobile;
        private Direction m_Direction;
        private bool m_Blocked;

        public Mobile Mobile { get { return m_Mobile; } }
        public Direction Direction { get { return m_Direction; } }
        public bool Blocked { get { return m_Blocked; } set { m_Blocked = value; } }

        private static readonly Queue<MovementEventArgs> m_Pool = new Queue<MovementEventArgs>();

        public static MovementEventArgs Create(Mobile mobile, Direction dir)
        {
            MovementEventArgs args;

            if (m_Pool.Count > 0)
            {
                args = m_Pool.Dequeue();

                args.m_Mobile = mobile;
                args.m_Direction = dir;
                args.m_Blocked = false;
            }
            else
            {
                args = new MovementEventArgs(mobile, dir);
            }

            return args;
        }

        public MovementEventArgs(Mobile mobile, Direction dir)
        {
            m_Mobile = mobile;
            m_Direction = dir;
        }

        public void Free()
        {
            m_Pool.Enqueue(this);
        }
    }
}
