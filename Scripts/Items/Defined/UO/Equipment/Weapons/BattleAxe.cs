using System;

namespace Server.Items
{
    [FlipableAttribute(0xF47, 0xF48)]
    public class BattleAxe : BaseAxe
    {
        [Constructable]
        public BattleAxe()
            : base(0xF47)
        {
            this.Weight = 4.0;
            this.Layer = Layer.TwoHanded;
        }

        public BattleAxe(Serial serial)
            : base(serial)
        {
        }

        public override int OldStrengthReq
        {
            get
            {
                return 40;
            }
        }
        public override int OldMinDamage
        {
            get
            {
                return 6;
            }
        }
        public override int OldMaxDamage
        {
            get
            {
                return 38;
            }
        }
        public override int OldSpeed
        {
            get
            {
                return 30;
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
                return 70;
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