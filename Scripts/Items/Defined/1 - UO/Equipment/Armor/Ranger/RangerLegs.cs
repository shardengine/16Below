using System;

namespace Server.Items
{
    [FlipableAttribute(0x13da, 0x13e1)]
    public class RangerLegs : BaseArmor
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RangerLegs()
            : base(0x13DA)
        {
            this.Weight = 3.0;
            this.Hue = 0x59C;
        }

        public RangerLegs(Serial serial)
            : base(serial)
        {
        }


        public override int InitMinHits
        {
            get
            {
                return 35;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 45;
            }
        }

        public override int OldStrReq
        {
            get
            {
                return 35;
            }
        }
        public override int ArmorBase
        {
            get
            {
                return 16;
            }
        }
        public override ArmorMaterialType MaterialType
        {
            get
            {
                return ArmorMaterialType.Studded;
            }
        }
        public override CraftResource DefaultResource
        {
            get
            {
                return CraftResource.RegularLeather;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1041496;
            }
        }// studded leggings, ranger armor
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (this.Weight == 3.0)
                this.Weight = 5.0;
        }
    }
}