using System;
using System.Collections;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Configuration;

namespace Server.Commands
{
    public class GMbody
    {
        public static void Initialize()
        {
            CommandSystem.Register("GMbody", AccessLevel.Counselor, new CommandEventHandler(GM_OnCommand));
            CommandSystem.Register("MakeGM", AccessLevel.Counselor, new CommandEventHandler(GM_OnCommand));
        }

        [Aliases("MakeGM")]
        [Usage("GMbody")]
        [Description("Helps staff members get going.")]
        public static void GM_OnCommand(CommandEventArgs e)
        {
            e.Mobile.Target = new GMmeTarget();
        }

        private class GMmeTarget : Target
        {
            private static Mobile m_Mobile;
            public GMmeTarget()
                : base(-1, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile)
                {
                    Mobile targ = (Mobile)targeted;
                    if (from != targ)
                        from.SendMessage("You may only set your own body to GM style.");

                    else
                    {
                        m_Mobile = from;
                        
                        if (Config.Staff.Staffbody)
                        {
                            m_Mobile.BodyValue = 987;

                            if (Config.Staff.UseColoring)
                            {
                                switch (m_Mobile.AccessLevel)
                                {
                                    case AccessLevel.Owner:m_Mobile.Hue = Config.Staff.Owner; break;
                                    case AccessLevel.Developer:m_Mobile.Hue = Config.Staff.Developer; break;
                                    case AccessLevel.Administrator: m_Mobile.Hue = Config.Staff.Administrator; break;
                                    case AccessLevel.Seer: m_Mobile.Hue = Config.Staff.Seer; break;
                                    case AccessLevel.GameMaster: m_Mobile.Hue = Config.Staff.GameMaster; break;
                                    case AccessLevel.Counselor: m_Mobile.Hue = Config.Staff.Counselor; break;
                                }
                            }
                        }

                        if (Config.Staff.CutHair)
                            m_Mobile.HairItemID = 0;

                        if (Config.Staff.CutFacialHair)
                            m_Mobile.FacialHairItemID = 0;

                        CommandLogging.WriteLine(from, "{0} {1} is assuming a GM body", from.AccessLevel, CommandLogging.Format(from));

                        Container pack = from.Backpack;

                        ArrayList ItemsToDelete = new ArrayList();

                        foreach (Item item in from.Items)
                        {
                            if (item.Layer != Layer.Bank && item.Layer != Layer.Hair && item.Layer != Layer.FacialHair && item.Layer != Layer.Mount && item.Layer != Layer.Backpack)
                            {
                                ItemsToDelete.Add(item);
                            }
                        }
                        foreach (Item item in ItemsToDelete)
                            item.Delete();

                        if (pack == null)
                        {
                            pack = new Backpack();
                            pack.Movable = false;

                            from.AddItem(pack);
                        }
                        else
                        {
                            pack.Delete();
                            pack = new Backpack();
                            pack.Movable = false;

                            from.AddItem(pack);
                        }

                        from.Hunger = 20;
                        from.Thirst = 20;
                        from.Fame = 0;
                        from.Karma = 0;
                        from.Kills = 0;
                        from.Hidden = true;
                        from.Blessed = true;
                        from.Hits = from.HitsMax;
                        from.Mana = from.ManaMax;
                        from.Stam = from.StamMax;

                        if (from.IsStaff())
                        {
                            EquipItem(new StaffRobe());

                            PackItem(new GMHidingStone());
                            PackItem(new GMEthereal());
                            PackItem(new StaffOrb(from));

                            PackItem(new Spellbook((ulong)18446744073709551615));

                            from.RawStr = 100;
                            from.RawDex = 100;
                            from.RawInt = 100;

                            from.Hits = from.HitsMax;
                            from.Mana = from.ManaMax;
                            from.Stam = from.StamMax;

                            for (int i = 0; i < targ.Skills.Length; ++i)
                                targ.Skills[i].Base = 120;
                        }

                        if (Config.Staff.GiveBoots)
                        {
                            int color = 0;
                            if (Config.Staff.UseColoring)
                            {
                                switch (m_Mobile.AccessLevel)
                                {
                                    case AccessLevel.Owner: color = Config.Staff.Owner; break;
                                    case AccessLevel.Developer: color = Config.Staff.Developer; break;
                                    case AccessLevel.Administrator: color = Config.Staff.Administrator; break;
                                    case AccessLevel.Seer: color = Config.Staff.Seer; break;
                                    case AccessLevel.GameMaster: color = Config.Staff.GameMaster; break;
                                    case AccessLevel.Counselor: color = Config.Staff.Counselor; break;
                                }
                            }

                            if (from.IsStaff() && from.AccessLevel <= AccessLevel.Spawner)
                                EquipItem(new FurBoots(color));
                            else if (from.AccessLevel == AccessLevel.GameMaster)
                                EquipItem(new FurBoots(color));
                            if (from.AccessLevel == AccessLevel.Seer)
                                EquipItem(new FurBoots(color));
                            if (from.AccessLevel == AccessLevel.Administrator)
                                EquipItem(new FurBoots(color));
                            if (from.AccessLevel == AccessLevel.Developer)
                                EquipItem(new FurBoots(color));
                            if (from.AccessLevel >= AccessLevel.CoOwner)
                                EquipItem(new FurBoots(color));
                        }
                    }
                }
            }

            private static void EquipItem(Item item)
            {
                EquipItem(item, false);
            }

            private static void EquipItem(Item item, bool mustEquip)
            {
                item.LootType = LootType.Blessed;

                if (m_Mobile != null && m_Mobile.EquipItem(item))
                    return;

                Container pack = m_Mobile.Backpack;

                if (!mustEquip && pack != null)
                    pack.DropItem(item);
                else
                    item.Delete();
            }

            private static void PackItem(Item item)
            {
                item.LootType = LootType.Blessed;

                Container pack = m_Mobile.Backpack;

                if (pack != null)
                    pack.DropItem(item);
                else
                    item.Delete();
            }
        }
    }
}