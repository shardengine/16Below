using System;

namespace Server.Items
{
    #region Reward Clothing
    public class LibraryFriendSkirt : Kilt
    {
        public override int LabelNumber
        {
            get
            {
                return 1073352;
            }
        }// Friends of the Library Kilt

        [Constructable]
        public LibraryFriendSkirt()
            : this(0)
        {
        }

        [Constructable]
        public LibraryFriendSkirt(int hue)
            : base(hue)
        {
        }

        public LibraryFriendSkirt(Serial serial)
            : base(serial)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
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

    public class LibraryFriendPants : LongPants
    {
        public override int LabelNumber
        {
            get
            {
                return 1073349;
            }
        }// Friends of the Library Pants

        [Constructable]
        public LibraryFriendPants()
            : this(0)
        {
        }

        [Constructable]
        public LibraryFriendPants(int hue)
            : base(hue)
        {
        }

        public LibraryFriendPants(Serial serial)
            : base(serial)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
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

    public class MalabellesDress : Skirt
    {
        public override int LabelNumber
        {
            get
            {
                return 1073251;
            }
        }// Malabelle's Dress - Museum of Vesper Replica

        [Constructable]
        public MalabellesDress()
            : this(0)
        {
        }

        [Constructable]
        public MalabellesDress(int hue)
            : base(hue)
        {
        }

        public MalabellesDress(Serial serial)
            : base(serial)
        {
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
    #endregion

    public abstract class BasePants : BaseClothing
    {
        public BasePants(int itemID)
            : this(itemID, 0)
        {
        }

        public BasePants(int itemID, int hue)
            : base(itemID, Layer.Pants, hue)
        {
        }

        public BasePants(Serial serial)
            : base(serial)
        {
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

    [FlipableAttribute(0x152e, 0x152f)]
    public class ShortPants : BasePants
    {
        [Constructable]
        public ShortPants()
            : this(0)
        {
        }

        [Constructable]
        public ShortPants(int hue)
            : base(0x152E, hue)
        {
            this.Weight = 2.0;
        }

        public ShortPants(Serial serial)
            : base(serial)
        {
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

    [FlipableAttribute(0x1539, 0x153a)]
    public class LongPants : BasePants
    {
        [Constructable]
        public LongPants()
            : this(0)
        {
        }

        [Constructable]
        public LongPants(int hue)
            : base(0x1539, hue)
        {
            this.Weight = 2.0;
        }

        public LongPants(Serial serial)
            : base(serial)
        {
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