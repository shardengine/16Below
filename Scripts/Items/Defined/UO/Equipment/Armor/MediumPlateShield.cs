using System;

namespace Server.Items
{
    // Based off a MetalShield
    [FlipableAttribute(0x4203, 0x4209)]
    public class MediumPlateShield : BaseShield
    {
        [Constructable]
        public MediumPlateShield()
            : base(0x4203)
        {
            //Weight = 6.0;
        }

        public MediumPlateShield(Serial serial)
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
                return 11;
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