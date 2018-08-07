using System;

namespace Server.Items
{
    public class MagicWand : BaseBashing
    {
        [Constructable]
        public MagicWand()
            : base(0xDF2)
        {
            this.Weight = 1.0;
        }

        public MagicWand(Serial serial)
            : base(serial)
        {
        }

        public override int OldStrengthReq
        {
            get
            {
                return 0;
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
                return 6;
            }
        }
        public override int OldSpeed
        {
            get
            {
                return 35;
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
                return 110;
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