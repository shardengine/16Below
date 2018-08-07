using System;

namespace Server.Items
{
    // Based off a Spear
    [FlipableAttribute(0x904, 0x406D)]
    public class DualPointedSpear : BaseSpear
    {
        [Constructable]
        public DualPointedSpear()
            : base(0x904)
        {
            //Weight = 7.0;
        }

        public DualPointedSpear(Serial serial)
            : base(serial)
        {
        }
 
        public override int OldStrengthReq
        {
            get
            {
                return 30;
            }
        }
        public override int OldMinDamage
        {
            get
            {
                return 2;
            }
        }
        public override int OldMaxDamage
        {
            get
            {
                return 36;
            }
        }
        public override int OldSpeed
        {
            get
            {
                return 46;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 31;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 80;
            }
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
    }
}