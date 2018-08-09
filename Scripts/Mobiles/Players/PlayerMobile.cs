#region Header
// **********
// 16Below - PlayerMobile.cs
// **********
#endregion

#region References
using System;
using System.Collections;
using System.Collections.Generic;

using Server.Accounting;
using Server.ContextMenus;
using Server.Engines.BulkOrders;
using Server.Engines.CannedEvil;
using Server.Engines.ConPVP;
using Server.Engines.Craft;
using Server.Engines.Help;
using Server.Engines.My16Below;
using Server.Ethics;
using Server.Factions;
using Server.Guilds;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Movement;
using Server.Multis;
using Server.Network;
using Server.Regions;
using Server.SkillHandlers;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Fourth;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Targeting;

using RankDefinition = Server.Guilds.RankDefinition;
#endregion

namespace Server.Mobiles
{

	#region Enums
	[Flags]
	public enum PlayerFlag // First 16 bits are reserved for default-distro use, start custom flags at 0x00010000
	{
		None = 0x00000000,
		Glassblowing = 0x00000001,
		Masonry = 0x00000002,
		SandMining = 0x00000004,
		StoneMining = 0x00000008,
		ToggleMiningStone = 0x00000010,
		KarmaLocked = 0x00000020,
        UseOwnFilter = 0x00000040,
        PublicMy16Below = 0x00000080,
        PagingSquelched = 0x00000100,
        Young = 0x00000200,
        AcceptGuildInvites = 0x00000400,
        DisplayChampionTitle = 0x00000800,
        HasStatReward = 0x00001000
    }

	public enum NpcGuild
	{
		None,
		MagesGuild,
		WarriorsGuild,
		ThievesGuild,
		RangersGuild,
		HealersGuild,
		MinersGuild,
		MerchantsGuild,
		TinkersGuild,
		TailorsGuild,
		FishermensGuild,
		BardsGuild,
		BlacksmithsGuild
	}

	#endregion

	public class PlayerMobile : Mobile, IHonorTarget
	{
		#region Mount Blocking
		public void SetMountBlock(BlockMountType type, TimeSpan duration, bool dismount)
		{
			if (dismount)
			{
				BaseMount.Dismount(this, this, type, duration, false);
			}
			else
			{
				BaseMount.SetMountPrevention(this, type, duration);
			}
		}
		#endregion

		private class CountAndTimeStamp
		{
			private int m_Count;
			private DateTime m_Stamp;

			public DateTime TimeStamp { get { return m_Stamp; } }

			public int Count
			{
				get { return m_Count; }
				set
				{
					m_Count = value;
					m_Stamp = DateTime.UtcNow;
				}
			}
		}

		private DesignContext m_DesignContext;

		private NpcGuild m_NpcGuild;
		private DateTime m_NpcGuildJoinTime;
		private TimeSpan m_NpcGuildGameTime;
		private PlayerFlag m_Flags;
		private int m_Profession;

		private DateTime m_LastOnline;
		private RankDefinition m_GuildRank;

		private int m_GuildMessageHue, m_AllianceMessageHue;

		private List<Mobile> m_AutoStabled;
		private List<Mobile> m_AllFollowers;
		private List<Mobile> m_RecentlyReported;

		#region Getters & Setters
		public List<Mobile> RecentlyReported { get { return m_RecentlyReported; } set { m_RecentlyReported = value; } }

		public List<Mobile> AutoStabled { get { return m_AutoStabled; } }

		public List<Mobile> AllFollowers
		{
			get
			{
				if (m_AllFollowers == null)
				{
					m_AllFollowers = new List<Mobile>();
				}
				return m_AllFollowers;
			}
		}

