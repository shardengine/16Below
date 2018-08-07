using System;
using Server.Targeting;

namespace Server.Items
{
    public class RunedSwitch : Item
    {
        [Constructable]
        public RunedSwitch()
            : base(0x2F61)
        {
            Weight = 1.0;
        }

        public RunedSwitch(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072896;
            }
        }// runed switch
        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1075101); // Please select an item to recharge.
                from.Target = new InternalTarget(this);
            }
            else
                from.SendLocalizedMessage(1060640); // The item must be in your backpack to use it.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        private class InternalTarget : Target
        {
            private readonly RunedSwitch m_Item;
            public InternalTarget(RunedSwitch item)
                : base(0, false, TargetFlags.None)
            {
                m_Item = item;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (m_Item == null || m_Item.Deleted)
                    return;
                
                from.SendLocalizedMessage(1046439); // That is not a valid target.
            }
        }
    }
}