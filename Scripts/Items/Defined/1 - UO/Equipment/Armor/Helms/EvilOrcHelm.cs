using System;
using Server.Misc;

namespace Server.Items
{
    public class EvilOrcHelm : OrcHelm
    {
        [Constructable]
        public EvilOrcHelm()
            : base()
        {
            this.Hue = 0x96E;
        }

        public EvilOrcHelm(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1062021;
            }
        }// an evil orc helm
        public override bool OnEquip(Mobile from)
        {
		
            Titles.AwardKarma(from, -22, true);

            return base.OnEquip(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}