		public RankDefinition GuildRank
		{
			get
			{
				if (AccessLevel >= AccessLevel.GameMaster)
				{
					return RankDefinition.Leader;
				}
				else
				{
					return m_GuildRank;
				}
			}
			set { m_GuildRank = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int GuildMessageHue { get { return m_GuildMessageHue; } set { m_GuildMessageHue = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int AllianceMessageHue { get { return m_AllianceMessageHue; } set { m_AllianceMessageHue = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Profession { get { return m_Profession; } set { m_Profession = value; } }

		public int StepsTaken { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public NpcGuild NpcGuild { get { return m_NpcGuild; } set { m_NpcGuild = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime NpcGuildJoinTime { get { return m_NpcGuildJoinTime; } set { m_NpcGuildJoinTime = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime NextBODTurnInTime { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastOnline { get { return m_LastOnline; } set { m_LastOnline = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public long LastMoved { get { return LastMoveTime; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan NpcGuildGameTime { get { return m_NpcGuildGameTime; } set { m_NpcGuildGameTime = value; } }
		#endregion

		#region PlayerFlags
		public PlayerFlag Flags { get { return m_Flags; } set { m_Flags = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool PagingSquelched { get { return GetFlag(PlayerFlag.PagingSquelched); } set { SetFlag(PlayerFlag.PagingSquelched, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Glassblowing { get { return GetFlag(PlayerFlag.Glassblowing); } set { SetFlag(PlayerFlag.Glassblowing, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Masonry { get { return GetFlag(PlayerFlag.Masonry); } set { SetFlag(PlayerFlag.Masonry, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool SandMining { get { return GetFlag(PlayerFlag.SandMining); } set { SetFlag(PlayerFlag.SandMining, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool StoneMining { get { return GetFlag(PlayerFlag.StoneMining); } set { SetFlag(PlayerFlag.StoneMining, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool ToggleMiningStone { get { return GetFlag(PlayerFlag.ToggleMiningStone); } set { SetFlag(PlayerFlag.ToggleMiningStone, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool KarmaLocked { get { return GetFlag(PlayerFlag.KarmaLocked); } set { SetFlag(PlayerFlag.KarmaLocked, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool UseOwnFilter { get { return GetFlag(PlayerFlag.UseOwnFilter); } set { SetFlag(PlayerFlag.UseOwnFilter, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool PublicMy16Below
		{
			get { return GetFlag(PlayerFlag.PublicMy16Below); }
			set
			{
				SetFlag(PlayerFlag.PublicMy16Below, value);
				InvalidateMy16Below();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool AcceptGuildInvites { get { return GetFlag(PlayerFlag.AcceptGuildInvites); } set { SetFlag(PlayerFlag.AcceptGuildInvites, value); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool HasStatReward { get { return GetFlag(PlayerFlag.HasStatReward); } set { SetFlag(PlayerFlag.HasStatReward, value); } }
        #endregion

		private DateTime m_AnkhNextUse;

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime AnkhNextUse { get { return m_AnkhNextUse; } set { m_AnkhNextUse = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan DisguiseTimeLeft { get { return DisguiseTimers.TimeRemaining(this); } }

		private DateTime m_PeacedUntil;

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime PeacedUntil { get { return m_PeacedUntil; } set { m_PeacedUntil = value; } }

		[CommandProperty(AccessLevel.Decorator)]
		public override string TitleName
		{
			get
			{
				string title = Titles.ComputeFameTitle(this);
				return title.Length > 0 ? title : RawName;
			}
		}

		#region Scroll of Alacrity
		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime AcceleratedStart { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public SkillName AcceleratedSkill { get; set; }
		#endregion

		public static Direction GetDirection4(Point3D from, Point3D to)
		{
			int dx = from.X - to.X;
			int dy = from.Y - to.Y;

			int rx = dx - dy;
			int ry = dx + dy;

			Direction ret;

			if (rx >= 0 && ry >= 0)
			{
				ret = Direction.West;
			}
			else if (rx >= 0 && ry < 0)
			{
				ret = Direction.South;
			}
			else if (rx < 0 && ry < 0)
			{
				ret = Direction.East;
			}
			else
			{
				ret = Direction.North;
			}

			return ret;
		}

		public override bool OnDroppedItemToWorld(Item item, Point3D location)
		{
			if (!base.OnDroppedItemToWorld(item, location))
			{
				return false;
			}

			if (Core.AOS)
			{
				IPooledEnumerable mobiles = Map.GetMobilesInRange(location, 0);

				foreach (Mobile m in mobiles)
				{
					if (m.Z >= location.Z && m.Z < location.Z + 16)
					{
						mobiles.Free();
						return false;
					}
				}

				mobiles.Free();
			}

			BounceInfo bi = item.GetBounce();

			if (bi != null)
			{
				Type type = item.GetType();

				if (type.IsDefined(typeof(FurnitureAttribute), true) || type.IsDefined(typeof(DynamicFlipingAttribute), true))
				{
					var objs = type.GetCustomAttributes(typeof(FlipableAttribute), true);

					if (objs != null && objs.Length > 0)
					{
						FlipableAttribute fp = objs[0] as FlipableAttribute;

						if (fp != null)
						{
							var itemIDs = fp.ItemIDs;

							Point3D oldWorldLoc = bi.m_WorldLoc;
							Point3D newWorldLoc = location;

							if (oldWorldLoc.X != newWorldLoc.X || oldWorldLoc.Y != newWorldLoc.Y)
							{
								Direction dir = GetDirection4(oldWorldLoc, newWorldLoc);

								if (itemIDs.Length == 2)
								{
									switch (dir)
									{
										case Direction.North:
										case Direction.South:
											item.ItemID = itemIDs[0];
											break;
										case Direction.East:
										case Direction.West:
											item.ItemID = itemIDs[1];
											break;
									}
								}
								else if (itemIDs.Length == 4)
								{
									switch (dir)
									{
										case Direction.South:
											item.ItemID = itemIDs[0];
											break;
										case Direction.East:
											item.ItemID = itemIDs[1];
											break;
										case Direction.North:
											item.ItemID = itemIDs[2];
											break;
										case Direction.West:
											item.ItemID = itemIDs[3];
											break;
									}
								}
							}
						}
					}
				}
			}

			return true;
		}

		public override int GetPacketFlags()
		{
			int flags = base.GetPacketFlags();

			return flags;
		}

		public override int GetOldPacketFlags()
		{
			int flags = base.GetOldPacketFlags();

			return flags;
		}

		public bool GetFlag(PlayerFlag flag)
		{
			return ((m_Flags & flag) != 0);
		}

		public void SetFlag(PlayerFlag flag, bool value)
		{
			if (value)
			{
				m_Flags |= flag;
			}
			else
			{
				m_Flags &= ~flag;
			}
		}

		public DesignContext DesignContext { get { return m_DesignContext; } set { m_DesignContext = value; } }

		public static void Initialize()
		{
			if (FastwalkPrevention)
			{
				PacketHandlers.RegisterThrottler(0x02, MovementThrottle_Callback);
			}

			EventSink.Login += OnLogin;
			EventSink.Logout += OnLogout;
			EventSink.Connected += EventSink_Connected;
			EventSink.Disconnected += EventSink_Disconnected;
		}

        public override int GetMaxResistance(ResistanceType type)
        {
            if (IsStaff())
            {
                return 100;
            }
 
            int max = base.GetMaxResistance(type);
 
            if (type != ResistanceType.Physical && 60 < max && CurseSpell.UnderEffect(this))
            {
                max = 60;
            }
 
            return Math.Max(MinPlayerResistance, Math.Max(MaxPlayerResistance, max));
        }

		public override int MaxWeight { get { return (((Core.ML && Race == Race.Human) ? 100 : 40) + (int)(3.5 * Str)); } }

		private int m_LastGlobalLight = -1, m_LastPersonalLight = -1;

		public override void OnNetStateChanged()
		{
			m_LastGlobalLight = -1;
			m_LastPersonalLight = -1;
		}

		public override void ComputeBaseLightLevels(out int global, out int personal)
		{
			global = LightCycle.ComputeLevelFor(this);


			if (LightLevel < 21)
			{
				personal = 21;
			}
			else
			{
				personal = LightLevel;
			}
		}

		public override void CheckLightLevels(bool forceResend)
		{
			NetState ns = NetState;

			if (ns == null)
			{
				return;
			}

			int global, personal;

			ComputeLightLevels(out global, out personal);

			if (!forceResend)
			{
				forceResend = (global != m_LastGlobalLight || personal != m_LastPersonalLight);
			}

			if (!forceResend)
			{
				return;
			}

			m_LastGlobalLight = global;
			m_LastPersonalLight = personal;

			ns.Send(GlobalLightLevel.Instantiate(global));
			ns.Send(new PersonalLightLevel(this, personal));
		}

		public override int GetMinResistance(ResistanceType type)
		{
			if (IsStaff())
			{
				return -100;
			}

			int magicResist = (int)(Skills[SkillName.MagicResist].Value * 10);
			int min = int.MinValue;

			if (magicResist >= 1000)
			{
				min = 40 + ((magicResist - 1000) / 50);
			}
			else if (magicResist >= 400)
			{
				min = (magicResist - 400) / 15;
			}

			return Math.Max(MinPlayerResistance, Math.Min(MaxPlayerResistance, min));
		}

		private static void OnLogin(LoginEventArgs e)
		{
			Mobile from = e.Mobile;

			CheckAtrophies(from);

			if (AccountHandler.LockdownLevel > AccessLevel.VIP)
			{
				string notice;

				Account acct = from.Account as Account;

				if (acct == null || !acct.HasAccess(from.NetState))
				{
					if (from.IsPlayer())
					{
						notice = "The server is currently under lockdown. No players are allowed to log in at this time.";
					}
					else
					{
						notice = "The server is currently under lockdown. You do not have sufficient access level to connect.";
					}

					Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(Disconnect), from);
				}
				else if (from.AccessLevel >= AccessLevel.Administrator)
				{
					notice =
						"The server is currently under lockdown. As you are an administrator, you may change this from the [Admin gump.";
				}
				else
				{
					notice = "The server is currently under lockdown. You have sufficient access level to connect.";
				}

				from.SendGump(new NoticeGump(1060637, 30720, notice, 0xFFC000, 300, 140, null, null));
				return;
			}

			if (from is PlayerMobile)
			{
				((PlayerMobile)from).ClaimAutoStabledPets();
			}
		}

		private bool m_NoDeltaRecursion;

		public void ValidateEquipment()
		{
			if (m_NoDeltaRecursion || Map == null || Map == Map.Internal)
			{
				return;
			}

			if (Items == null)
			{
				return;
			}

			m_NoDeltaRecursion = true;
			Timer.DelayCall(TimeSpan.Zero, ValidateEquipment_Sandbox);
		}

		private void ValidateEquipment_Sandbox()
		{
			try
			{
				if (Map == null || Map == Map.Internal)
				{
					return;
				}

				var items = Items;

				if (items == null)
				{
					return;
				}

				bool moved = false;

				int str = Str;
				int dex = Dex;
				int intel = Int;

				#region Factions
				int factionItemCount = 0;
				#endregion

				Mobile from = this;

				#region Ethics
				Ethic ethic = Ethic.Find(from);
				#endregion

				for (int i = items.Count - 1; i >= 0; --i)
				{
					if (i >= items.Count)
					{
						continue;
					}

					Item item = items[i];

					#region Ethics
					if ((item.SavedFlags & 0x100) != 0)
					{
						if (item.Hue != Ethic.Hero.Definition.PrimaryHue)
						{
							item.SavedFlags &= ~0x100;
						}
						else if (ethic != Ethic.Hero)
						{
							from.AddToBackpack(item);
							moved = true;
							continue;
						}
					}
					else if ((item.SavedFlags & 0x200) != 0)
					{
						if (item.Hue != Ethic.Evil.Definition.PrimaryHue)
						{
							item.SavedFlags &= ~0x200;
						}
						else if (ethic != Ethic.Evil)
						{
							from.AddToBackpack(item);
							moved = true;
							continue;
						}
					}
					#endregion

					if (item is BaseWeapon)
					{
						BaseWeapon weapon = (BaseWeapon)item;

						bool drop = false;

						if (dex < weapon.DexRequirement)
						{
							drop = true;
						}
						else if (str < MathHelper.Scale(weapon.StrRequirement, 100))
						{
							drop = true;
						}
						else if (intel < weapon.IntRequirement)
						{
							drop = true;
						}
						if (drop)
						{
							string name = weapon.Name;

							if (name == null)
							{
								name = String.Format("#{0}", weapon.LabelNumber);
							}

							from.SendLocalizedMessage(1062001, name); // You can no longer wield your ~1_WEAPON~
							from.AddToBackpack(weapon);
							moved = true;
						}
					}
					else if (item is BaseArmor)
					{
						BaseArmor armor = (BaseArmor)item;

						bool drop = false;

						if (!armor.AllowMaleWearer && !from.Female && from.AccessLevel < AccessLevel.GameMaster)
						{
							drop = true;
						}
						else if (!armor.AllowFemaleWearer && from.Female && from.AccessLevel < AccessLevel.GameMaster)
						{
							drop = true;
						}

						if (drop)
						{
							string name = armor.Name;

							if (name == null)
							{
								name = String.Format("#{0}", armor.LabelNumber);
							}

							if (armor is BaseShield)
							{
								from.SendLocalizedMessage(1062003, name); // You can no longer equip your ~1_SHIELD~
							}
							else
							{
								from.SendLocalizedMessage(1062002, name); // You can no longer wear your ~1_ARMOR~
							}

							from.AddToBackpack(armor);
							moved = true;
						}
					}
					else if (item is BaseClothing)
					{
						BaseClothing clothing = (BaseClothing)item;

						bool drop = false;

						if (!clothing.AllowMaleWearer && !from.Female && from.AccessLevel < AccessLevel.GameMaster)
						{
							drop = true;
						}
						else if (!clothing.AllowFemaleWearer && from.Female && from.AccessLevel < AccessLevel.GameMaster)
						{
							drop = true;
						}

						if (drop)
						{
							string name = clothing.Name;

							if (name == null)
							{
								name = String.Format("#{0}", clothing.LabelNumber);
							}

							from.SendLocalizedMessage(1062002, name); // You can no longer wear your ~1_ARMOR~

							from.AddToBackpack(clothing);
							moved = true;
						}
					}

					FactionItem factionItem = FactionItem.Find(item);

					if (factionItem != null)
					{
						bool drop = false;

						Faction ourFaction = Faction.Find(this);

						if (ourFaction == null || ourFaction != factionItem.Faction)
						{
							drop = true;
						}
						else if (++factionItemCount > FactionItem.GetMaxWearables(this))
						{
							drop = true;
						}

						if (drop)
						{
							from.AddToBackpack(item);
							moved = true;
						}
					}
				}

				if (moved)
				{
					from.SendLocalizedMessage(500647); // Some equipment has been moved to your backpack.
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			finally
			{
				m_NoDeltaRecursion = false;
			}
		}

		public override void Delta(MobileDelta flag)
		{
			base.Delta(flag);

			if ((flag & MobileDelta.Stat) != 0)
			{
				ValidateEquipment();
			}

			if ((flag & (MobileDelta.Name | MobileDelta.Hue)) != 0)
			{
				InvalidateMy16Below();
			}
		}

		private static void Disconnect(object state)
		{
			NetState ns = ((Mobile)state).NetState;

			if (ns != null)
			{
				ns.Dispose();
			}
		}

		private static void OnLogout(LogoutEventArgs e)
		{
			if (e.Mobile is PlayerMobile)
			{
				((PlayerMobile)e.Mobile).AutoStablePets();
			}

			#region Scroll of Alacrity
			if (((PlayerMobile)e.Mobile).AcceleratedStart > DateTime.UtcNow)
			{
				((PlayerMobile)e.Mobile).AcceleratedStart = DateTime.UtcNow;
				ScrollofAlacrity.AlacrityEnd(e.Mobile);
			}
			#endregion
		}

		private static void EventSink_Connected(ConnectedEventArgs e)
		{
			PlayerMobile pm = e.Mobile as PlayerMobile;

			if (pm != null)
			{
				pm.m_SessionStart = DateTime.UtcNow;
				pm.BedrollLogout = false;
				pm.LastOnline = DateTime.UtcNow;
			}

			DisguiseTimers.StartTimer(e.Mobile);

			Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(ClearSpecialMovesCallback), e.Mobile);
		}

		private static void ClearSpecialMovesCallback(object state)
		{
			Mobile from = (Mobile)state;

			SpecialMove.ClearAllMoves(from);
		}

		private static void EventSink_Disconnected(DisconnectedEventArgs e)
		{
			Mobile from = e.Mobile;
			DesignContext context = DesignContext.Find(from);

			if (context != null)
			{
				/* Client disconnected
				*  - Remove design context
				*  - Eject all from house
				*  - Restore relocated entities
				*/
				// Remove design context
				DesignContext.Remove(from);

				// Eject all from house
				from.RevealingAction();

				foreach (Item item in context.Foundation.GetItems())
				{
					item.Location = context.Foundation.BanLocation;
				}

				foreach (Mobile mobile in context.Foundation.GetMobiles())
				{
					mobile.Location = context.Foundation.BanLocation;
				}

				// Restore relocated entities
				context.Foundation.RestoreRelocatedEntities();
			}

			PlayerMobile pm = e.Mobile as PlayerMobile;

			if (pm != null)
			{
				pm.m_GameTime += (DateTime.UtcNow - pm.m_SessionStart);

				pm.m_SpeechLog = null;
				pm.LastOnline = DateTime.UtcNow;
			}

			DisguiseTimers.StopTimer(from);
		}

		public override void RevealingAction()
		{
			if (m_DesignContext != null)
			{
				return;
			}

			InvisibilitySpell.RemoveTimer(this);

			base.RevealingAction();
		}

		public override void OnHiddenChanged()
		{
			base.OnHiddenChanged();

			RemoveBuff(BuffIcon.Invisibility);
			//Always remove, default to the hiding icon EXCEPT in the invis spell where it's explicitly set

			if (!Hidden)
			{
				RemoveBuff(BuffIcon.HidingAndOrStealth);
			}
			else // if( !InvisibilitySpell.HasTimer( this ) )
			{
				BuffInfo.AddBuff(this, new BuffInfo(BuffIcon.HidingAndOrStealth, 1075655)); //Hidden/Stealthing & You Are Hidden
			}
		}

		public override void OnSubItemAdded(Item item)
		{
			if (AccessLevel < AccessLevel.GameMaster && item.IsChildOf(Backpack))
			{
				int maxWeight = WeightOverloading.GetMaxWeight(this);
				int curWeight = BodyWeight + TotalWeight;

				if (curWeight > maxWeight)
				{
					SendLocalizedMessage(1019035, true, String.Format(" : {0} / {1}", curWeight, maxWeight));
				}
			}
		}

		public override bool CanBeHarmful(Mobile target, bool message, bool ignoreOurBlessedness)
		{
			if (m_DesignContext != null || (target is PlayerMobile && ((PlayerMobile)target).m_DesignContext != null))
			{
				return false;
			}

			if ((target is BaseVendor && ((BaseVendor)target).IsInvulnerable) || target is PlayerVendor || target is TownCrier)
			{
				if (message)
				{
					if (target.Title == null)
					{
						SendMessage("{0} the vendor cannot be harmed.", target.Name);
					}
					else
					{
						SendMessage("{0} {1} cannot be harmed.", target.Name, target.Title);
					}
				}

				return false;
			}

			return base.CanBeHarmful(target, message, ignoreOurBlessedness);
		}

		public override bool CanBeBeneficial(Mobile target, bool message, bool allowDead)
		{
			if (m_DesignContext != null || (target is PlayerMobile && ((PlayerMobile)target).m_DesignContext != null))
			{
				return false;
			}

			return base.CanBeBeneficial(target, message, allowDead);
		}

		public override bool CheckContextMenuDisplay(IEntity target)
		{
			return (m_DesignContext == null);
		}

		public override void OnItemAdded(Item item)
		{
			base.OnItemAdded(item);

			if (item is BaseArmor || item is BaseWeapon)
			{
				Hits = Hits;
				Stam = Stam;
				Mana = Mana;
			}

			if (NetState != null)
			{
				CheckLightLevels(false);
			}

			InvalidateMy16Below();
		}

		public override void OnItemRemoved(Item item)
		{
			base.OnItemRemoved(item);

			if (item is BaseArmor || item is BaseWeapon)
			{
				Hits = Hits;
				Stam = Stam;
				Mana = Mana;
			}

			if (NetState != null)
			{
				CheckLightLevels(false);
			}

			InvalidateMy16Below();
		}

		public override double ArmorRating
		{
			get
			{
				//BaseArmor ar;
				double rating = 0.0;

				AddArmorRating(ref rating, NeckArmor);
				AddArmorRating(ref rating, HandArmor);
				AddArmorRating(ref rating, HeadArmor);
				AddArmorRating(ref rating, ArmsArmor);
				AddArmorRating(ref rating, LegsArmor);
				AddArmorRating(ref rating, ChestArmor);
				AddArmorRating(ref rating, ShieldArmor);

				return VirtualArmor + VirtualArmorMod + rating;
			}
		}

		private void AddArmorRating(ref double rating, Item armor)
		{
			BaseArmor ar = armor as BaseArmor;

			if (ar != null)
			{
				rating += ar.ArmorRatingScaled;
			}
		}

		#region [Stats]Max
		[CommandProperty(AccessLevel.GameMaster)]
		public override int HitsMax
		{
			get
			{
				int strBase;
				int strOffs = GetStatOffset(StatType.Str);

				strBase = RawStr;

				return (strBase / 2) + 50 + strOffs;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public override int StamMax { get { return base.StamMax; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public override int ManaMax { get
		{
			return base.ManaMax;
		} }
		#endregion

		#region Stat Getters/Setters
		[CommandProperty(AccessLevel.GameMaster)]
		public override int Str
		{
			get
			{
				if (Core.ML && IsPlayer())
				{
					return Math.Min(base.Str, 150);
				}

				return base.Str;
			}
			set { base.Str = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public override int Int
		{
			get
			{
				if (Core.ML && IsPlayer())
				{
					return Math.Min(base.Int, 150);
				}

				return base.Int;
			}
			set { base.Int = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public override int Dex
		{
			get
			{
				if (Core.ML && IsPlayer())
				{
					return Math.Min(base.Dex, 150);
				}

				return base.Dex;
			}
			set { base.Dex = value; }
		}
		#endregion

        public long NextPassiveDetectHidden { get; set; }

		public override bool Move(Direction d)
		{
			NetState ns = NetState;

			if (ns != null)
			{
				if (HasGump(typeof(ResurrectGump)))
				{
					if (Alive)
					{
						CloseGump(typeof(ResurrectGump));
					}
					else
					{
						SendLocalizedMessage(500111); // You are frozen and cannot move.
						return false;
					}
				}
			}

			int speed = ComputeMovementSpeed(d);

			bool res;

			if (!Alive)
			{
				MovementImpl.IgnoreMovableImpassables = true;
			}

			res = base.Move(d);

			MovementImpl.IgnoreMovableImpassables = false;

			if (!res)
			{
				return false;
			}

			m_NextMovementTime += speed;

            if (Core.TickCount - NextPassiveDetectHidden >= 0)
            {
                DetectHidden.DoPassiveDetect(this);
                NextPassiveDetectHidden = Core.TickCount + (int)TimeSpan.FromSeconds(2).TotalMilliseconds;
            }
			return true;
		}

		public override bool CheckMovement(Direction d, out int newZ)
		{
			DesignContext context = m_DesignContext;

			if (context == null)
			{
				return base.CheckMovement(d, out newZ);
			}

			HouseFoundation foundation = context.Foundation;

			newZ = foundation.Z + HouseFoundation.GetLevelZ(context.Level, context.Foundation);

			int newX = X, newY = Y;
			Movement.Movement.Offset(d, ref newX, ref newY);

			int startX = foundation.X + foundation.Components.Min.X + 1;
			int startY = foundation.Y + foundation.Components.Min.Y + 1;
			int endX = startX + foundation.Components.Width - 1;
			int endY = startY + foundation.Components.Height - 2;

			return (newX >= startX && newY >= startY && newX < endX && newY < endY && Map == foundation.Map);
		}

		public override bool AllowItemUse(Item item)
		{
			#region Dueling
			if (m_DuelContext != null && !m_DuelContext.AllowItemUse(this, item))
			{
				return false;
			}
			#endregion

			return DesignContext.Check(this);
		}

		public override bool AllowSkillUse(SkillName skill)
		{
			#region Dueling
			if (m_DuelContext != null && !m_DuelContext.AllowSkillUse(this, skill))
			{
				return false;
			}
			#endregion

			return DesignContext.Check(this);
		}

		private bool m_LastProtectedMessage;
		private int m_NextProtectionCheck = 10;

		public virtual void RecheckTownProtection()
		{
			m_NextProtectionCheck = 10;

			GuardedRegion reg = (GuardedRegion)Region.GetRegion(typeof(GuardedRegion));
			bool isProtected = (reg != null && !reg.IsDisabled());

			if (isProtected != m_LastProtectedMessage)
			{
				if (isProtected)
				{
					SendLocalizedMessage(500112); // You are now under the protection of the town guards.
				}
				else
				{
					SendLocalizedMessage(500113); // You have left the protection of the town guards.
				}

				m_LastProtectedMessage = isProtected;
			}
		}

		public override void MoveToWorld(Point3D loc, Map map)
		{
			base.MoveToWorld(loc, map);

			RecheckTownProtection();
		}

		public override void SetLocation(Point3D loc, bool isTeleport)
		{
			if (!isTeleport && IsPlayer() && !Flying)
			{
				// moving, not teleporting
				int zDrop = (Location.Z - loc.Z);

				if (zDrop > 20) // we fell more than one story
				{
					Hits -= ((zDrop / 20) * 10) - 5; // deal some damage; does not kill, disrupt, etc
                    SendMessage("Ouch!");
				}
			}

			base.SetLocation(loc, isTeleport);

			if (isTeleport || --m_NextProtectionCheck == 0)
			{
				RecheckTownProtection();
			}
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			if (from == this)
			{
				BaseHouse house = BaseHouse.FindHouseAt(this);

				if (house != null)
				{
					if (Alive && house.InternalizedVendors.Count > 0 && house.IsOwner(this))
					{
						list.Add(new CallbackEntry(6204, GetVendor));
					}

					if (house.IsAosRules && !Region.IsPartOf(typeof(SafeZone))) // Dueling
					{
						list.Add(new CallbackEntry(6207, LeaveHouse));
					}
				}

                if (Core.Expansion >= Expansion.AOS)
                {
				    if (m_JusticeProtectors.Count > 0)
				    {
					    list.Add(new CallbackEntry(6157, CancelProtection));
				    }

				    if (Alive)
				    {
					    list.Add(new CallbackEntry(6210, ToggleChampionTitleDisplay));
				    }

                }
            }
			else
			{
				BaseHouse curhouse = BaseHouse.FindHouseAt(this);

				if (curhouse != null)
				{
					if (Alive && Core.Expansion >= Expansion.AOS && curhouse.IsAosRules && curhouse.IsFriend(from))
					{
						list.Add(new EjectPlayerEntry(from, this));
					}
				}
			}
		}

		private void CancelProtection()
		{
			for (int i = 0; i < m_JusticeProtectors.Count; ++i)
			{
				Mobile prot = m_JusticeProtectors[i];

				string args = String.Format("{0}\t{1}", Name, prot.Name);

				prot.SendLocalizedMessage(1049371, args);
				// The protective relationship between ~1_PLAYER1~ and ~2_PLAYER2~ has been ended.
				SendLocalizedMessage(1049371, args);
				// The protective relationship between ~1_PLAYER1~ and ~2_PLAYER2~ has been ended.
			}

			m_JusticeProtectors.Clear();
		}

        private void GetVendor()
		{
			BaseHouse house = BaseHouse.FindHouseAt(this);

			if (CheckAlive() && house != null && house.IsOwner(this) && house.InternalizedVendors.Count > 0)
			{
				CloseGump(typeof(ReclaimVendorGump));
				SendGump(new ReclaimVendorGump(house));
			}
		}

		private void LeaveHouse()
		{
			BaseHouse house = BaseHouse.FindHouseAt(this);

			if (house != null)
			{
				Location = house.BanLocation;
			}
		}

		private delegate void ContextCallback();

		private class CallbackEntry : ContextMenuEntry
		{
			private readonly ContextCallback m_Callback;

			public CallbackEntry(int number, ContextCallback callback)
				: this(number, -1, callback)
			{ }

			public CallbackEntry(int number, int range, ContextCallback callback)
				: base(number, range)
			{
				m_Callback = callback;
			}

			public override void OnClick()
			{
				if (m_Callback != null)
				{
					m_Callback();
				}
			}
		}

		public override void DisruptiveAction()
		{
			if (Meditating)
			{
				RemoveBuff(BuffIcon.ActiveMeditation);
			}

			base.DisruptiveAction();
		}
        public override bool Meditating
        {
            set
            {
                base.Meditating = value;
                if (value == false)
                {
                    RemoveBuff(BuffIcon.ActiveMeditation);
                }
            }
        }

		public override void OnDoubleClick(Mobile from)
		{
			if (this == from && !Warmode)
			{
				IMount mount = Mount;

				if (mount != null && !DesignContext.Check(this))
				{
					return;
				}
			}

			base.OnDoubleClick(from);
		}

		public override void DisplayPaperdollTo(Mobile to)
		{
			if (DesignContext.Check(this))
			{
				base.DisplayPaperdollTo(to);
			}
		}

		private static bool m_NoRecursion;

		public override bool CheckEquip(Item item)
		{
			if (!base.CheckEquip(item))
			{
				return false;
			}

			#region Dueling
			if (m_DuelContext != null && !m_DuelContext.AllowItemEquip(this, item))
			{
				return false;
			}
			#endregion

			#region Factions
			FactionItem factionItem = FactionItem.Find(item);

			if (factionItem != null)
			{
				Faction faction = Faction.Find(this);

				if (faction == null)
				{
					SendLocalizedMessage(1010371); // You cannot equip a faction item!
					return false;
				}
				else if (faction != factionItem.Faction)
				{
					SendLocalizedMessage(1010372); // You cannot equip an opposing faction's item!
					return false;
				}
				else
				{
					int maxWearables = FactionItem.GetMaxWearables(this);

					for (int i = 0; i < Items.Count; ++i)
					{
						Item equiped = Items[i];

						if (item != equiped && FactionItem.Find(equiped) != null)
						{
							if (--maxWearables == 0)
							{
								SendLocalizedMessage(1010373); // You do not have enough rank to equip more faction items!
								return false;
							}
						}
					}
				}
			}
			#endregion

			if (AccessLevel < AccessLevel.GameMaster && item.Layer != Layer.Mount && HasTrade)
			{
				BounceInfo bounce = item.GetBounce();

				if (bounce != null)
				{
					if (bounce.m_Parent is Item)
					{
						Item parent = (Item)bounce.m_Parent;

						if (parent == Backpack || parent.IsChildOf(Backpack))
						{
							return true;
						}
					}
					else if (bounce.m_Parent == this)
					{
						return true;
					}
				}

				SendLocalizedMessage(1004042); // You can only equip what you are already carrying while you have a trade pending.
				return false;
			}

			return true;
		}

		public override bool CheckTrade(
			Mobile to, Item item, SecureTradeContainer cont, bool message, bool checkItems, int plusItems, int plusWeight)
		{
			int msgNum = 0;

			if (cont == null)
			{
				if (to.Holding != null)
				{
					msgNum = 1062727; // You cannot trade with someone who is dragging something.
				}
				else if (HasTrade)
				{
					msgNum = 1062781; // You are already trading with someone else!
				}
				else if (to.HasTrade)
				{
					msgNum = 1062779; // That person is already involved in a trade
				}
			}

			if (msgNum == 0 && item != null)
			{
				if (cont != null)
				{
					plusItems += cont.TotalItems;
					plusWeight += cont.TotalWeight;
				}

				if (Backpack == null || !Backpack.CheckHold(this, item, false, checkItems, plusItems, plusWeight))
				{
					msgNum = 1004040; // You would not be able to hold this if the trade failed.
				}
				else if (to.Backpack == null || !to.Backpack.CheckHold(to, item, false, checkItems, plusItems, plusWeight))
				{
					msgNum = 1004039; // The recipient of this trade would not be able to carry this.
				}
				else
				{
					msgNum = CheckContentForTrade(item);
				}
			}

			if (msgNum == 0)
			{
				return true;
			}

			if (!message)
			{
				return false;
			}

			if (msgNum == 1154111)
			{
				SendLocalizedMessage(msgNum, RawName);
			}
			else
			{
				SendLocalizedMessage(msgNum);
			}

			return false;
		}

		private static int CheckContentForTrade(Item item)
		{
			if (item is TrapableContainer && ((TrapableContainer)item).TrapType != TrapType.None)
			{
				return 1004044; // You may not trade trapped items.
			}

			if (StolenItem.IsStolen(item))
			{
				return 1004043; // You may not trade recently stolen items.
			}

			if (item is Container)
			{
				foreach (Item subItem in item.Items)
				{
					int msg = CheckContentForTrade(subItem);

					if (msg != 0)
					{
						return msg;
					}
				}
			}

			return 0;
		}

		public override bool CheckNonlocalDrop(Mobile from, Item item, Item target)
		{
			if (!base.CheckNonlocalDrop(from, item, target))
			{
				return false;
			}

			if (from.AccessLevel >= AccessLevel.GameMaster)
			{
				return true;
			}

			Container pack = Backpack;
			if (from == this && HasTrade && (target == pack || target.IsChildOf(pack)))
			{
				BounceInfo bounce = item.GetBounce();

				if (bounce != null && bounce.m_Parent is Item)
				{
					Item parent = (Item)bounce.m_Parent;

					if (parent == pack || parent.IsChildOf(pack))
					{
						return true;
					}
				}

				SendLocalizedMessage(1004041); // You can't do that while you have a trade pending.
				return false;
			}

			return true;
		}

		protected override void OnLocationChange(Point3D oldLocation)
		{
			CheckLightLevels(false);

			#region Dueling
			if (m_DuelContext != null)
			{
				m_DuelContext.OnLocationChanged(this);
			}
			#endregion

			DesignContext context = m_DesignContext;

			if (context == null || m_NoRecursion)
			{
				return;
			}

			m_NoRecursion = true;

			HouseFoundation foundation = context.Foundation;

			int newX = X, newY = Y;
			int newZ = foundation.Z + HouseFoundation.GetLevelZ(context.Level, context.Foundation);

			int startX = foundation.X + foundation.Components.Min.X + 1;
			int startY = foundation.Y + foundation.Components.Min.Y + 1;
			int endX = startX + foundation.Components.Width - 1;
			int endY = startY + foundation.Components.Height - 2;

			if (newX >= startX && newY >= startY && newX < endX && newY < endY && Map == foundation.Map)
			{
				if (Z != newZ)
				{
					Location = new Point3D(X, Y, newZ);
				}

				m_NoRecursion = false;
				return;
			}

			Location = new Point3D(foundation.X, foundation.Y, newZ);
			Map = foundation.Map;

			m_NoRecursion = false;
		}

		public override bool OnMoveOver(Mobile m)
		{
			if (m is BaseCreature && !((BaseCreature)m).Controlled)
			{
				return (!Alive || !m.Alive || IsDeadBondedPet || m.IsDeadBondedPet) || (Hidden && IsStaff());
			}

			#region Dueling
			if (Region.IsPartOf(typeof(SafeZone)) && m is PlayerMobile)
			{
				PlayerMobile pm = (PlayerMobile)m;

				if (pm.DuelContext == null || pm.DuelPlayer == null || !pm.DuelContext.Started || pm.DuelContext.Finished ||
					pm.DuelPlayer.Eliminated)
				{
					return true;
				}
			}
			#endregion

			return base.OnMoveOver(m);
		}

		protected override void OnMapChange(Map oldMap)
		{
			if ((Map != Faction.Facet && oldMap == Faction.Facet) || (Map == Faction.Facet && oldMap != Faction.Facet))
			{
				InvalidateProperties();
			}

			#region Dueling
			if (m_DuelContext != null)
			{
				m_DuelContext.OnMapChanged(this);
			}
			#endregion

			DesignContext context = m_DesignContext;

			if (context == null || m_NoRecursion)
			{
				return;
			}

			m_NoRecursion = true;

			HouseFoundation foundation = context.Foundation;

			if (Map != foundation.Map)
			{
				Map = foundation.Map;
			}

			m_NoRecursion = false;
		}

		public override void OnBeneficialAction(Mobile target, bool isCriminal)
		{
			if (m_SentHonorContext != null)
			{
				m_SentHonorContext.OnSourceBeneficialAction(target);
			}

			base.OnBeneficialAction(target, isCriminal);
		}

		public override void OnDamage(int amount, Mobile from, bool willKill)
		{
			int disruptThreshold;


			disruptThreshold = 0;

			if (amount > disruptThreshold)
			{
				BandageContext c = BandageContext.GetContext(this);

				if (c != null)
				{
					c.Slip();
				}
			}

			WeightOverloading.FatigueOnDamage(this, amount);

			if (m_ReceivedHonorContext != null)
			{
				m_ReceivedHonorContext.OnTargetDamaged(from, amount);
			}
			if (m_SentHonorContext != null)
			{
				m_SentHonorContext.OnSourceDamaged(from, amount);
			}

			base.OnDamage(amount, from, willKill);
		}

		public override void Resurrect()
		{
			bool wasAlive = Alive;

			base.Resurrect();

			if (Alive && !wasAlive)
			{
				Item deathRobe = new DeathRobe();

				if (!EquipItem(deathRobe))
				{
					deathRobe.Delete();
				}

				#region Scroll of Alacrity
				if (AcceleratedStart > DateTime.UtcNow)
				{
					BuffInfo.AddBuff(this, new BuffInfo(BuffIcon.ArcaneEmpowerment, 1078511, 1078512, AcceleratedSkill.ToString()));
				}
				#endregion
			}
		}

		private List<Item> m_EquipSnapshot;

		public List<Item> EquipSnapshot { get { return m_EquipSnapshot; } }

		private bool FindItems_Callback(Item item)
		{
			if (!item.Deleted && (item.LootType == LootType.Blessed || item.Insured))
			{
				if (Backpack != item.ParentEntity)
				{
					return true;
				}
			}
			return false;
		}

		public override bool OnBeforeDeath()
		{
			NetState state = NetState;

			if (state != null)
			{
				state.CancelAllTrades();
			}

			DropHolding();

			m_EquipSnapshot = new List<Item>(Items);

			if (m_ReceivedHonorContext != null)
			{
				m_ReceivedHonorContext.OnTargetKilled();
			}
			if (m_SentHonorContext != null)
			{
				m_SentHonorContext.OnSourceKilled();
			}

			return base.OnBeforeDeath();
		}

		public override DeathMoveResult GetParentMoveResultFor(Item item)
		{
			DeathMoveResult res = base.GetParentMoveResultFor(item);

			if (res == DeathMoveResult.MoveToCorpse && item.Movable && Young)
			{
				res = DeathMoveResult.MoveToBackpack;
			}

			return res;
		}

		public override DeathMoveResult GetInventoryMoveResultFor(Item item)
		{
			DeathMoveResult res = base.GetInventoryMoveResultFor(item);

			if (res == DeathMoveResult.MoveToCorpse && item.Movable && Young)
			{
				res = DeathMoveResult.MoveToBackpack;
			}

			return res;
		}

		public override void OnDeath(Container c)
		{
			PlayerMobile killer = null;
			Mobile m = FindMostRecentDamager(false);
			killer = m as PlayerMobile;
			if(killer == null)
			{
				if(m is BaseCreature)
				{
					killer = ((BaseCreature)m).ControlMaster as PlayerMobile;
				}
			}
			
			base.OnDeath(c);

			m_EquipSnapshot = null;

			HueMod = -1;
			NameMod = null;
			SavagePaintExpiration = TimeSpan.Zero;

			SetHairMods(-1, -1);

			PolymorphSpell.StopTimer(this);
			IncognitoSpell.StopTimer(this);
			DisguiseTimers.RemoveTimer(this);

			EndAction(typeof(PolymorphSpell));
			EndAction(typeof(IncognitoSpell));

			MeerMage.StopEffect(this, false);

			StolenItem.ReturnOnDeath(this, c);

			if (m_PermaFlags.Count > 0)
			{
				m_PermaFlags.Clear();

				if (c is Corpse)
				{
					((Corpse)c).Criminal = true;
				}

				if (Stealing.ClassicMode)
				{
					Criminal = true;
				}
			}

			if(killer != null &&
				Kills >= 5 &&
				DateTime.UtcNow >= killer.m_NextJustAward)
			{
				// This scales 700.0 skill points to 1000 valor points
				int pointsToGain = (int)(SkillsTotal / 7);
				// This scales 700.0 skill points to 7 minutes wait
				int minutesToWait = Math.Max(1, (int)(SkillsTotal / 1000));

				bool gainedPath = false;
				if (VirtueHelper.Award(m, VirtueName.Justice, pointsToGain, ref gainedPath))
				{
					if (gainedPath)
					{
						m.SendLocalizedMessage(1049367); // You have gained a path in Justice!
					}
					else
					{
						m.SendLocalizedMessage(1049363); // You have gained in Justice.
					}

					m.FixedParticles(0x375A, 9, 20, 5027, EffectLayer.Waist);
					m.PlaySound(0x1F7);

					killer.m_NextJustAward = DateTime.UtcNow + TimeSpan.FromMinutes(minutesToWait);
				}
			}

			if (Young && m_DuelContext == null)
			{
				if (YoungDeathTeleport())
				{
					Timer.DelayCall(TimeSpan.FromSeconds(2.5), SendYoungDeathNotice);
				}
			}

			if (m_DuelContext == null || !m_DuelContext.Registered || !m_DuelContext.Started || m_DuelPlayer == null ||
				m_DuelPlayer.Eliminated)
			{
				Faction.HandleDeath(this, killer);
			}

			Guilds.Guild.HandleDeath(this, killer);

			#region Dueling
			if (m_DuelContext != null)
			{
				m_DuelContext.OnDeath(this, c);
			}
			#endregion

			if (m_BuffTable != null)
			{
				var list = new List<BuffInfo>();

				foreach (BuffInfo buff in m_BuffTable.Values)
				{
					if (!buff.RetainThroughDeath)
					{
						list.Add(buff);
					}
				}

				for (int i = 0; i < list.Count; i++)
				{
					RemoveBuff(list[i]);
				}
			}
		}

		private List<Mobile> m_PermaFlags;
		private readonly List<Mobile> m_VisList;
		private readonly Hashtable m_AntiMacroTable;
		private TimeSpan m_GameTime;
		private TimeSpan m_ShortTermElapse;
		private TimeSpan m_LongTermElapse;
		private DateTime m_SessionStart;
		private DateTime m_NextSmithBulkOrder;
		private DateTime m_NextTailorBulkOrder;
		private DateTime m_SavagePaintExpiration;
		private SkillName m_Learning = (SkillName)(-1);

		public SkillName Learning { get { return m_Learning; } set { m_Learning = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan SavagePaintExpiration
		{
			get
			{
				TimeSpan ts = m_SavagePaintExpiration - DateTime.UtcNow;

				if (ts < TimeSpan.Zero)
				{
					ts = TimeSpan.Zero;
				}

				return ts;
			}
			set { m_SavagePaintExpiration = DateTime.UtcNow + value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan NextSmithBulkOrder
		{
			get
			{
				TimeSpan ts = m_NextSmithBulkOrder - DateTime.UtcNow;

				if (ts < TimeSpan.Zero)
				{
					ts = TimeSpan.Zero;
				}

				return ts;
			}
			set
			{
				try
				{
					m_NextSmithBulkOrder = DateTime.UtcNow + value;
				}
				catch
				{ }
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan NextTailorBulkOrder
		{
			get
			{
				TimeSpan ts = m_NextTailorBulkOrder - DateTime.UtcNow;

				if (ts < TimeSpan.Zero)
				{
					ts = TimeSpan.Zero;
				}

				return ts;
			}
			set
			{
				try
				{
					m_NextTailorBulkOrder = DateTime.UtcNow + value;
				}
				catch
				{ }
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastEscortTime { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastPetBallTime { get; set; }

		public PlayerMobile()
		{
			m_AutoStabled = new List<Mobile>();

			m_VisList = new List<Mobile>();
			m_PermaFlags = new List<Mobile>();
			m_AntiMacroTable = new Hashtable();
			m_RecentlyReported = new List<Mobile>();

			m_BOBFilter = new BOBFilter();

			m_GameTime = TimeSpan.Zero;
			m_ShortTermElapse = TimeSpan.FromHours(8.0);
			m_LongTermElapse = TimeSpan.FromHours(40.0);

			m_JusticeProtectors = new List<Mobile>();
			m_GuildRank = RankDefinition.Lowest;

			m_ChampionTitles = new ChampionTitleInfo();

			InvalidateMy16Below();
		}

		public override bool MutateSpeech(List<Mobile> hears, ref string text, ref object context)
		{
			if (Alive)
			{
				return false;
			}

			return base.MutateSpeech(hears, ref text, ref context);
		}

		public override void DoSpeech(string text, int[] keywords, MessageType type, int hue)
		{
			if (Guilds.Guild.NewGuildSystem && (type == MessageType.Guild || type == MessageType.Alliance))
			{
				Guild g = Guild as Guild;
				if (g == null)
				{
					SendLocalizedMessage(1063142); // You are not in a guild!
				}
				else if (type == MessageType.Alliance)
				{
					if (g.Alliance != null && g.Alliance.IsMember(g))
					{
						//g.Alliance.AllianceTextMessage( hue, "[Alliance][{0}]: {1}", this.Name, text );
						g.Alliance.AllianceChat(this, text);
						SendToStaffMessage(this, "[Alliance]: {0}", text);

						m_AllianceMessageHue = hue;
					}
					else
					{
						SendLocalizedMessage(1071020); // You are not in an alliance!
					}
				}
				else //Type == MessageType.Guild
				{
					m_GuildMessageHue = hue;

					g.GuildChat(this, text);
					SendToStaffMessage(this, "[Guild]: {0}", text);
				}
			}
			else
			{
				base.DoSpeech(text, keywords, type, hue);
			}
		}

		private static void SendToStaffMessage(Mobile from, string text)
		{
			Packet p = null;

			foreach (NetState ns in from.GetClientsInRange(8))
			{
				Mobile mob = ns.Mobile;

				if (mob != null && mob.AccessLevel >= AccessLevel.GameMaster && mob.AccessLevel > from.AccessLevel)
				{
					if (p == null)
					{
						p =
							Packet.Acquire(
								new UnicodeMessage(
									from.Serial, from.Body, MessageType.Regular, from.SpeechHue, 3, from.Language, from.Name, text));
					}

					ns.Send(p);
				}
			}

			Packet.Release(p);
		}

		private static void SendToStaffMessage(Mobile from, string format, params object[] args)
		{
			SendToStaffMessage(from, String.Format(format, args));
		}

		#region Poison
		public override ApplyPoisonResult ApplyPoison(Mobile from, Poison poison)
		{
			if (!Alive)
			{
				return ApplyPoisonResult.Immune;
			}

			ApplyPoisonResult result = base.ApplyPoison(from, poison);

			if (from != null && result == ApplyPoisonResult.Poisoned && PoisonTimer is PoisonImpl.PoisonTimer)
			{
				(PoisonTimer as PoisonImpl.PoisonTimer).From = from;
			}

			return result;
		}

		public override bool CheckPoisonImmunity(Mobile from, Poison poison)
		{
			if (Young && (DuelContext == null || !DuelContext.Started || DuelContext.Finished))
			{
				return true;
			}

			return base.CheckPoisonImmunity(from, poison);
		}

		public override void OnPoisonImmunity(Mobile from, Poison poison)
		{
			if (Young && (DuelContext == null || !DuelContext.Started || DuelContext.Finished))
			{
				SendLocalizedMessage(502808);
				// You would have been poisoned, were you not new to the land of Britannia. Be careful in the future.
			}
			else
			{
				base.OnPoisonImmunity(from, poison);
			}
		}
		#endregion

		public PlayerMobile(Serial s)
			: base(s)
		{
			m_VisList = new List<Mobile>();
			m_AntiMacroTable = new Hashtable();
			InvalidateMy16Below();
		}

		public List<Mobile> VisibilityList { get { return m_VisList; } }

		public List<Mobile> PermaFlags { get { return m_PermaFlags; } }

		public override bool IsHarmfulCriminal(Mobile target)
		{
			if (Stealing.ClassicMode && target is PlayerMobile && ((PlayerMobile)target).m_PermaFlags.Count > 0)
			{
				int noto = Notoriety.Compute(this, target);

				if (noto == Notoriety.Innocent)
				{
					target.Delta(MobileDelta.Noto);
				}

				return false;
			}

			if (target is BaseCreature && ((BaseCreature)target).InitialInnocent && !((BaseCreature)target).Controlled)
			{
				return false;
			}

			return base.IsHarmfulCriminal(target);
		}

		public bool AntiMacroCheck(Skill skill, object obj)
		{
			if (obj == null || m_AntiMacroTable == null || IsStaff())
			{
				return true;
			}

			Hashtable tbl = (Hashtable)m_AntiMacroTable[skill];
			if (tbl == null)
			{
				m_AntiMacroTable[skill] = tbl = new Hashtable();
			}

			CountAndTimeStamp count = (CountAndTimeStamp)tbl[obj];
			if (count != null)
			{
				if (count.TimeStamp + SkillCheck.AntiMacroExpire <= DateTime.UtcNow)
				{
					count.Count = 1;
					return true;
				}
				else
				{
					++count.Count;
					if (count.Count <= SkillCheck.Allowance)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			else
			{
				tbl[obj] = count = new CountAndTimeStamp();
				count.Count = 1;

				return true;
			}
		}

		private void RevertHair()
		{
			SetHairMods(-1, -1);
		}

		private BOBFilter m_BOBFilter;

		public BOBFilter BOBFilter { get { return m_BOBFilter; } }

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						m_PeacedUntil = reader.ReadDateTime();

						m_AnkhNextUse = reader.ReadDateTime();

						m_AutoStabled = reader.ReadStrongMobileList();

						int recipeCount = reader.ReadInt();

						if (recipeCount > 0)
						{
							m_AcquiredRecipes = new Dictionary<int, bool>();

							for (int i = 0; i < recipeCount; i++)
							{
								int r = reader.ReadInt();
								if (reader.ReadBool()) //Don't add in recipies which we haven't gotten or have been removed
								{
									m_AcquiredRecipes.Add(r, true);
								}
							}
						}

						m_LastHonorLoss = reader.ReadDeltaTime();

                        m_ChampionTitles = new ChampionTitleInfo(reader);

						m_LastValorLoss = reader.ReadDateTime();

						m_AllianceMessageHue = reader.ReadEncodedInt();
						m_GuildMessageHue = reader.ReadEncodedInt();

						int rank = reader.ReadEncodedInt();
						int maxRank = RankDefinition.Ranks.Length - 1;
						if (rank > maxRank)
						{
							rank = maxRank;
						}

						m_GuildRank = RankDefinition.Ranks[rank];
						m_LastOnline = reader.ReadDateTime();

						m_Profession = reader.ReadEncodedInt();

						m_LastCompassionLoss = reader.ReadDeltaTime();

						m_CompassionGains = reader.ReadEncodedInt();

						if (m_CompassionGains > 0)
						{
							m_NextCompassionDay = reader.ReadDeltaTime();
						}

						m_BOBFilter = new BOBFilter(reader);

						if (reader.ReadBool())
						{
							m_HairModID = reader.ReadInt();
							m_HairModHue = reader.ReadInt();
							m_BeardModID = reader.ReadInt();
							m_BeardModHue = reader.ReadInt();
						}

						SavagePaintExpiration = reader.ReadTimeSpan();

						if (SavagePaintExpiration > TimeSpan.Zero)
						{
							BodyMod = (Female ? 184 : 183);
							HueMod = 0;
						}

						m_NpcGuild = (NpcGuild)reader.ReadInt();
						m_NpcGuildJoinTime = reader.ReadDateTime();
						m_NpcGuildGameTime = reader.ReadTimeSpan();

						m_PermaFlags = reader.ReadStrongMobileList();

						NextTailorBulkOrder = reader.ReadTimeSpan();

						NextSmithBulkOrder = reader.ReadTimeSpan();

						m_LastJusticeLoss = reader.ReadDeltaTime();
						m_JusticeProtectors = reader.ReadStrongMobileList();

						m_LastSacrificeGain = reader.ReadDeltaTime();
						m_LastSacrificeLoss = reader.ReadDeltaTime();
						m_AvailableResurrects = reader.ReadInt();

						m_Flags = (PlayerFlag)reader.ReadInt();

						m_LongTermElapse = reader.ReadTimeSpan();
						m_ShortTermElapse = reader.ReadTimeSpan();
						m_GameTime = reader.ReadTimeSpan();

						m_AutoStabled = new List<Mobile>();

                        break;
					}
			}

			if (m_RecentlyReported == null)
			{
				m_RecentlyReported = new List<Mobile>();
			}

			// Professions weren't verified on 1.0 RC0
			if (!CharacterCreation.VerifyProfession(m_Profession))
			{
				m_Profession = 0;
			}

			if (m_PermaFlags == null)
			{
				m_PermaFlags = new List<Mobile>();
			}

			if (m_JusticeProtectors == null)
			{
				m_JusticeProtectors = new List<Mobile>();
			}

			if (m_BOBFilter == null)
			{
				m_BOBFilter = new BOBFilter();
			}

			if (m_GuildRank == null)
			{
				m_GuildRank = RankDefinition.Member;
				//Default to member if going from older version to new version (only time it should be null)
			}

			if (m_LastOnline == DateTime.MinValue && Account != null)
			{
				m_LastOnline = ((Account)Account).LastLogin;
			}

			if (m_ChampionTitles == null)
			{
				m_ChampionTitles = new ChampionTitleInfo();
			}

			var list = Stabled;

			for (int i = 0; i < list.Count; ++i)
			{
				BaseCreature bc = list[i] as BaseCreature;

				if (bc != null)
				{
					bc.IsStabled = true;
					bc.StabledBy = this;
				}
			}

			CheckAtrophies(this);

			if (Hidden) //Hiding is the only buff where it has an effect that's serialized.
			{
				AddBuff(new BuffInfo(BuffIcon.HidingAndOrStealth, 1075655));
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			//cleanup our anti-macro table
			foreach (Hashtable t in m_AntiMacroTable.Values)
			{
				ArrayList remove = new ArrayList();
				foreach (CountAndTimeStamp time in t.Values)
				{
					if (time.TimeStamp + SkillCheck.AntiMacroExpire <= DateTime.UtcNow)
					{
						remove.Add(time);
					}
				}

				for (int i = 0; i < remove.Count; ++i)
				{
					t.Remove(remove[i]);
				}
			}

			CheckKillDecay();
            CheckAtrophies(this);

			base.Serialize(writer);

			writer.Write(0); // version

			writer.Write(m_PeacedUntil);
			writer.Write(m_AnkhNextUse);
			writer.Write(m_AutoStabled, true);

			if (m_AcquiredRecipes == null)
			{
				writer.Write(0);
			}
			else
			{
				writer.Write(m_AcquiredRecipes.Count);

				foreach (var kvp in m_AcquiredRecipes)
				{
					writer.Write(kvp.Key);
					writer.Write(kvp.Value);
				}
			}

			writer.WriteDeltaTime(m_LastHonorLoss);

			ChampionTitleInfo.Serialize(writer, m_ChampionTitles);

			writer.Write(m_LastValorLoss);

			writer.WriteEncodedInt(m_AllianceMessageHue);
			writer.WriteEncodedInt(m_GuildMessageHue);

			writer.WriteEncodedInt(m_GuildRank.Rank);
			writer.Write(m_LastOnline);

			writer.WriteEncodedInt(m_Profession);

			writer.WriteDeltaTime(m_LastCompassionLoss);

			writer.WriteEncodedInt(m_CompassionGains);

			if (m_CompassionGains > 0)
			{
				writer.WriteDeltaTime(m_NextCompassionDay);
			}

			m_BOBFilter.Serialize(writer);

			bool useMods = (m_HairModID != -1 || m_BeardModID != -1);

			writer.Write(useMods);

			if (useMods)
			{
				writer.Write(m_HairModID);
				writer.Write(m_HairModHue);
				writer.Write(m_BeardModID);
				writer.Write(m_BeardModHue);
			}

			writer.Write(SavagePaintExpiration);

			writer.Write((int)m_NpcGuild);
			writer.Write(m_NpcGuildJoinTime);
			writer.Write(m_NpcGuildGameTime);

			writer.Write(m_PermaFlags, true);

			writer.Write(NextTailorBulkOrder);

			writer.Write(NextSmithBulkOrder);

			writer.WriteDeltaTime(m_LastJusticeLoss);
			writer.Write(m_JusticeProtectors, true);

			writer.WriteDeltaTime(m_LastSacrificeGain);
			writer.WriteDeltaTime(m_LastSacrificeLoss);
			writer.Write(m_AvailableResurrects);

			writer.Write((int)m_Flags);

			writer.Write(m_LongTermElapse);
			writer.Write(m_ShortTermElapse);
			writer.Write(GameTime);
		}

		public static void CheckAtrophies(Mobile m)
		{
			SacrificeVirtue.CheckAtrophy(m);
			JusticeVirtue.CheckAtrophy(m);
			CompassionVirtue.CheckAtrophy(m);
			ValorVirtue.CheckAtrophy(m);

			if (m is PlayerMobile)
			{
				ChampionTitleInfo.CheckAtrophy((PlayerMobile)m);
			}
		}

		public void CheckKillDecay()
		{
			if (m_ShortTermElapse < GameTime)
			{
				m_ShortTermElapse += TimeSpan.FromHours(8);
				if (ShortTermMurders > 0)
				{
					--ShortTermMurders;
				}
			}

			if (m_LongTermElapse < GameTime)
			{
				m_LongTermElapse += TimeSpan.FromHours(40);
				if (Kills > 0)
				{
					--Kills;
				}
			}
		}

		public void ResetKillTime()
		{
			m_ShortTermElapse = GameTime + TimeSpan.FromHours(8);
			m_LongTermElapse = GameTime + TimeSpan.FromHours(40);
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime SessionStart { get { return m_SessionStart; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan GameTime
		{
			get
			{
				if (NetState != null)
				{
					return m_GameTime + (DateTime.UtcNow - m_SessionStart);
				}
				else
				{
					return m_GameTime;
				}
			}
		}

		public override bool CanSee(Mobile m)
		{
			if (m is CharacterStatue)
			{
				((CharacterStatue)m).OnRequestedAnimation(this);
			}

			if (m is PlayerMobile && ((PlayerMobile)m).m_VisList.Contains(this))
			{
				return true;
			}

			if (m_DuelContext != null && m_DuelPlayer != null && !m_DuelContext.Finished && m_DuelContext.m_Tournament != null &&
				!m_DuelPlayer.Eliminated)
			{
				Mobile owner = m;

				if (owner is BaseCreature)
				{
					BaseCreature bc = (BaseCreature)owner;

					Mobile master = bc.GetMaster();

					if (master != null)
					{
						owner = master;
					}
				}

				if (m.IsPlayer() && owner is PlayerMobile && ((PlayerMobile)owner).DuelContext != m_DuelContext)
				{
					return false;
				}
			}

			return base.CanSee(m);
		}

		public override bool CanSee(Item item)
		{
			if (m_DesignContext != null && m_DesignContext.Foundation.IsHiddenToCustomizer(item))
			{
				return false;
			}

			return base.CanSee(item);
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			Faction faction = Faction.Find(this);

			if (faction != null)
			{
				faction.RemoveMember(this);
			}

			BaseHouse.HandleDeletion(this);

			DisguiseTimers.RemoveTimer(this);
		}

		public override bool NewGuildDisplay { get { return Guilds.Guild.NewGuildSystem; } }

		public delegate void PlayerPropertiesEventHandler(PlayerPropertiesEventArgs e);

		public static event PlayerPropertiesEventHandler PlayerProperties;

		public class PlayerPropertiesEventArgs : EventArgs
		{
			public PlayerMobile Player = null;
			public ObjectPropertyList PropertyList = null;

			public PlayerPropertiesEventArgs(PlayerMobile player, ObjectPropertyList list)
			{
				Player = player;
				PropertyList = list;
			}
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (Map == Faction.Facet)
			{
				PlayerState pl = PlayerState.Find(this);

				if (pl != null)
				{
					Faction faction = pl.Faction;

					if (faction.Commander == this)
					{
						list.Add(1042733, faction.Definition.PropName); // Commanding Lord of the ~1_FACTION_NAME~
					}
					else if (pl.Sheriff != null)
					{
						list.Add(1042734, "{0}\t{1}", pl.Sheriff.Definition.FriendlyName, faction.Definition.PropName);
						// The Sheriff of  ~1_CITY~, ~2_FACTION_NAME~
					}
					else if (pl.Finance != null)
					{
						list.Add(1042735, "{0}\t{1}", pl.Finance.Definition.FriendlyName, faction.Definition.PropName);
						// The Finance Minister of ~1_CITY~, ~2_FACTION_NAME~
					}
					else if (pl.MerchantTitle != MerchantTitle.None)
					{
						list.Add(1060776, "{0}\t{1}", MerchantTitles.GetInfo(pl.MerchantTitle).Title, faction.Definition.PropName);
						// ~1_val~, ~2_val~
					}
					else
					{
						list.Add(1060776, "{0}\t{1}", pl.Rank.Title, faction.Definition.PropName); // ~1_val~, ~2_val~
					}
				}
			}

			if (AccessLevel > AccessLevel.Player)
			{
				string color = "";
				switch (AccessLevel)
				{
					case AccessLevel.VIP:
						color = "#1EFF00";
						break;
					case AccessLevel.Counselor:
						color = "#00BFFF";
						break; //Deep Sky Blue
					case AccessLevel.Decorator:
						color = "#FF8000";
						break;
					case AccessLevel.Spawner:
						color = "#E6CC80";
						break;
					case AccessLevel.GameMaster:
						color = "#FF0000";
						break; //Red
					case AccessLevel.Seer:
						color = "#00FF00";
						break; //Green
					case AccessLevel.Administrator:
						color = "#0070FF";
						break;
					case AccessLevel.Developer:
						color = "#A335EE";
						break;
					case AccessLevel.CoOwner:
						color = "#FFD700";
						break;
					case AccessLevel.Owner:
						color = "#FFD700";
						break;
				}
				if (IsStaff())
				{
					list.Add(
						1060658, "{0}\t{1}", "Staff", String.Format("<BASEFONT COLOR={0}>{1}", color, GetAccessLevelName(AccessLevel)));
				}
				else
				{
					list.Add(1060658, "VIP");
				}
			}

			if (PlayerProperties != null)
			{
				PlayerProperties(new PlayerPropertiesEventArgs(this, list));
			}
		}

		public override void OnSingleClick(Mobile from)
		{
			if (Map == Faction.Facet)
			{
				PlayerState pl = PlayerState.Find(this);

				if (pl != null)
				{
					string text;
					bool ascii = false;

					Faction faction = pl.Faction;

					if (faction.Commander == this)
					{
						text = String.Concat(
							Female ? "(Commanding Lady of the " : "(Commanding Lord of the ", faction.Definition.FriendlyName, ")");
					}
					else if (pl.Sheriff != null)
					{
						text = String.Concat(
							"(The Sheriff of ", pl.Sheriff.Definition.FriendlyName, ", ", faction.Definition.FriendlyName, ")");
					}
					else if (pl.Finance != null)
					{
						text = String.Concat(
							"(The Finance Minister of ", pl.Finance.Definition.FriendlyName, ", ", faction.Definition.FriendlyName, ")");
					}
					else
					{
						ascii = true;

						if (pl.MerchantTitle != MerchantTitle.None)
						{
							text = String.Concat(
								"(", MerchantTitles.GetInfo(pl.MerchantTitle).Title.String, ", ", faction.Definition.FriendlyName, ")");
						}
						else
						{
							text = String.Concat("(", pl.Rank.Title.String, ", ", faction.Definition.FriendlyName, ")");
						}
					}

					int hue = (Faction.Find(from) == faction ? 98 : 38);

					PrivateOverheadMessage(MessageType.Label, hue, ascii, text, from.NetState);
				}
			}

			base.OnSingleClick(from);
		}

		public bool BedrollLogout { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public override bool Paralyzed
		{
			get { return base.Paralyzed; }
			set
			{
				base.Paralyzed = value;

				if (value)
				{
					AddBuff(new BuffInfo(BuffIcon.Paralyze, 1075827)); //Paralyze/You are frozen and can not move
				}
				else
				{
					RemoveBuff(BuffIcon.Paralyze);
				}
			}
		}

		#region Ethics
		private Player m_EthicPlayer;

		[CommandProperty(AccessLevel.GameMaster)]
		public Player EthicPlayer { get { return m_EthicPlayer; } set { m_EthicPlayer = value; } }
		#endregion

		#region Factions
		public PlayerState FactionPlayerState { get; set; }
		#endregion

		#region Dueling
		private DuelContext m_DuelContext;
		private DuelPlayer m_DuelPlayer;

		public DuelContext DuelContext { get { return m_DuelContext; } }

		public DuelPlayer DuelPlayer
		{
			get { return m_DuelPlayer; }
			set
			{
				bool wasInTourny = (m_DuelContext != null && !m_DuelContext.Finished && m_DuelContext.m_Tournament != null);

				m_DuelPlayer = value;

				if (m_DuelPlayer == null)
				{
					m_DuelContext = null;
				}
				else
				{
					m_DuelContext = m_DuelPlayer.Participant.Context;
				}

				bool isInTourny = (m_DuelContext != null && !m_DuelContext.Finished && m_DuelContext.m_Tournament != null);

				if (wasInTourny != isInTourny)
				{
					SendEverything();
				}
			}
		}
		#endregion

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Peaced
        {
            get
            {
                if (m_PeacedUntil > DateTime.UtcNow)
                {
                    return true;
                }

                return false;
            }
        }

		#region My16Below Invalidation
		private bool m_ChangedMy16Below;

		public bool ChangedMy16Below { get { return m_ChangedMy16Below; } set { m_ChangedMy16Below = value; } }

		public void InvalidateMy16Below()
		{
			if (!Deleted && !m_ChangedMy16Below)
			{
				m_ChangedMy16Below = true;
				My16Below.QueueMobileUpdate(this);
			}
		}

		public override void OnKillsChange(int oldValue)
		{
			if (Young && Kills > oldValue)
			{
				Account acc = Account as Account;

				if (acc != null)
				{
					acc.RemoveYoungStatus(0);
				}
			}

			InvalidateMy16Below();
		}

		public override void OnGenderChanged(bool oldFemale)
		{
			InvalidateMy16Below();
		}

		public override void OnGuildChange(BaseGuild oldGuild)
		{
			InvalidateMy16Below();
		}

		public override void OnGuildTitleChange(string oldTitle)
		{
			InvalidateMy16Below();
		}

		public override void OnKarmaChange(int oldValue)
		{
			InvalidateMy16Below();
		}

		public override void OnFameChange(int oldValue)
		{
			InvalidateMy16Below();
		}

		public override void OnSkillChange(SkillName skill, double oldBase)
		{
			if (Young && SkillsTotal >= 4500)
			{
				Account acc = Account as Account;

				if (acc != null)
				{
					acc.RemoveYoungStatus(1019036);
					// You have successfully obtained a respectable skill level, and have outgrown your status as a young player!
				}
			}

			InvalidateMy16Below();
		}

		public override void OnAccessLevelChanged(AccessLevel oldLevel)
		{
			if (IsPlayer())
			{
				IgnoreMobiles = false;
			}
			else
			{
				IgnoreMobiles = true;
			}

			InvalidateMy16Below();
		}

		public override void OnRawStatChange(StatType stat, int oldValue)
		{
			InvalidateMy16Below();
		}

		public override void OnDelete()
		{
			if (m_ReceivedHonorContext != null)
			{
				m_ReceivedHonorContext.Cancel();
			}
			if (m_SentHonorContext != null)
			{
				m_SentHonorContext.Cancel();
			}

			InvalidateMy16Below();
		}
		#endregion

		#region Fastwalk Prevention
		private static bool FastwalkPrevention = true; // Is fastwalk prevention enabled?

		private static int FastwalkThreshold = 400; // Fastwalk prevention will become active after 0.4 seconds

		private long m_NextMovementTime;
		private bool m_HasMoved;

		public virtual bool UsesFastwalkPrevention { get { return (IsPlayer()) & !Flying; } }

		public override int ComputeMovementSpeed(Direction dir, bool checkTurning)
		{
			if (checkTurning && (dir & Direction.Mask) != (Direction & Direction.Mask))
			{
				return RunMount; // We are NOT actually moving (just a direction change)
			}

			bool running = ((dir & Direction.Running) != 0);

			bool onHorse = (Mount != null);

			return (running ? RunFoot : WalkFoot);
		}

		public static bool MovementThrottle_Callback(NetState ns)
		{
			PlayerMobile pm = ns.Mobile as PlayerMobile;

			if (pm == null || !pm.UsesFastwalkPrevention)
			{
				return true;
			}

			if (!pm.m_HasMoved)
			{
				// has not yet moved
				pm.m_NextMovementTime = Core.TickCount;
				pm.m_HasMoved = true;
				return true;
			}

			long ts = pm.m_NextMovementTime - Core.TickCount;

			if (ts < 0)
			{
				// been a while since we've last moved
				pm.m_NextMovementTime = Core.TickCount;
				return true;
			}

			return (ts < FastwalkThreshold);
		}
		#endregion

		#region Enemy of One
		private Type m_EnemyOfOneType;

		public Type EnemyOfOneType
		{
			get { return m_EnemyOfOneType; }
			set
			{
				Type oldType = m_EnemyOfOneType;
				Type newType = value;

				if (oldType == newType)
				{
					return;
				}

				m_EnemyOfOneType = value;

				DeltaEnemies(oldType, newType);
			}
		}

		public bool WaitingForEnemy { get; set; }

		private void DeltaEnemies(Type oldType, Type newType)
		{
			foreach (Mobile m in GetMobilesInRange(18))
			{
				Type t = m.GetType();

				if (t == oldType || t == newType)
				{
					NetState ns = NetState;

					if (ns != null)
					{
						if (ns.StygianAbyss)
						{
							ns.Send(new MobileMoving(m, Notoriety.Compute(this, m)));
						}
						else
						{
							ns.Send(new MobileMovingOld(m, Notoriety.Compute(this, m)));
						}
					}
				}
			}
		}
		#endregion

		#region Hair and beard mods
		private int m_HairModID = -1, m_HairModHue;
		private int m_BeardModID = -1, m_BeardModHue;

		public void SetHairMods(int hairID, int beardID)
		{
			if (hairID == -1)
			{
				InternalRestoreHair(true, ref m_HairModID, ref m_HairModHue);
			}
			else if (hairID != -2)
			{
				InternalChangeHair(true, hairID, ref m_HairModID, ref m_HairModHue);
			}

			if (beardID == -1)
			{
				InternalRestoreHair(false, ref m_BeardModID, ref m_BeardModHue);
			}
			else if (beardID != -2)
			{
				InternalChangeHair(false, beardID, ref m_BeardModID, ref m_BeardModHue);
			}
		}

		private void CreateHair(bool hair, int id, int hue)
		{
			if (hair)
			{
				//TODO Verification?
				HairItemID = id;
				HairHue = hue;
			}
			else
			{
				FacialHairItemID = id;
				FacialHairHue = hue;
			}
		}

		private void InternalRestoreHair(bool hair, ref int id, ref int hue)
		{
			if (id == -1)
			{
				return;
			}

			if (hair)
			{
				HairItemID = 0;
			}
			else
			{
				FacialHairItemID = 0;
			}

			//if( id != 0 )
			CreateHair(hair, id, hue);

			id = -1;
			hue = 0;
		}

		private void InternalChangeHair(bool hair, int id, ref int storeID, ref int storeHue)
		{
			if (storeID == -1)
			{
				storeID = hair ? HairItemID : FacialHairItemID;
				storeHue = hair ? HairHue : FacialHairHue;
			}
			CreateHair(hair, id, 0);
		}
		#endregion

		#region Virtues
		private DateTime m_LastSacrificeGain;
		private DateTime m_LastSacrificeLoss;
		private int m_AvailableResurrects;

		public DateTime LastSacrificeGain { get { return m_LastSacrificeGain; } set { m_LastSacrificeGain = value; } }
		public DateTime LastSacrificeLoss { get { return m_LastSacrificeLoss; } set { m_LastSacrificeLoss = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int AvailableResurrects { get { return m_AvailableResurrects; } set { m_AvailableResurrects = value; } }

		private DateTime m_NextJustAward;
		private DateTime m_LastJusticeLoss;
		private List<Mobile> m_JusticeProtectors;

		public DateTime LastJusticeLoss { get { return m_LastJusticeLoss; } set { m_LastJusticeLoss = value; } }
		public List<Mobile> JusticeProtectors { get { return m_JusticeProtectors; } set { m_JusticeProtectors = value; } }

		private DateTime m_LastCompassionLoss;
		private DateTime m_NextCompassionDay;
		private int m_CompassionGains;

		public DateTime LastCompassionLoss { get { return m_LastCompassionLoss; } set { m_LastCompassionLoss = value; } }
		public DateTime NextCompassionDay { get { return m_NextCompassionDay; } set { m_NextCompassionDay = value; } }
		public int CompassionGains { get { return m_CompassionGains; } set { m_CompassionGains = value; } }

		private DateTime m_LastValorLoss;

		public DateTime LastValorLoss { get { return m_LastValorLoss; } set { m_LastValorLoss = value; } }

		private DateTime m_LastHonorLoss;
		private HonorContext m_ReceivedHonorContext;
		private HonorContext m_SentHonorContext;
		public DateTime m_hontime;

		public DateTime LastHonorLoss { get { return m_LastHonorLoss; } set { m_LastHonorLoss = value; } }

		public DateTime LastHonorUse { get; set; }

		public bool HonorActive { get; set; }

		public HonorContext ReceivedHonorContext { get { return m_ReceivedHonorContext; } set { m_ReceivedHonorContext = value; } }
		public HonorContext SentHonorContext { get { return m_SentHonorContext; } set { m_SentHonorContext = value; } }
		#endregion

		#region Young system
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Young
		{
			get { return GetFlag(PlayerFlag.Young); }
			set
			{
				SetFlag(PlayerFlag.Young, value);
				InvalidateProperties();
			}
		}

		public override string ApplyNameSuffix(string suffix)
		{
			if (Young)
			{
				if (suffix.Length == 0)
				{
					suffix = "(Young)";
				}
				else
				{
					suffix = String.Concat(suffix, " (Young)");
				}
			}

			#region Ethics
			if (m_EthicPlayer != null)
			{
				if (suffix.Length == 0)
				{
					suffix = m_EthicPlayer.Ethic.Definition.Adjunct.String;
				}
				else
				{
					suffix = String.Concat(suffix, " ", m_EthicPlayer.Ethic.Definition.Adjunct.String);
				}
			}
			#endregion

			if (Core.ML && Map == Faction.Facet)
			{
				Faction faction = Faction.Find(this);

				if (faction != null)
				{
					string adjunct = String.Format("[{0}]", faction.Definition.Abbreviation);
					if (suffix.Length == 0)
					{
						suffix = adjunct;
					}
					else
					{
						suffix = String.Concat(suffix, " ", adjunct);
					}
				}
			}

			return base.ApplyNameSuffix(suffix);
		}

		public override TimeSpan GetLogoutDelay()
		{
			if (Young || BedrollLogout || TestCenter.Enabled)
			{
				return TimeSpan.Zero;
			}

			return base.GetLogoutDelay();
		}

		private DateTime m_LastYoungMessage = DateTime.MinValue;

		public bool CheckYoungProtection(Mobile from)
		{
			if (!Young)
			{
				return false;
			}

			if (Region is BaseRegion && !((BaseRegion)Region).YoungProtected)
			{
				return false;
			}

			if (from is BaseCreature && ((BaseCreature)from).IgnoreYoungProtection)
			{
				return false;
			}

			if (DateTime.UtcNow - m_LastYoungMessage > TimeSpan.FromMinutes(1.0))
			{
				m_LastYoungMessage = DateTime.UtcNow;
				SendLocalizedMessage(1019067);
				// A monster looks at you menacingly but does not attack.  You would be under attack now if not for your status as a new citizen of Britannia.
			}

			return true;
		}

		private DateTime m_LastYoungHeal = DateTime.MinValue;

		public bool CheckYoungHealTime()
		{
			if (DateTime.UtcNow - m_LastYoungHeal > TimeSpan.FromMinutes(5.0))
			{
				m_LastYoungHeal = DateTime.UtcNow;
				return true;
			}

			return false;
		}

		private static readonly Point3D[] m_TrammelDeathDestinations = new[]
		{
			new Point3D(1481, 1612, 20), new Point3D(2708, 2153, 0), new Point3D(2249, 1230, 0), new Point3D(5197, 3994, 37),
			new Point3D(1412, 3793, 0), new Point3D(3688, 2232, 20), new Point3D(2578, 604, 0), new Point3D(4397, 1089, 0),
			new Point3D(5741, 3218, -2), new Point3D(2996, 3441, 15), new Point3D(624, 2225, 0), new Point3D(1916, 2814, 0),
			new Point3D(2929, 854, 0), new Point3D(545, 967, 0), new Point3D(3469, 2559, 36)
		};

		private static readonly Point3D[] m_IlshenarDeathDestinations = new[]
		{
			new Point3D(1216, 468, -13), new Point3D(723, 1367, -60), new Point3D(745, 725, -28), new Point3D(281, 1017, 0),
			new Point3D(986, 1011, -32), new Point3D(1175, 1287, -30), new Point3D(1533, 1341, -3), new Point3D(529, 217, -44),
			new Point3D(1722, 219, 96)
		};

		private static readonly Point3D[] m_MalasDeathDestinations = new[]
		{new Point3D(2079, 1376, -70), new Point3D(944, 519, -71)};

		private static readonly Point3D[] m_TokunoDeathDestinations = new[]
		{new Point3D(1166, 801, 27), new Point3D(782, 1228, 25), new Point3D(268, 624, 15)};

		public bool YoungDeathTeleport()
		{
			if (Region.IsPartOf(typeof(Jail)) || Region.IsPartOf("Samurai start location") ||
				Region.IsPartOf("Ninja start location") || Region.IsPartOf("Ninja cave"))
			{
				return false;
			}

			Point3D loc;
			Map map;

			DungeonRegion dungeon = (DungeonRegion)Region.GetRegion(typeof(DungeonRegion));
			if (dungeon != null && dungeon.EntranceLocation != Point3D.Zero)
			{
				loc = dungeon.EntranceLocation;
				map = dungeon.EntranceMap;
			}
			else
			{
				loc = Location;
				map = Map;
			}

			Point3D[] list;

			if (map == Map.Trammel)
			{
				list = m_TrammelDeathDestinations;
			}
			else if (map == Map.Ilshenar)
			{
				list = m_IlshenarDeathDestinations;
			}
			else if (map == Map.Malas)
			{
				list = m_MalasDeathDestinations;
			}
			else if (map == Map.Tokuno)
			{
				list = m_TokunoDeathDestinations;
			}
			else
			{
				return false;
			}

			Point3D dest = Point3D.Zero;
			int sqDistance = int.MaxValue;

			for (int i = 0; i < list.Length; i++)
			{
				Point3D curDest = list[i];

				int width = loc.X - curDest.X;
				int height = loc.Y - curDest.Y;
				int curSqDistance = width * width + height * height;

				if (curSqDistance < sqDistance)
				{
					dest = curDest;
					sqDistance = curSqDistance;
				}
			}

			MoveToWorld(dest, map);
			return true;
		}

		private void SendYoungDeathNotice()
		{
			SendGump(new YoungDeathNotice());
		}
		#endregion

		#region Speech log
		private SpeechLog m_SpeechLog;

		public SpeechLog SpeechLog { get { return m_SpeechLog; } }

		public override void OnSpeech(SpeechEventArgs e)
		{
			if (SpeechLog.Enabled && NetState != null)
			{
				if (m_SpeechLog == null)
				{
					m_SpeechLog = new SpeechLog();
				}

				m_SpeechLog.Add(e.Mobile, e.Speech);
			}
		}
		#endregion

		#region Champion Titles
		[CommandProperty(AccessLevel.GameMaster)]
		public bool DisplayChampionTitle { get { return GetFlag(PlayerFlag.DisplayChampionTitle); } set { SetFlag(PlayerFlag.DisplayChampionTitle, value); } }

		private ChampionTitleInfo m_ChampionTitles;

		[CommandProperty(AccessLevel.GameMaster)]
		public ChampionTitleInfo ChampionTitles { get { return m_ChampionTitles; } set { } }

		private void ToggleChampionTitleDisplay()
		{
			if (!CheckAlive())
			{
				return;
			}

			if (DisplayChampionTitle)
			{
				SendLocalizedMessage(1062419, "", 0x23); // You have chosen to hide your monster kill title.
			}
			else
			{
				SendLocalizedMessage(1062418, "", 0x23); // You have chosen to display your monster kill title.
			}

			DisplayChampionTitle = !DisplayChampionTitle;
		}

		[PropertyObject]
		public class ChampionTitleInfo
		{
			public static TimeSpan LossDelay = TimeSpan.FromDays(1.0);
			public const int LossAmount = 90;

			private class TitleInfo
			{
				private int m_Value;
				private DateTime m_LastDecay;

				public int Value { get { return m_Value; } set { m_Value = value; } }
				public DateTime LastDecay { get { return m_LastDecay; } set { m_LastDecay = value; } }

				public TitleInfo()
				{ }

				public TitleInfo(GenericReader reader)
				{
					int version = reader.ReadEncodedInt();

					switch (version)
					{
						case 0:
							{
								m_Value = reader.ReadEncodedInt();
								m_LastDecay = reader.ReadDateTime();
								break;
							}
					}
				}

				public static void Serialize(GenericWriter writer, TitleInfo info)
				{
					writer.WriteEncodedInt(0); // version

					writer.WriteEncodedInt(info.m_Value);
					writer.Write(info.m_LastDecay);
				}
			}

			private TitleInfo[] m_Values;

			private int m_Harrower; //Harrower titles do NOT decay

			public int GetValue(ChampionSpawnType type)
			{
				return GetValue((int)type);
			}

			public void SetValue(ChampionSpawnType type, int value)
			{
				SetValue((int)type, value);
			}

			public void Award(ChampionSpawnType type, int value)
			{
				Award((int)type, value);
			}

			public int GetValue(int index)
			{
				if (m_Values == null || index < 0 || index >= m_Values.Length)
				{
					return 0;
				}

				if (m_Values[index] == null)
				{
					m_Values[index] = new TitleInfo();
				}

				return m_Values[index].Value;
			}

			public DateTime GetLastDecay(int index)
			{
				if (m_Values == null || index < 0 || index >= m_Values.Length)
				{
					return DateTime.MinValue;
				}

				if (m_Values[index] == null)
				{
					m_Values[index] = new TitleInfo();
				}

				return m_Values[index].LastDecay;
			}

			public void SetValue(int index, int value)
			{
				if (m_Values == null)
				{
					m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];
				}

				if (value < 0)
				{
					value = 0;
				}

				if (index < 0 || index >= m_Values.Length)
				{
					return;
				}

				if (m_Values[index] == null)
				{
					m_Values[index] = new TitleInfo();
				}

				m_Values[index].Value = value;
			}

			public void Award(int index, int value)
			{
				if (m_Values == null)
				{
					m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];
				}

				if (index < 0 || index >= m_Values.Length || value <= 0)
				{
					return;
				}

				if (m_Values[index] == null)
				{
					m_Values[index] = new TitleInfo();
				}

				m_Values[index].Value += value;
			}

			public void Atrophy(int index, int value)
			{
				if (m_Values == null)
				{
					m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];
				}

				if (index < 0 || index >= m_Values.Length || value <= 0)
				{
					return;
				}

				if (m_Values[index] == null)
				{
					m_Values[index] = new TitleInfo();
				}

				int before = m_Values[index].Value;

				if ((m_Values[index].Value - value) < 0)
				{
					m_Values[index].Value = 0;
				}
				else
				{
					m_Values[index].Value -= value;
				}

				if (before != m_Values[index].Value)
				{
					m_Values[index].LastDecay = DateTime.UtcNow;
				}
			}

			public override string ToString()
			{
				return "...";
			}

			[CommandProperty(AccessLevel.GameMaster)]
			public int Abyss { get { return GetValue(ChampionSpawnType.Abyss); } set { SetValue(ChampionSpawnType.Abyss, value); } }

			[CommandProperty(AccessLevel.GameMaster)]
			public int Arachnid { get { return GetValue(ChampionSpawnType.Arachnid); } set { SetValue(ChampionSpawnType.Arachnid, value); } }

			[CommandProperty(AccessLevel.GameMaster)]
			public int ColdBlood { get { return GetValue(ChampionSpawnType.ColdBlood); } set { SetValue(ChampionSpawnType.ColdBlood, value); } }

			[CommandProperty(AccessLevel.GameMaster)]
			public int ForestLord { get { return GetValue(ChampionSpawnType.ForestLord); } set { SetValue(ChampionSpawnType.ForestLord, value); } }

			[CommandProperty(AccessLevel.GameMaster)]
			public int SleepingDragon { get { return GetValue(ChampionSpawnType.SleepingDragon); } set { SetValue(ChampionSpawnType.SleepingDragon, value); } }

			[CommandProperty(AccessLevel.GameMaster)]
			public int UnholyTerror { get { return GetValue(ChampionSpawnType.UnholyTerror); } set { SetValue(ChampionSpawnType.UnholyTerror, value); } }

			[CommandProperty(AccessLevel.GameMaster)]
			public int VerminHorde { get { return GetValue(ChampionSpawnType.VerminHorde); } set { SetValue(ChampionSpawnType.VerminHorde, value); } }

			[CommandProperty(AccessLevel.GameMaster)]
			public int Harrower { get { return m_Harrower; } set { m_Harrower = value; } }

			public ChampionTitleInfo()
			{ }

			public ChampionTitleInfo(GenericReader reader)
			{
				int version = reader.ReadEncodedInt();

				switch (version)
				{
					case 0:
						{
							m_Harrower = reader.ReadEncodedInt();

							int length = reader.ReadEncodedInt();
							m_Values = new TitleInfo[length];

							for (int i = 0; i < length; i++)
							{
								m_Values[i] = new TitleInfo(reader);
							}

							if (m_Values.Length != ChampionSpawnInfo.Table.Length)
							{
								var oldValues = m_Values;
								m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

								for (int i = 0; i < m_Values.Length && i < oldValues.Length; i++)
								{
									m_Values[i] = oldValues[i];
								}
							}
							break;
						}
				}
			}

			public static void Serialize(GenericWriter writer, ChampionTitleInfo titles)
			{
				writer.WriteEncodedInt(0); // version

				writer.WriteEncodedInt(titles.m_Harrower);

				int length = titles.m_Values.Length;
				writer.WriteEncodedInt(length);

				for (int i = 0; i < length; i++)
				{
					if (titles.m_Values[i] == null)
					{
						titles.m_Values[i] = new TitleInfo();
					}

					TitleInfo.Serialize(writer, titles.m_Values[i]);
				}
			}

			public static void CheckAtrophy(PlayerMobile pm)
			{
				ChampionTitleInfo t = pm.m_ChampionTitles;
				if (t == null)
				{
					return;
				}

				if (t.m_Values == null)
				{
					t.m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];
				}

				for (int i = 0; i < t.m_Values.Length; i++)
				{
					if ((t.GetLastDecay(i) + LossDelay) < DateTime.UtcNow)
					{
						t.Atrophy(i, LossAmount);
					}
				}
			}

			public static void AwardHarrowerTitle(PlayerMobile pm)
				//Called when killing a harrower.  Will give a minimum of 1 point.
			{
				ChampionTitleInfo t = pm.m_ChampionTitles;
				if (t == null)
				{
					return;
				}

				if (t.m_Values == null)
				{
					t.m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];
				}

				int count = 1;

				for (int i = 0; i < t.m_Values.Length; i++)
				{
					if (t.m_Values[i].Value > 900)
					{
						count++;
					}
				}

				t.m_Harrower = Math.Max(count, t.m_Harrower); //Harrower titles never decay.
			}
		}
		#endregion

		#region Recipes
		private Dictionary<int, bool> m_AcquiredRecipes;

		public virtual bool HasRecipe(Recipe r)
		{
			if (r == null)
			{
				return false;
			}

			return HasRecipe(r.ID);
		}

		public virtual bool HasRecipe(int recipeID)
		{
			if (m_AcquiredRecipes != null && m_AcquiredRecipes.ContainsKey(recipeID))
			{
				return m_AcquiredRecipes[recipeID];
			}

			return false;
		}

		public virtual void AcquireRecipe(Recipe r)
		{
			if (r != null)
			{
				AcquireRecipe(r.ID);
			}
		}

		public virtual void AcquireRecipe(int recipeID)
		{
			if (m_AcquiredRecipes == null)
			{
				m_AcquiredRecipes = new Dictionary<int, bool>();
			}

			m_AcquiredRecipes[recipeID] = true;
		}

		public virtual void ResetRecipes()
		{
			m_AcquiredRecipes = null;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int KnownRecipes
		{
			get
			{
				if (m_AcquiredRecipes == null)
				{
					return 0;
				}

				return m_AcquiredRecipes.Count;
			}
		}
		#endregion

		#region Buff Icons
		public void ResendBuffs()
		{
			if (!BuffInfo.Enabled || m_BuffTable == null)
			{
				return;
			}

			NetState state = NetState;

			if (state != null && state.BuffIcon)
			{
				foreach (BuffInfo info in m_BuffTable.Values)
				{
					state.Send(new AddBuffPacket(this, info));
				}
			}
		}

		private Dictionary<BuffIcon, BuffInfo> m_BuffTable;

		public void AddBuff(BuffInfo b)
		{
			if (!BuffInfo.Enabled || b == null)
			{
				return;
			}

			RemoveBuff(b); //Check & subsequently remove the old one.

			if (m_BuffTable == null)
			{
				m_BuffTable = new Dictionary<BuffIcon, BuffInfo>();
			}

			m_BuffTable.Add(b.ID, b);

			NetState state = NetState;

			if (state != null && state.BuffIcon)
			{
				state.Send(new AddBuffPacket(this, b));
			}
		}

		public void RemoveBuff(BuffInfo b)
		{
			if (b == null)
			{
				return;
			}

			RemoveBuff(b.ID);
		}

		public void RemoveBuff(BuffIcon b)
		{
			if (m_BuffTable == null || !m_BuffTable.ContainsKey(b))
			{
				return;
			}

			BuffInfo info = m_BuffTable[b];

			if (info.Timer != null && info.Timer.Running)
			{
				info.Timer.Stop();
			}

			m_BuffTable.Remove(b);

			NetState state = NetState;

			if (state != null && state.BuffIcon)
			{
				state.Send(new RemoveBuffPacket(this, b));
			}

			if (m_BuffTable.Count <= 0)
			{
				m_BuffTable = null;
			}
		}
		#endregion

		#region XML PVP Dismount Pet
		public void DismountAndStable()
		{
			BaseCreature bc = Mount as BaseCreature;

			if (Mount != null)
			{
				Mount.Rider = null;
			}

			if (bc != null)
			{
				bc.ControlTarget = null;
				bc.ControlOrder = OrderType.Stay;
				bc.Internalize();
				bc.SetControlMaster(null);
				bc.SummonMaster = null;
				bc.IsStabled = true;

				Stabled.Add(bc);
				m_AutoStabled.Add(bc);

				SendMessage("Your Mount has been Stabled !.");
			}
		}

		public void RetrivePet()
		{
			if (m_AutoStabled.Count < 1)
			{
				return;
			}

			for (int k = 0; k < m_AutoStabled.Count; ++k)
			{
				BaseCreature bc = (BaseCreature)m_AutoStabled[k];

				if (Stabled.Contains(bc))
				{
					bc.ControlTarget = null;
					bc.ControlOrder = OrderType.Follow;
					bc.SetControlMaster(this);
					bc.SummonMaster = null;

					if (bc.Summoned)
					{
						bc.SummonMaster = this;
					}

					bc.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully happy

					bc.MoveToWorld(Location, Map);

					bc.IsStabled = false;

					if (m_AutoStabled.Contains(bc))
					{
						m_AutoStabled.Remove(bc);
					}

					SendMessage("Your Mount return to You !.");
				}
			}
			m_AutoStabled.Clear();
		}
		#endregion

		public void AutoStablePets()
		{
			if (Core.SE && AllFollowers.Count > 0)
			{
				for (int i = m_AllFollowers.Count - 1; i >= 0; --i)
				{
					BaseCreature pet = AllFollowers[i] as BaseCreature;

					if (pet == null || pet.ControlMaster == null)
					{
						continue;
					}

					if (pet.Summoned)
					{
						if (pet.Map != Map)
						{
							pet.PlaySound(pet.GetAngerSound());
							Timer.DelayCall(TimeSpan.Zero, pet.Delete);
						}
						continue;
					}

					if (pet is IMount && ((IMount)pet).Rider != null)
					{
						continue;
					}

					if ((pet is PackLlama || pet is PackHorse || pet is Beetle) &&
						(pet.Backpack != null && pet.Backpack.Items.Count > 0))
					{
						continue;
					}

					if (pet is BaseEscortable)
					{
						continue;
					}

					pet.ControlTarget = null;
					pet.ControlOrder = OrderType.Stay;
					pet.Internalize();

					pet.SetControlMaster(null);
					pet.SummonMaster = null;

					pet.IsStabled = true;
					pet.StabledBy = this;

					pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully happy

					Stabled.Add(pet);
					m_AutoStabled.Add(pet);
				}
			}
		}

		public void ClaimAutoStabledPets()
		{
			if (!Core.SE || m_AutoStabled.Count <= 0)
			{
				return;
			}

			if (!Alive)
			{
				SendLocalizedMessage(1076251);
				// Your pet was unable to join you while you are a ghost.  Please re-login once you have ressurected to claim your pets.
				return;
			}

			for (int i = m_AutoStabled.Count - 1; i >= 0; --i)
			{
				BaseCreature pet = m_AutoStabled[i] as BaseCreature;

				if (pet == null || pet.Deleted)
				{
					pet.IsStabled = false;
					pet.StabledBy = null;

					if (Stabled.Contains(pet))
					{
						Stabled.Remove(pet);
					}

					continue;
				}

				if ((Followers + pet.ControlSlots) <= FollowersMax)
				{
					pet.SetControlMaster(this);

					if (pet.Summoned)
					{
						pet.SummonMaster = this;
					}

					pet.ControlTarget = this;
					pet.ControlOrder = OrderType.Follow;

					pet.MoveToWorld(Location, Map);

					pet.IsStabled = false;
					pet.StabledBy = null;

					pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully Happy

					if (Stabled.Contains(pet))
					{
						Stabled.Remove(pet);
					}
				}
				else
				{
					SendLocalizedMessage(1049612, pet.Name); // ~1_NAME~ remained in the stables because you have too many followers.
				}
			}

			m_AutoStabled.Clear();
		}
	}
}
