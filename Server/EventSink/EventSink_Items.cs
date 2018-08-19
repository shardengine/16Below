using System;

namespace Server // move to mobile or move item from mobile here??
{
    public delegate void OnItemUseEventHandler(OnItemUseEventArgs e);
    public delegate void OnConsumeEventHandler(OnConsumeEventArgs e);

    public static partial class EventSink
    {
        public static event OnItemUseEventHandler OnItemUse;
        public static event OnConsumeEventHandler OnConsume;

        public static void InvokeOnItemUse(OnItemUseEventArgs e)
        {
            OnItemUse?.Invoke(e);
        }

        public static void InvokeOnConsume(OnConsumeEventArgs e)
        {
            OnConsume?.Invoke(e);
        }

        private static void ResetItems()
        {
            OnItemUse = null;
            OnConsume = null;
        }
    }

    public class OnItemUseEventArgs : EventArgs
    {
        private readonly Mobile m_From;
        private readonly Item m_Item;

        public OnItemUseEventArgs(Mobile from, Item item)
        {
            m_From = from;
            m_Item = item;
        }

        public Mobile From { get { return m_From; } }
        public Item Item { get { return m_Item; } }
    }

    public class OnConsumeEventArgs : EventArgs
    {
        private readonly Mobile m_Consumer;
        private readonly Item m_Consumed;
        private readonly int m_Quantity;

        public OnConsumeEventArgs(Mobile consumer, Item consumed)
            : this(consumer, consumed, 1)
        { }

        public OnConsumeEventArgs(Mobile consumer, Item consumed, int quantity)
        {
            m_Consumer = consumer;
            m_Consumed = consumed;
            m_Quantity = quantity;
        }

        public Mobile Consumer { get { return m_Consumer; } }
        public Item Consumed { get { return m_Consumed; } }
        public int Quantity { get { return m_Quantity; } }
    }
}
