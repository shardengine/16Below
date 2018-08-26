#region Header
// **********
// 16Below - HouseDemolishGump.cs
// **********
#endregion

#region References
using Server.Accounting;
using Server.Configuration;
using Server.Guilds;
using Server.Items;
using Server.Multis;
using Server.Network;
using System;
#endregion

namespace Server.Gumps
{
	public class HouseDemolishGump : Gump
	{
		private readonly BaseHouse m_House;
		private readonly Mobile m_Mobile;

		public HouseDemolishGump(Mobile mobile, BaseHouse house)
			: base(110, 100)
		{
			m_Mobile = mobile;
			m_House = house;

			mobile.CloseGump(typeof(HouseDemolishGump));

			Closable = false;

			AddPage(0);

			AddBackground(0, 0, 420, 280, 5054);

			AddImageTiled(10, 10, 400, 20, 2624);
			AddAlphaRegion(10, 10, 400, 20);

            if(Core.AOS)
			    AddHtmlLocalized(10, 10, 400, 20, 1060635, 30720, false, false); // <CENTER>WARNING</CENTER>
            else
                this.AddHtml(10, 10, 400, 20, this.Color("<CENTER>WARNING</CENTER>", 0xFF0000), false, false);

            AddImageTiled(10, 40, 400, 200, 2624);
			AddAlphaRegion(10, 40, 400, 200);

            if (Core.AOS)
                AddHtmlLocalized(10, 40, 400, 200, 1061795, 32512, false, true); 
            else
            {
			    /* You are about to demolish your house.
                * You will be refunded the house's value directly to your bank box.
                * All items in the house will remain behind and can be freely picked up by anyone.
                * Once the house is demolished, anyone can attempt to place a new house on the vacant land.
                * This action will not un-condemn any other houses on your account, nor will it end your 7-day waiting period (if it applies to you).
                * Are you sure you wish to continue?
                */
               // string text = string.Format("{0}", text)
               //     "You are about to demolish your house. You will be refunded the house's value directly to your bank box. All items in the house will remain behind and can be freely picked up by anyone. Once the house is demolished, anyone can attempt to place a new house on the vacant land. This action will not un - condemn any other houses on your account, nor will it end your 7 - day waiting period (if it applies to you). Are you sure you wish to continue?";

                AddHtml(10, 40, 400, 200, this.Color("You are about to demolish your house. You will be refunded the house's value directly to your bank box. All items in the house will remain behind and can be freely picked up by anyone. Once the house is demolished, anyone can attempt to place a new house on the vacant land. This action will not un - condemn any other houses on your account, nor will it end your 7 - day waiting period (if it applies to you).\r\n\r\nAre you sure you wish to continue?", 0xFFFF00), false, true);
            }


			AddImageTiled(10, 250, 400, 20, 2624);
			AddAlphaRegion(10, 250, 400, 20);

			AddButton(10, 250, 4005, 4007, 1, GumpButtonType.Reply, 0);
			AddHtmlLocalized(40, 250, 170, 20, 1011036, 32767, false, false); // OKAY

			AddButton(210, 250, 4005, 4007, 0, GumpButtonType.Reply, 0);
			AddHtmlLocalized(240, 250, 170, 20, 1011012, 32767, false, false); // CANCEL
		}


        public string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }


        public override void OnResponse(NetState state, RelayInfo info)
		{
			if (info.ButtonID == 1 && !m_House.Deleted)
			{
				if (m_House.IsOwner(m_Mobile))
				{
					if (m_House.MovingCrate != null || m_House.InternalizedVendors.Count > 0)
					{
						return;
					}

					if (!Guild.NewGuildSystem && m_House.FindGuildstone() != null)
					{
						m_Mobile.SendLocalizedMessage(501389); // You cannot redeed a house with a guildstone inside.
						return;
					}
					/*
					if (m_House.PlayerVendors.Count > 0)
                    {
						m_Mobile.SendLocalizedMessage(503236); // You need to collect your vendor's belongings before moving.
						return;
                    }
					*/
					if (m_House.HasRentedVendors && m_House.VendorInventories.Count > 0)
					{
						m_Mobile.SendLocalizedMessage(1062679);
						// You cannot do that that while you still have contract vendors or unclaimed contract vendor inventory in your house.
						return;
					}

					if (m_House.HasRentedVendors)
					{
						m_Mobile.SendLocalizedMessage(1062680);
						// You cannot do that that while you still have contract vendors in your house.
						return;
					}

					if (m_House.VendorInventories.Count > 0)
					{
						m_Mobile.SendLocalizedMessage(1062681);
						// You cannot do that that while you still have unclaimed contract vendor inventory in your house.
						return;
					}

					if (m_Mobile.AccessLevel >= AccessLevel.GameMaster)
					{
						m_Mobile.SendMessage("You do not get a refund for your house as you are not a player");
						m_House.RemoveKeys(m_Mobile);
						m_House.Delete();
					}
					else
					{
						Item toGive;

						if (m_House.IsAosRules || Config.EraMods.AOSHousePlacementTool)
						{
							if (m_House.Price > 0)
							{
								toGive = new BankCheck(m_House.Price);
							}
							else
							{
                                m_Mobile.SendMessage("The house had no value so you received a deed.");
                                toGive = m_House.GetDeed();
							}
						}
						else
						{
							toGive = m_House.GetDeed();

							if (toGive == null && m_House.Price > 0)
							{
								toGive = new BankCheck(m_House.Price);
							}
						}

						if (AccountGold.Enabled && toGive is BankCheck)
						{
							var worth = ((BankCheck)toGive).Worth;

							if (m_Mobile.Account != null && m_Mobile.Account.DepositGold(worth))
							{
								toGive.Delete();

								m_Mobile.SendLocalizedMessage(1060397, worth.ToString("#,0"));
								// ~1_AMOUNT~ gold has been deposited into your bank box.

								m_House.RemoveKeys(m_Mobile);
								m_House.Delete();
								return;
							}
						}

						if (toGive != null)
						{
							var box = m_Mobile.BankBox;

							if (box.TryDropItem(m_Mobile, toGive, false))
							{
								if (toGive is BankCheck)
								{
									m_Mobile.SendLocalizedMessage(1060397, ((BankCheck)toGive).Worth.ToString("#,0"));
									// ~1_AMOUNT~ gold has been deposited into your bank box.
								}

								m_House.RemoveKeys(m_Mobile);
								m_House.Delete();
							}
							else
							{
								toGive.Delete();
								m_Mobile.SendLocalizedMessage(500390); // Your bank box is full.
							}
						}
						else
						{
							m_Mobile.SendMessage("Unable to refund house.");
						}
					}
				}
				else
				{
					m_Mobile.SendLocalizedMessage(501320); // Only the house owner may do this.
				}
			}
		}
	}
}