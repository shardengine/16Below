using System;

namespace Server.Items
{
    public class BronzeShield : BaseShield
    {
        [Constructable]
        public BronzeShield()
            : base(0x1B72)
        {
            this.Weight = 6.0;
        }

        public BronzeShield(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits
        {
            get
            {
                return 25;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 30;
            }
        }
        public override int ArmorBase
        {
            get
            {
                return 10;
            }
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version
        }
    }
}