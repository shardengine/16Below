using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Mobiles;

namespace Server.Items
{
    public enum GemType
    {
        None,
        StarSapphire,
        Emerald,
        Sapphire,
        Ruby,
        Citrine,
        Amethyst,
        Tourmaline,
        Amber,
        Diamond
    }

    public abstract class BaseJewel : Item, ICraftable, IWearableDurability
    {
        private int m_MaxHitPoints;
        private int m_HitPoints;

        private CraftResource m_Resource;
        private GemType m_GemType;

        private Mobile m_BlessedBy;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile BlessedBy
        {
            get
            {
                return m_BlessedBy;
            }
            set
            {
                m_BlessedBy = value;
                InvalidateProperties();
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (BlessedFor == from && BlessedBy == from && RootParent == from)
            {
                list.Add(new UnBlessEntry(from, this));
            }
        }

        private class UnBlessEntry : ContextMenuEntry
        {
            private readonly Mobile m_From;
            private readonly BaseJewel m_Item;

            public UnBlessEntry(Mobile from, BaseJewel item)
                : base(6208, -1)
            {
                m_From = from;
                m_Item = item;
            }

            public override void OnClick()
            {
                m_Item.BlessedFor = null;
                m_Item.BlessedBy = null;

                Container pack = m_From.Backpack;

                if (pack != null)
                {
                    pack.DropItem(new PersonalBlessDeed(m_From));
                    m_From.SendLocalizedMessage(1062200); // A personal bless deed has been placed in your backpack.
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHitPoints
        {
            get
            {
                return m_MaxHitPoints;
            }
            set
            {
                m_MaxHitPoints = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitPoints
        {
            get 
            {
                return m_HitPoints;
            }
            set 
            {
                if (value != m_HitPoints && MaxHitPoints > 0)
                {
                    m_HitPoints = value;

                    if (m_HitPoints < 0)
                        Delete();
                    else if (m_HitPoints > MaxHitPoints)
                        m_HitPoints = MaxHitPoints;

                    InvalidateProperties();
                }
            }
        }

       [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get
            {
                return m_Resource;
            }
            set
            {
                m_Resource = value;
                Hue = CraftResources.GetHue(m_Resource);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public GemType GemType
        {
            get
            {
                return m_GemType;
            }
            set
            {
                m_GemType = value;
                InvalidateProperties();
            }
        }

        public virtual int BaseGemTypeNumber
        {
            get
            {
                return 0;
            }
        }

        public virtual int InitMinHits
        {
            get
            {
                return 0;
            }
        }
        public virtual int InitMaxHits
        {
            get
            {
                return 0;
            }
        }

        public override int LabelNumber
        {
            get
            {
                if (m_GemType == GemType.None)
                    return base.LabelNumber;

                return BaseGemTypeNumber + (int)m_GemType - 1;
            }
        }

        public virtual int ArtifactRarity
        {
            get
            {
                return 0;
            }
        }

        private Mobile m_Crafter;
        private ArmorQuality m_Quality;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get
            {
                return m_Crafter;
            }
            set
            {
                m_Crafter = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ArmorQuality Quality
        {
            get
            {
                return m_Quality;
            }
            set
            {
                m_Quality = value;
                InvalidateProperties();
            }
        }

        public BaseJewel(int itemID, Layer layer)
            : base(itemID)
        {
            m_Resource = CraftResource.Iron;
            m_GemType = GemType.None;

            Layer = layer;

            m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax(InitMinHits, InitMaxHits);

        }

        public override void OnAdded(object parent)
        {
            if (parent is Mobile)
            {
                if (Engines.XmlSpawner2.XmlAttach.CheckCanEquip(this, (Mobile)parent))
                    Engines.XmlSpawner2.XmlAttach.CheckOnEquip(this, (Mobile)parent);
                else
                    ((Mobile)parent).AddToBackpack(this);
            }
        }

        public override void OnRemoved(object parent)
        {
           Engines.XmlSpawner2.XmlAttach.CheckOnRemoved(this, parent);
        }

        public virtual int OnHit(BaseWeapon weap, int damageTaken)
        {
            if (25 > Utility.Random(100)) // 25% chance to lower durability
            {
                int wear = Utility.Random(2);

                if (wear > 0)
                {
                    if (m_HitPoints >= wear)
                    {
                        HitPoints -= wear;
                        wear = 0;
                    }
                    else
                    {
                        wear -= HitPoints;
                        HitPoints = 0;
                    }

                    if (wear > 0)
                    {
                        if (m_MaxHitPoints > wear)
                        {
                            MaxHitPoints -= wear;

                            if (Parent is Mobile)
                                ((Mobile)Parent).LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.
                        }
                        else
                        {
                            this.Delete();
                        }
                    }
                }
            }

            return damageTaken;
        }

        public BaseJewel(Serial serial)
            : base(serial)
        {
        }

        private string GetNameString()
        {
            string name = Name;

            if (name == null)
                name = String.Format("#{0}", LabelNumber);

            return name;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);


            int prop;

            if ((prop = ArtifactRarity) > 0)
                list.Add(1061078, prop.ToString()); // artifact rarity ~1_val~

            AddResistanceProperties(list);

            Engines.XmlSpawner2.XmlAttach.AddAttachmentProperties(this, list);

            if (m_HitPoints >= 0 && m_MaxHitPoints > 0)
                list.Add(1060639, "{0}\t{1}", m_HitPoints, m_MaxHitPoints); // durability ~1_val~ / ~2_val~
        }

        public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			if (m_Crafter != null)
			{
				LabelTo(from, 1050043, m_Crafter.TitleName); // crafted by ~1_NAME~
			}
		}
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(7); // version

            writer.Write((Mobile)m_BlessedBy);

            writer.Write(m_Crafter);
            writer.Write((int)m_Quality);

            // Version 3
            writer.WriteEncodedInt((int)m_MaxHitPoints);
            writer.WriteEncodedInt((int)m_HitPoints);

            writer.WriteEncodedInt((int)m_Resource);
            writer.WriteEncodedInt((int)m_GemType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 7:
                    {
                        goto case 6;
                    }
                case 6:
                    {
                        goto case 5;
                    }
                case 5:
                    {
                        m_BlessedBy = reader.ReadMobile();

                        m_Crafter = reader.ReadMobile();
                        m_Quality = (ArmorQuality)reader.ReadInt();
                        goto case 3;
                    }
                case 3:
                    {
                        m_MaxHitPoints = reader.ReadEncodedInt();
                        m_HitPoints = reader.ReadEncodedInt();

                        goto case 2;
                    }
                case 2:
                    {
                        m_Resource = (CraftResource)reader.ReadEncodedInt();
                        m_GemType = (GemType)reader.ReadEncodedInt();

                        goto case 1;
                    }
                case 1:
                    {
                        if (Parent is Mobile)
                            ((Mobile)Parent).CheckStatTimers();

                        break;
                    }
                case 0:
                    {
                        break;
                    }
            }

            if (version < 2)
            {
                m_Resource = CraftResource.Iron;
                m_GemType = GemType.None;
            }
        }

        #region ICraftable Members

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            Type resourceType = typeRes;

            if (resourceType == null)
                resourceType = craftItem.Resources.GetAt(0).ItemType;

            if (!craftItem.ForceNonExceptional)
                Resource = CraftResources.GetFromType(resourceType);

            CraftContext context = craftSystem.GetContext(from);

            if (context != null && context.DoNotColor)
                Hue = 0;

            if (1 < craftItem.Resources.Count)
            {
                resourceType = craftItem.Resources.GetAt(1).ItemType;

                if (resourceType == typeof(StarSapphire))
                    GemType = GemType.StarSapphire;
                else if (resourceType == typeof(Emerald))
                    GemType = GemType.Emerald;
                else if (resourceType == typeof(Sapphire))
                    GemType = GemType.Sapphire;
                else if (resourceType == typeof(Ruby))
                    GemType = GemType.Ruby;
                else if (resourceType == typeof(Citrine))
                    GemType = GemType.Citrine;
                else if (resourceType == typeof(Amethyst))
                    GemType = GemType.Amethyst;
                else if (resourceType == typeof(Tourmaline))
                    GemType = GemType.Tourmaline;
                else if (resourceType == typeof(Amber))
                    GemType = GemType.Amber;
                else if (resourceType == typeof(Diamond))
                    GemType = GemType.Diamond;
            }
            return 1;
        }

        #endregion
    }
}
