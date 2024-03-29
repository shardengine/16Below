using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
    public enum ResurrectMessage
    {
        ChaosShrine = 0,
        VirtueShrine = 1,
        Healer = 2,
        Generic = 3,
        SilverSapling = 102034,
    }

    public class ResurrectGump : Gump
    {
        private readonly Mobile m_Healer;
        private readonly int m_Price;
        private readonly bool m_FromSacrifice;
        private readonly double m_HitsScalar;
        private readonly ResurrectMessage m_Msg;

        private Action<Mobile> m_Callback;

        public ResurrectGump(Mobile owner)
            : this(owner, owner, ResurrectMessage.Generic, false)
        {
        }

        public ResurrectGump(Mobile owner, double hitsScalar)
            : this(owner, owner, ResurrectMessage.Generic, false, hitsScalar, null)
        {
        }

        public ResurrectGump(Mobile owner, bool fromSacrifice)
            : this(owner, owner, ResurrectMessage.Generic, fromSacrifice)
        {
        }

        public ResurrectGump(Mobile owner, Mobile healer)
            : this(owner, healer, ResurrectMessage.Generic, false)
        {
        }

        public ResurrectGump(Mobile owner, ResurrectMessage msg)
            : this(owner, owner, msg, false)
        {
        }

        public ResurrectGump(Mobile owner, Mobile healer, ResurrectMessage msg)
            : this(owner, healer, msg, false)
        {
        }

        public ResurrectGump(Mobile owner, Mobile healer, ResurrectMessage msg, bool fromSacrifice)
            : this(owner, healer, msg, fromSacrifice, 0.0, null)
        {
        }

        public ResurrectGump(Mobile owner, Mobile healer, ResurrectMessage msg, bool fromSacrifice, double hitsScalar, Action<Mobile> callback)
            : base(100, 0)
        {
            this.m_Healer = healer;
            this.m_FromSacrifice = fromSacrifice;
            this.m_HitsScalar = hitsScalar;

            this.m_Msg = msg;

            this.AddPage(0);

            this.AddBackground(0, 0, 400, 350, 2600);

            this.AddHtmlLocalized(0, 20, 400, 35, 1011022, false, false); // <center>Resurrection</center>

            this.AddHtmlLocalized(50, 55, 300, 140, 1011023 + (int)msg, true, true); /* It is possible for you to be resurrected here by this healer. Do you wish to try?<br>
            * CONTINUE - You chose to try to come back to life now.<br>
            * CANCEL - You prefer to remain a ghost for now.
            */

            m_Callback = callback;

            this.AddButton(200, 227, 4005, 4007, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(235, 230, 110, 35, 1011012, false, false); // CANCEL

            this.AddButton(65, 227, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(100, 230, 110, 35, 1011011, false, false); // CONTINUE
        }

        public ResurrectGump(Mobile owner, Mobile healer, int price)
            : base(150, 50)
        {
            this.m_Healer = healer;
            this.m_Price = price;

            this.Closable = false;

            this.AddPage(0);

            this.AddImage(0, 0, 3600);

            this.AddImageTiled(0, 14, 15, 200, 3603);
            this.AddImageTiled(380, 14, 14, 200, 3605);

            this.AddImage(0, 201, 3606);

            this.AddImageTiled(15, 201, 370, 16, 3607);
            this.AddImageTiled(15, 0, 370, 16, 3601);

            this.AddImage(380, 0, 3602);

            this.AddImage(380, 201, 3608);

            this.AddImageTiled(15, 15, 365, 190, 2624);

            this.AddRadio(30, 140, 9727, 9730, true, 1);
            this.AddHtmlLocalized(65, 145, 300, 25, 1060015, 0x7FFF, false, false); // Grudgingly pay the money

            this.AddRadio(30, 175, 9727, 9730, false, 0);
            this.AddHtmlLocalized(65, 178, 300, 25, 1060016, 0x7FFF, false, false); // I'd rather stay dead, you scoundrel!!!

            this.AddHtmlLocalized(30, 20, 360, 35, 1060017, 0x7FFF, false, false); // Wishing to rejoin the living, are you?  I can restore your body... for a price of course...

            this.AddHtmlLocalized(30, 105, 345, 40, 1060018, 0x5B2D, false, false); // Do you accept the fee, which will be withdrawn from your bank?

            this.AddImage(65, 72, 5605);

            this.AddImageTiled(80, 90, 200, 1, 9107);
            this.AddImageTiled(95, 92, 200, 1, 9157);

            this.AddLabel(90, 70, 1645, price.ToString());
            this.AddHtmlLocalized(140, 70, 100, 25, 1023823, 0x7FFF, false, false); // gold coins

            this.AddButton(290, 175, 247, 248, 2, GumpButtonType.Reply, 0);

            this.AddImageTiled(15, 14, 365, 1, 9107);
            this.AddImageTiled(380, 14, 1, 190, 9105);
            this.AddImageTiled(15, 205, 365, 1, 9107);
            this.AddImageTiled(15, 14, 1, 190, 9105);
            this.AddImageTiled(0, 0, 395, 1, 9157);
            this.AddImageTiled(394, 0, 1, 217, 9155);
            this.AddImageTiled(0, 216, 395, 1, 9157);
            this.AddImageTiled(0, 0, 1, 217, 9155);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            from.CloseGump(typeof(ResurrectGump));

            if (ResurrectMessage.SilverSapling == this.m_Msg && 1 == info.ButtonID)
            {
                PlayerMobile pm = from as PlayerMobile;
                if (null != pm && pm.Region.IsPartOf("Abyss"))
                {
                    if (null != pm.Corpse)
                    {
                        pm.Corpse.Location = pm.Location;
                        pm.Corpse.Map = pm.Map;
                    }
                    pm.Resurrect();
                }
                return;
            }

            if (info.ButtonID == 1 || info.ButtonID == 2)
            {
                if (from.Map == null || !from.Map.CanFit(from.Location, 16, false, false))
                {
                    from.SendLocalizedMessage(502391); // Thou can not be resurrected there!
                    return;
                }

                if (this.m_Price > 0)
                {
                    if (info.IsSwitched(1))
                    {
                        if (Banker.Withdraw(from, this.m_Price))
                        {
                            if (Core.AOS || !Core.bEnforceExpansionClient)
                                from.SendLocalizedMessage(1060398, this.m_Price.ToString()); // ~1_AMOUNT~ gold has been withdrawn from your bank box.
                            else
                            {
                                string temp = string.Format("{0} gold has been withdrawn from your bank box.", this.m_Price.ToString());
                                from.SendMessage(temp);
                            }

                            from.SendLocalizedMessage(1060022, Banker.GetBalance(from).ToString()); // You have ~1_AMOUNT~ gold in cash remaining in your bank box.
                        }
                        else
                        {
                            from.SendLocalizedMessage(1060020); // Unfortunately, you do not have enough cash in your bank to cover the cost of the healing.
                            return;
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1060019); // You decide against paying the healer, and thus remain dead.
                        return;
                    }
                }

                from.PlaySound(0x214);
                from.FixedEffect(0x376A, 10, 16);

                from.Resurrect();

                if (this.m_Healer != null && from != this.m_Healer)
                {
                    VirtueLevel level = VirtueHelper.GetLevel(this.m_Healer, VirtueName.Compassion);

                    switch( level )
                    {
                        case VirtueLevel.Seeker:
                            from.Hits = Utility.RandomMinMax(20, from.HitsMax);
                            break;
                        case VirtueLevel.Follower:
                            from.Hits = Utility.RandomMinMax(40, from.HitsMax);
                            break;
                        case VirtueLevel.Knight:
                            from.Hits = Utility.RandomMinMax(80, from.HitsMax);
                            break;
                    }
                }

                if (this.m_FromSacrifice && from is PlayerMobile)
                {
                    ((PlayerMobile)from).AvailableResurrects -= 1;

                    Container pack = from.Backpack;
                    Container corpse = from.Corpse;

                    if (pack != null && corpse != null)
                    {
                        List<Item> items = new List<Item>(corpse.Items);

                        for (int i = 0; i < items.Count; ++i)
                        {
                            Item item = items[i];

                            if (item.Layer != Layer.Hair && item.Layer != Layer.FacialHair && item.Movable)
                                pack.DropItem(item);
                        }
                    }
                }

                if (from.Fame > 0)
                {
                    int amount = from.Fame / 10;

                    Misc.Titles.AwardFame(from, -amount, true);
                }


                double loss = (100.0 - (4.0 + (from.ShortTermMurders / 5.0))) / 100.0; // 5 to 15% loss

                if (loss < 0.85)
                    loss = 0.85;
                else if (loss > 0.95)
                    loss = 0.95;

                if (from.RawStr * loss > 10)
                    from.RawStr = (int)(from.RawStr * loss);
                if (from.RawInt * loss > 10)
                    from.RawInt = (int)(from.RawInt * loss);
                if (from.RawDex * loss > 10)
                    from.RawDex = (int)(from.RawDex * loss);

                for (int s = 0; s < from.Skills.Length; s++)
                {
                    if (from.Skills[s].Base * loss > 35)
                        from.Skills[s].Base *= loss;
                }

                if (from.Alive && this.m_HitsScalar > 0)
                    from.Hits = (int)(from.HitsMax * this.m_HitsScalar);

                if (m_Callback != null)
                    m_Callback(from);
            }
        }
    }
}