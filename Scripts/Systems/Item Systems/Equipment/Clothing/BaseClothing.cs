using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Factions;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public enum ClothingQuality
    {
        Low,
        Regular,
        Exceptional
    }

    public interface IArcaneEquip
    {
        bool IsArcane { get; }
        int CurArcaneCharges { get; set; }
        int MaxArcaneCharges { get; set; }
    }

    public abstract class BaseClothing : Item, IDyable, IScissorable, IFactionItem, ICraftable, IWearableDurability
    {
        #region Factions
        private FactionItem m_FactionState;

        public FactionItem FactionItemState
        {
            get
            {
                return this.m_FactionState;
            }
            set
            {
                this.m_FactionState = value;

                if (this.m_FactionState == null)
                    this.Hue = 0;

                this.LootType = (this.m_FactionState == null ? LootType.Regular : LootType.Blessed);
            }
        }
        #endregion

        private int m_MaxHitPoints;
        private int m_HitPoints;
        private Mobile m_Crafter;
        private ClothingQuality m_Quality;
        private bool m_PlayerConstructed;
        protected CraftResource m_Resource;
        private int m_StrReq = -1;

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHitPoints
        {
            get
            {
                return this.m_MaxHitPoints;
            }
            set
            {
                this.m_MaxHitPoints = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitPoints
        {
            get 
            {
                return this.m_HitPoints;
            }
            set 
            {
                if (value != this.m_HitPoints && this.MaxHitPoints > 0)
                {
                    this.m_HitPoints = value;

                    if (this.m_HitPoints < 0)
                        this.Delete();
                    else if (this.m_HitPoints > this.MaxHitPoints)
                        this.m_HitPoints = this.MaxHitPoints;

                    this.InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get
            {
                return this.m_Crafter;
            }
            set
            {
                this.m_Crafter = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StrRequirement
        {
            get
            {
                return (int)((double)(m_StrReq == -1 ? (Core.AOS ? AosStrReq : OldStrReq) : m_StrReq));
            }
            set
            {
                this.m_StrReq = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ClothingQuality Quality
        {
            get
            {
                return this.m_Quality;
            }
            set
            {
                this.m_Quality = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerConstructed
        {
            get
            {
                return this.m_PlayerConstructed;
            }
            set
            {
                this.m_PlayerConstructed = value;
            }
        }

        #region Personal Bless Deed
        private Mobile m_BlessedBy;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile BlessedBy
        {
            get
            {
                return this.m_BlessedBy;
            }
            set
            {
                this.m_BlessedBy = value;
                this.InvalidateProperties();
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (this.BlessedFor == from && this.BlessedBy == from && this.RootParent == from)
            {
                list.Add(new UnBlessEntry(from, this));
            }
        }

        private class UnBlessEntry : ContextMenuEntry
        {
            private readonly Mobile m_From;
            private readonly BaseClothing m_Item;

            public UnBlessEntry(Mobile from, BaseClothing item)
                : base(6208, -1)
            {
                this.m_From = from;
                this.m_Item = item; // BaseArmor, BaseWeapon or BaseClothing
            }

            public override void OnClick()
            {
                this.m_Item.BlessedFor = null;
                this.m_Item.BlessedBy = null;

                Container pack = this.m_From.Backpack;

                if (pack != null)
                {
                    pack.DropItem(new PersonalBlessDeed(this.m_From));
                    this.m_From.SendLocalizedMessage(1062200); // A personal bless deed has been placed in your backpack.
                }
            }
        }
        #endregion

        public virtual CraftResource DefaultResource
        {
            get
            {
                return CraftResource.None;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get
            {
                return this.m_Resource;
            }
            set
            {
                this.m_Resource = value;
                this.Hue = CraftResources.GetHue(this.m_Resource);
                this.InvalidateProperties();
            }
        }

        public virtual int BasePhysicalResistance
        {
            get
            {
                return 0;
            }
        }
        public virtual int BaseFireResistance
        {
            get
            {
                return 0;
            }
        }
        public virtual int BaseColdResistance
        {
            get
            {
                return 0;
            }
        }
        public virtual int BasePoisonResistance
        {
            get
            {
                return 0;
            }
        }
        public virtual int BaseEnergyResistance
        {
            get
            {
                return 0;
            }
        }
        
        public virtual int ArtifactRarity
        {
            get
            {
                return 0;
            }
        }

        public virtual int BaseStrBonus
        {
            get
            {
                return 0;
            }
        }
        public virtual int BaseDexBonus
        {
            get
            {
                return 0;
            }
        }
        public virtual int BaseIntBonus
        {
            get
            {
                return 0;
            }
        }

        public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            if (!Ethics.Ethic.CheckTrade(from, to, newOwner, this))
                return false;

            return base.AllowSecureTrade(from, to, newOwner, accepted);
        }

        public virtual Race RequiredRace
        {
            get
            {
                return null;
            }
        }

        public override bool CanEquip(Mobile from)
        {
            if (!Ethics.Ethic.CheckEquip(from, this))
                return false;

            if (from.IsPlayer())
            {
                if (!this.AllowMaleWearer && !from.Female)
                {
                    if (this.AllowFemaleWearer)
                        from.SendLocalizedMessage(1010388); // Only females can wear this.
                    else
                        from.SendMessage("You may not wear this.");

                    return false;
                }
                else if (!this.AllowFemaleWearer && from.Female)
                {
                    if (this.AllowMaleWearer)
                        from.SendLocalizedMessage(1063343); // Only males can wear this.
                    else
                        from.SendMessage("You may not wear this.");

                    return false;
                }
                #region Personal Bless Deed
                else if (this.BlessedBy != null && this.BlessedBy != from)
                {
                    from.SendLocalizedMessage(1075277); // That item is blessed by another player.

                    return false;
                }
                #endregion
            }

            return base.CanEquip(from);
        }

        public virtual int AosStrReq
        {
            get
            {
                return 10;
            }
        }
        public virtual int OldStrReq
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

        public virtual bool AllowMaleWearer
        {
            get
            {
                return true;
            }
        }
        public virtual bool AllowFemaleWearer
        {
            get
            {
                return true;
            }
        }
        public virtual bool CanBeBlessed
        {
            get
            {
                return true;
            }
        }

        public static void ValidateMobile(Mobile m)
        {
            for (int i = m.Items.Count - 1; i >= 0; --i)
            {
                if (i >= m.Items.Count)
                    continue;

                Item item = m.Items[i];

                if (item is BaseClothing)
                {
                    BaseClothing clothing = (BaseClothing)item;

                    if (!clothing.AllowMaleWearer && !m.Female && m.AccessLevel < AccessLevel.GameMaster)
                    {
                        if (clothing.AllowFemaleWearer)
                            m.SendLocalizedMessage(1010388); // Only females can wear this.
                        else
                            m.SendMessage("You may not wear this.");

                        m.AddToBackpack(clothing);
                    }
                    else if (!clothing.AllowFemaleWearer && m.Female && m.AccessLevel < AccessLevel.GameMaster)
                    {
                        if (clothing.AllowMaleWearer)
                            m.SendLocalizedMessage(1063343); // Only males can wear this.
                        else
                            m.SendMessage("You may not wear this.");

                        m.AddToBackpack(clothing);
                    }
                }
            }
        }

        public override void OnAdded(object parent)
        {
            Mobile mob = parent as Mobile;

            if (mob != null)
            {

                mob.CheckStatTimers();
            }

            base.OnAdded(parent);
        }

        public override void OnRemoved(object parent)
        {
            Mobile mob = parent as Mobile;

            if (mob != null)
            {
                string modName = this.Serial.ToString();

                mob.RemoveStatMod(modName + "Str");
                mob.RemoveStatMod(modName + "Dex");
                mob.RemoveStatMod(modName + "Int");

                mob.CheckStatTimers();
            }

            base.OnRemoved(parent);
        }

        public virtual int OnHit(BaseWeapon weapon, int damageTaken)
        {
            if (m_MaxHitPoints == 0)
                return damageTaken;

            int Absorbed = Utility.RandomMinMax(1, 4);

            damageTaken -= Absorbed;

            if (damageTaken < 0) 
                damageTaken = 0;

            if (25 > Utility.Random(100)) // 25% chance to lower durability
            {
                int wear;

                if (weapon.Type == WeaponType.Bashing)
                    wear = Absorbed / 2;
                else
                    wear = Utility.Random(2);

                if (wear > 0 && this.m_MaxHitPoints > 0)
                {
                    if (this.m_HitPoints >= wear)
                    {
                        this.HitPoints -= wear;
                        wear = 0;
                    }
                    else
                    {
                        wear -= this.HitPoints;
                        this.HitPoints = 0;
                    }

                    if (wear > 0)
                    {
                        if (this.m_MaxHitPoints > wear)
                        {
                            this.MaxHitPoints -= wear;

                            if (this.Parent is Mobile)
                                ((Mobile)this.Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.
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

        public BaseClothing(int itemID, Layer layer)
            : this(itemID, layer, 0)
        {
        }

        public BaseClothing(int itemID, Layer layer, int hue)
            : base(itemID)
        {
            this.Layer = layer;
            this.Hue = hue;

            this.m_Resource = this.DefaultResource;
            this.m_Quality = ClothingQuality.Regular;

            this.m_HitPoints = this.m_MaxHitPoints = Utility.RandomMinMax(this.InitMinHits, this.InitMaxHits);
        }

        public BaseClothing(Serial serial)
            : base(serial)
        {
        }

        public override bool AllowEquipedCast(Mobile from)
        {
            if (base.AllowEquipedCast(from))
                return true;

            return (false);
        }

        public override bool CheckPropertyConfliction(Mobile m)
        {
            if (base.CheckPropertyConfliction(m))
                return true;

            if (this.Layer == Layer.Pants)
                return (m.FindItemOnLayer(Layer.InnerLegs) != null);

            if (this.Layer == Layer.Shirt)
                return (m.FindItemOnLayer(Layer.InnerTorso) != null);

            return false;
        }

        private string GetNameString()
        {
            string name = this.Name;

            if (name == null)
                name = String.Format("#{0}", this.LabelNumber);

            return name;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            int oreType;

            switch ( this.m_Resource )
            {
                case CraftResource.DullCopper:
                    oreType = 1053108;
                    break; // dull copper
                case CraftResource.ShadowIron:
                    oreType = 1053107;
                    break; // shadow iron
                case CraftResource.Copper:
                    oreType = 1053106;
                    break; // copper
                case CraftResource.Bronze:
                    oreType = 1053105;
                    break; // bronze
                case CraftResource.Gold:
                    oreType = 1053104;
                    break; // golden
                case CraftResource.Agapite:
                    oreType = 1053103;
                    break; // agapite
                case CraftResource.Verite:
                    oreType = 1053102;
                    break; // verite
                case CraftResource.Valorite:
                    oreType = 1053101;
                    break; // valorite
                case CraftResource.SpinedLeather:
                    oreType = 1061118;
                    break; // spined
                case CraftResource.HornedLeather:
                    oreType = 1061117;
                    break; // horned
                case CraftResource.BarbedLeather:
                    oreType = 1061116;
                    break; // barbed
                case CraftResource.RedScales:
                    oreType = 1060814;
                    break; // red
                case CraftResource.YellowScales:
                    oreType = 1060818;
                    break; // yellow
                case CraftResource.BlackScales:
                    oreType = 1060820;
                    break; // black
                case CraftResource.GreenScales:
                    oreType = 1060819;
                    break; // green
                case CraftResource.WhiteScales:
                    oreType = 1060821;
                    break; // white
                case CraftResource.BlueScales:
                    oreType = 1060815;
                    break; // blue
                default:
                    oreType = 0;
                    break;
            }

            if (oreType != 0)
                list.Add(1053099, "#{0}\t{1}", oreType, this.GetNameString()); // ~1_oretype~ ~2_armortype~
            else if (this.Name == null)
                list.Add(this.LabelNumber);
            else
                list.Add(this.Name);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_Crafter != null)
				list.Add(1050043, m_Crafter.TitleName); // crafted by ~1_NAME~

            #region Factions
            if (this.m_FactionState != null)
                list.Add(1041350); // faction item
            #endregion

            if (this.m_Quality == ClothingQuality.Exceptional)
                list.Add(1060636); // exceptional

            if (this.RequiredRace == Race.Elf)
                list.Add(1075086); // Elves Only

            int prop;

            if ((prop = this.ArtifactRarity) > 0)
                list.Add(1061078, prop.ToString()); // artifact rarity ~1_val~

            base.AddResistanceProperties(list);


            if (this.m_HitPoints >= 0 && this.m_MaxHitPoints > 0)
                list.Add(1060639, "{0}\t{1}", this.m_HitPoints, this.m_MaxHitPoints); // durability ~1_val~ / ~2_val~
        }

        public override void OnSingleClick(Mobile from)
        {
            List<EquipInfoAttribute> attrs = new List<EquipInfoAttribute>();

            this.AddEquipInfoAttributes(from, attrs);

            int number;

            if (this.Name == null)
            {
                number = this.LabelNumber;
            }
            else
            {
                this.LabelTo(from, this.Name);
                number = 1041000;
            }

            if (attrs.Count == 0 && this.Crafter == null && this.Name != null)
                return;

            EquipmentInfo eqInfo = new EquipmentInfo(number, this.m_Crafter, false, attrs.ToArray());

            from.Send(new DisplayEquipmentInfo(this, eqInfo));
        }

        public virtual void AddEquipInfoAttributes(Mobile from, List<EquipInfoAttribute> attrs)
        {
            if (this.DisplayLootType)
            {
                if (this.LootType == LootType.Blessed)
                    attrs.Add(new EquipInfoAttribute(1038021)); // blessed
                else if (this.LootType == LootType.Cursed)
                    attrs.Add(new EquipInfoAttribute(1049643)); // cursed
            }

            #region Factions
            if (this.m_FactionState != null)
                attrs.Add(new EquipInfoAttribute(1041350)); // faction item
            #endregion

            if (this.m_Quality == ClothingQuality.Exceptional)
                attrs.Add(new EquipInfoAttribute(1018305 - (int)this.m_Quality));
        }

        #region Serialization
        private static void SetSaveFlag(ref SaveFlag flags, SaveFlag toSet, bool setIf)
        {
            if (setIf)
                flags |= toSet;
        }

        private static bool GetSaveFlag(SaveFlag flags, SaveFlag toGet)
        {
            return ((flags & toGet) != 0);
        }

        [Flags]
        private enum SaveFlag
        {
            None = 0x00000000,
            Resource = 0x00000001,
            Attributes = 0x00000002,
            ClothingAttributes = 0x00000004,
            SkillBonuses = 0x00000008,
            Resistances = 0x00000010,
            MaxHitPoints = 0x00000020,
            HitPoints = 0x00000040,
            PlayerConstructed = 0x00000080,
            Crafter = 0x00000100,
            Quality = 0x00000200,
            StrReq = 0x00000400,
            NegativeAttributes  = 0x00000800,
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(8); // version

            writer.Write(this.m_BlessedBy);
            
            // Version 5
            SaveFlag flags = SaveFlag.None;

            SetSaveFlag(ref flags, SaveFlag.MaxHitPoints, this.m_MaxHitPoints != 0);
            SetSaveFlag(ref flags, SaveFlag.HitPoints, this.m_HitPoints != 0);
            SetSaveFlag(ref flags, SaveFlag.PlayerConstructed, this.m_PlayerConstructed != false);
            SetSaveFlag(ref flags, SaveFlag.Crafter, this.m_Crafter != null);
            SetSaveFlag(ref flags, SaveFlag.Quality, this.m_Quality != ClothingQuality.Regular);
            SetSaveFlag(ref flags, SaveFlag.StrReq, this.m_StrReq != -1);

            writer.WriteEncodedInt((int)flags);

            if (GetSaveFlag(flags, SaveFlag.Resource))
                writer.WriteEncodedInt((int)this.m_Resource);

            if (GetSaveFlag(flags, SaveFlag.MaxHitPoints))
                writer.WriteEncodedInt((int)this.m_MaxHitPoints);

            if (GetSaveFlag(flags, SaveFlag.HitPoints))
                writer.WriteEncodedInt((int)this.m_HitPoints);

            if (GetSaveFlag(flags, SaveFlag.Crafter))
                writer.Write((Mobile)this.m_Crafter);

            if (GetSaveFlag(flags, SaveFlag.Quality))
                writer.WriteEncodedInt((int)this.m_Quality);

            if (GetSaveFlag(flags, SaveFlag.StrReq))
                writer.WriteEncodedInt((int)this.m_StrReq);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 8:
                        {
                            goto case 7;
                        }
                case 7:
                    {
                        this.m_BlessedBy = reader.ReadMobile();

                        goto case 5;
                    }
                case 5:
                    {
                        SaveFlag flags = (SaveFlag)reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.Resource))
                            this.m_Resource = (CraftResource)reader.ReadEncodedInt();
                        else
                            this.m_Resource = this.DefaultResource;

                        if (GetSaveFlag(flags, SaveFlag.MaxHitPoints))
                            this.m_MaxHitPoints = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.HitPoints))
                            this.m_HitPoints = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.Crafter))
                            this.m_Crafter = reader.ReadMobile();

                        if (GetSaveFlag(flags, SaveFlag.Quality))
                            this.m_Quality = (ClothingQuality)reader.ReadEncodedInt();
                        else
                            this.m_Quality = ClothingQuality.Regular;

                        if (GetSaveFlag(flags, SaveFlag.StrReq))
                            this.m_StrReq = reader.ReadEncodedInt();
                        else
                            this.m_StrReq = -1;

                        if (GetSaveFlag(flags, SaveFlag.PlayerConstructed))
                            this.m_PlayerConstructed = true;

                        break;
                    }
                case 4:
                    {
                        this.m_Resource = (CraftResource)reader.ReadInt();

                        goto case 3;
                    }
                case 3:
                    {
                        goto case 2;
                    }
                case 2:
                    {
                        this.m_PlayerConstructed = reader.ReadBool();
                        goto case 1;
                    }
                case 1:
                    {
                        this.m_Crafter = reader.ReadMobile();
                        this.m_Quality = (ClothingQuality)reader.ReadInt();
                        break;
                    }
                case 0:
                    {
                        this.m_Crafter = null;
                        this.m_Quality = ClothingQuality.Regular;
                        break;
                    }
            }

            if (version < 2)
                this.m_PlayerConstructed = true; // we don't know, so, assume it's crafted

            if (version < 4)
                this.m_Resource = this.DefaultResource;

            if (this.m_MaxHitPoints == 0 && this.m_HitPoints == 0)
                this.m_HitPoints = this.m_MaxHitPoints = Utility.RandomMinMax(this.InitMinHits, this.InitMaxHits);

        }

        #endregion

        public virtual bool Dye(Mobile from, DyeTub sender)
        {
            if (this.Deleted)
                return false;
            else if (this.RootParent is Mobile && from != this.RootParent)
                return false;

            this.Hue = sender.DyedHue;

            return true;
        }

        public virtual bool Scissor(Mobile from, Scissors scissors)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(502437); // Items you wish to cut must be in your backpack.
                return false;
            }

            if (Ethics.Ethic.IsImbued(this))
            {
                from.SendLocalizedMessage(502440); // Scissors can not be used on that to produce anything.
                return false;
            }

            CraftSystem system = DefTailoring.CraftSystem;

            CraftItem item = system.CraftItems.SearchFor(this.GetType());

            if (item != null && item.Resources.Count == 1 && item.Resources.GetAt(0).Amount >= 2)
            {
                try
                {
                    Type resourceType = null;

                    CraftResourceInfo info = CraftResources.GetInfo(this.m_Resource);

                    if (info != null && info.ResourceTypes.Length > 0)
                        resourceType = info.ResourceTypes[0];

                    if (resourceType == null)
                        resourceType = item.Resources.GetAt(0).ItemType;

                    Item res = (Item)Activator.CreateInstance(resourceType);

                    this.ScissorHelper(from, res, this.m_PlayerConstructed ? (item.Resources.GetAt(0).Amount / 2) : 1);

                    res.LootType = LootType.Regular;

                    return true;
                }
                catch
                {
                }
            }

            from.SendLocalizedMessage(502440); // Scissors can not be used on that to produce anything.
            return false;
        }

        #region ICraftable Members

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            this.Quality = (ClothingQuality)quality;

            if (makersMark)
                this.Crafter = from;

            this.PlayerConstructed = true;

            CraftContext context = craftSystem.GetContext(from);

            if (context != null && context.DoNotColor)
                this.Hue = 0;

            return quality;
        }

        #endregion

    }
}