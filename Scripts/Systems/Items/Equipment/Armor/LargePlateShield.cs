using System;

namespace Server.Items
{
    // Based off a HeaterShield
    [FlipableAttribute(0x4204, 0x4208)]
    public class LargePlateShield : BaseShield
    {
        [Constructable]
        public LargePlateShield()
            : base(0x4204)
        {
            //Weight = 8.0;
        }

        public LargePlateShield(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits
        {
            get
            {
                return 50;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 65;
            }
        }
        public override int ArmorBase
        {
            get
            {
                return 23;
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