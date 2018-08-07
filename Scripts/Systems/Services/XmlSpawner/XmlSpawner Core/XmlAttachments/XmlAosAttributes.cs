using System;

/*
* This script adds four attachments that will allow dynamic enhancment of Aos attributes
* XmlAosAttributes 
* XmlAosWeaponAttributes 
* XmlAosArmorAttributes 
* XmlAosElementAttributes 
*/
namespace Server.Engines.XmlSpawner2
{
    public class XmlAosAttributes : XmlBaseAttributes
    {
        // a serial constructor is REQUIRED
        public XmlAosAttributes(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlAosAttributes()
            : base()
        {
        }

        [Attachable]
        public XmlAosAttributes(double expiresin)
            : base(expiresin)
        {
        }

        // These are the various ways in which the message attachment can be constructed.
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword
        // Other overloads could be defined to handle other types of arguments
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
        }
    }

    public class XmlAosWeaponAttributes : XmlBaseAttributes
    {
        // a serial constructor is REQUIRED
        public XmlAosWeaponAttributes(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlAosWeaponAttributes()
            : base()
        {
        }

        [Attachable]
        public XmlAosWeaponAttributes(double expiresin)
            : base(expiresin)
        {
        }

        // These are the various ways in which the message attachment can be constructed.
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword
        // Other overloads could be defined to handle other types of arguments
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
        }
    }

    public class XmlAosArmorAttributes : XmlBaseAttributes
    {
        // a serial constructor is REQUIRED
        public XmlAosArmorAttributes(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlAosArmorAttributes()
            : base()
        {
        }

        [Attachable]
        public XmlAosArmorAttributes(double expiresin)
            : base(expiresin)
        {
        }

        // These are the various ways in which the message attachment can be constructed.
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword
        // Other overloads could be defined to handle other types of arguments
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
        }
    }

    public class XmlAosElementAttributes : XmlBaseAttributes
    {
        // a serial constructor is REQUIRED
        public XmlAosElementAttributes(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlAosElementAttributes()
            : base()
        {
        }

        [Attachable]
        public XmlAosElementAttributes(double expiresin)
            : base(expiresin)
        {
        }

        // These are the various ways in which the message attachment can be constructed.
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword
        // Other overloads could be defined to handle other types of arguments
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
        }
    }

    public class XmlBaseAttributes : XmlAttachment
    {
        private static readonly int[] m_Empty = new int[0];
        private uint m_Names;
        private int[] m_Values = new int[0];
        // a serial constructor is REQUIRED
        public XmlBaseAttributes(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlBaseAttributes()
        {
        }

        [Attachable]
        public XmlBaseAttributes(double expiresin)
        {
            this.Expiration = TimeSpan.FromMinutes(expiresin);
        }

        public bool IsEmpty
        {
            get
            {
                return (this.m_Names == 0);
            }
        }
        // These are the various ways in which the message attachment can be constructed.
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword
        // Other overloads could be defined to handle other types of arguments
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
            writer.Write((uint)this.m_Names);
            writer.WriteEncodedInt((int)this.m_Values.Length);

            for (int i = 0; i < this.m_Values.Length; ++i)
                writer.WriteEncodedInt((int)this.m_Values[i]);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
            this.m_Names = reader.ReadUInt();
            this.m_Values = new int[reader.ReadEncodedInt()];

            for (int i = 0; i < this.m_Values.Length; ++i)
                this.m_Values[i] = reader.ReadEncodedInt();
        }

        public override void OnDelete()
        {
            base.OnDelete();

            // remove the mod
            if (this.AttachedTo is Item)
            {
                ((Item)this.AttachedTo).InvalidateProperties();
            }
        }

        public override void OnAttach()
        {
            base.OnAttach();

            if (this.AttachedTo is Item)
            {
                ((Item)this.AttachedTo).InvalidateProperties();
            }
        }

        public int GetValue(int bitmask)
        {
            uint mask = (uint)bitmask;

            if ((this.m_Names & mask) == 0)
                return 0;

            int index = this.GetIndex(mask);

            if (index >= 0 && index < this.m_Values.Length)
                return this.m_Values[index];

            return 0;
        }

        public void SetValue(int bitmask, int value)
        {
            uint mask = (uint)bitmask;

            if (value != 0)
            {
                if ((this.m_Names & mask) != 0)
                {
                    int index = this.GetIndex(mask);

                    if (index >= 0 && index < this.m_Values.Length)
                        this.m_Values[index] = value;
                }
                else
                {
                    int index = this.GetIndex(mask);

                    if (index >= 0 && index <= this.m_Values.Length)
                    {
                        int[] old = this.m_Values;
                        this.m_Values = new int[old.Length + 1];

                        for (int i = 0; i < index; ++i)
                            this.m_Values[i] = old[i];

                        this.m_Values[index] = value;

                        for (int i = index; i < old.Length; ++i)
                            this.m_Values[i + 1] = old[i];

                        this.m_Names |= mask;
                    }
                }
            }
            else if ((this.m_Names & mask) != 0)
            {
                int index = this.GetIndex(mask);

                if (index >= 0 && index < this.m_Values.Length)
                {
                    this.m_Names &= ~mask;

                    if (this.m_Values.Length == 1)
                    {
                        this.m_Values = m_Empty;
                    }
                    else
                    {
                        int[] old = this.m_Values;
                        this.m_Values = new int[old.Length - 1];

                        for (int i = 0; i < index; ++i)
                            this.m_Values[i] = old[i];

                        for (int i = index + 1; i < old.Length; ++i)
                            this.m_Values[i - 1] = old[i];
                    }
                }
            }

            if (this.AttachedTo is Item)
            {
                ((Item)this.AttachedTo).InvalidateProperties();
            }
        }

        private int GetIndex(uint mask)
        {
            int index = 0;
            uint ourNames = this.m_Names;
            uint currentBit = 1;

            while (currentBit != mask)
            {
                if ((ourNames & currentBit) != 0)
                    ++index;

                if (currentBit == 0x80000000)
                    return -1;

                currentBit <<= 1;
            }

            return index;
        }
    }
}