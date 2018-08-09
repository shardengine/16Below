#region Header
// **********
// 16Below - Loot.cs
// **********
#endregion

#region References
using System;

using Server.Items;
using Server.Mobiles;
#endregion

namespace Server
{
    public class Loot
    {
        #region List definitions

        private static readonly Type[] m_WeaponTypes = new[]
		{
			typeof(Axe), typeof(BattleAxe), typeof(DoubleAxe), typeof(ExecutionersAxe), typeof(Hatchet), typeof(LargeBattleAxe),
			typeof(TwoHandedAxe), typeof(WarAxe), typeof(Club), typeof(Mace), typeof(Maul), typeof(WarHammer), typeof(WarMace),
			typeof(Bardiche), typeof(Halberd), typeof(Spear), typeof(ShortSpear), typeof(Pitchfork), typeof(WarFork),
			typeof(BlackStaff), typeof(GnarledStaff), typeof(QuarterStaff), typeof(Broadsword), typeof(Cutlass), typeof(Katana),
			typeof(Kryss), typeof(Longsword), typeof(Scimitar), typeof(VikingSword), typeof(Pickaxe), typeof(HammerPick),
			typeof(ButcherKnife), typeof(Cleaver), typeof(Dagger), typeof(SkinningKnife), typeof(ShepherdsCrook)
		};

        public static Type[] WeaponTypes { get { return m_WeaponTypes; } }

        private static readonly Type[] m_ArmorTypes = new[]
		{
			typeof(BoneArms), typeof(BoneChest), typeof(BoneGloves), typeof(BoneLegs), typeof(BoneHelm), typeof(ChainChest),
			typeof(ChainLegs), typeof(ChainCoif), typeof(Bascinet), typeof(CloseHelm), typeof(Helmet), typeof(NorseHelm),
			typeof(OrcHelm), typeof(FemaleLeatherChest), typeof(LeatherArms), typeof(LeatherBustierArms), typeof(LeatherChest),
			typeof(LeatherGloves), typeof(LeatherGorget), typeof(LeatherLegs), typeof(LeatherShorts), typeof(LeatherSkirt),
			typeof(LeatherCap), typeof(FemalePlateChest), typeof(PlateArms), typeof(PlateChest), typeof(PlateGloves),
			typeof(PlateGorget), typeof(PlateHelm), typeof(PlateLegs), typeof(RingmailArms), typeof(RingmailChest),
			typeof(RingmailGloves), typeof(RingmailLegs), typeof(FemaleStuddedChest), typeof(StuddedArms),
			typeof(StuddedBustierArms), typeof(StuddedChest), typeof(StuddedGloves), typeof(StuddedGorget), typeof(StuddedLegs)
		};

        private static readonly Type[] m_RangedWeaponTypes = new[] { typeof(Bow), typeof(Crossbow), typeof(HeavyCrossbow) };

        public static Type[] RangedWeaponTypes { get { return m_RangedWeaponTypes; } }

        public static Type[] ArmorTypes { get { return m_ArmorTypes; } }

        private static readonly Type[] m_ShieldTypes = new[]
		{
			typeof(BronzeShield), typeof(Buckler), typeof(HeaterShield), typeof(MetalShield), typeof(MetalKiteShield),
			typeof(WoodenKiteShield), typeof(WoodenShield), typeof(OrderShield), typeof(ChaosShield)
		};

        public static Type[] ShieldTypes { get { return m_ShieldTypes; } }

        private static readonly Type[] m_GemTypes = new[]
		{
			typeof(Amber), typeof(Amethyst), typeof(Citrine), typeof(Diamond), typeof(Emerald), typeof(Ruby), typeof(Sapphire),
			typeof(StarSapphire), typeof(Tourmaline)
		};

        public static Type[] GemTypes { get { return m_GemTypes; } }

        private static readonly Type[] m_JewelryTypes = new[]
		{
			typeof(GoldRing), typeof(GoldBracelet), typeof(SilverRing), typeof(SilverBracelet),
		};

        public static Type[] JewelryTypes { get { return m_JewelryTypes; } }

        private static readonly Type[] m_RegTypes = new[]
		{
			typeof(BlackPearl), typeof(Bloodmoss), typeof(Garlic), typeof(Ginseng), typeof(MandrakeRoot), typeof(Nightshade),
			typeof(SulfurousAsh), typeof(SpidersSilk)
		};

        public static Type[] RegTypes { get { return m_RegTypes; } }

        private static readonly Type[] m_PotionTypes = new[]
		{
			typeof(AgilityPotion), typeof(StrengthPotion), typeof(RefreshPotion), typeof(LesserCurePotion),
			typeof(LesserHealPotion), typeof(LesserPoisonPotion)
		};

        public static Type[] PotionTypes { get { return m_PotionTypes; } }

        private static readonly Type[] m_InstrumentTypes = new[] { typeof(Drums), typeof(Harp), typeof(LapHarp), typeof(Lute), typeof(Tambourine), typeof(TambourineTassel) };

        public static Type[] InstrumentTypes { get { return m_InstrumentTypes; } }

        private static readonly Type[] m_StatueTypes = new[]
		{
			typeof(StatueSouth), typeof(StatueSouth2), typeof(StatueNorth), typeof(StatueWest), typeof(StatueEast),
			typeof(StatueEast2), typeof(StatueSouthEast), typeof(BustSouth), typeof(BustEast)
		};

        public static Type[] StatueTypes { get { return m_StatueTypes; } }

        private static readonly Type[] m_RegularScrollTypes = new[]
		{
			typeof(ReactiveArmorScroll), typeof(ClumsyScroll), typeof(CreateFoodScroll), typeof(FeeblemindScroll),
			typeof(HealScroll), typeof(MagicArrowScroll), typeof(NightSightScroll), typeof(WeakenScroll), typeof(AgilityScroll),
			typeof(CunningScroll), typeof(CureScroll), typeof(HarmScroll), typeof(MagicTrapScroll), typeof(MagicUnTrapScroll),
			typeof(ProtectionScroll), typeof(StrengthScroll), typeof(BlessScroll), typeof(FireballScroll),
			typeof(MagicLockScroll), typeof(PoisonScroll), typeof(TelekinisisScroll), typeof(TeleportScroll),
			typeof(UnlockScroll), typeof(WallOfStoneScroll), typeof(ArchCureScroll), typeof(ArchProtectionScroll),
			typeof(CurseScroll), typeof(FireFieldScroll), typeof(GreaterHealScroll), typeof(LightningScroll),
			typeof(ManaDrainScroll), typeof(RecallScroll), typeof(BladeSpiritsScroll), typeof(DispelFieldScroll),
			typeof(IncognitoScroll), typeof(MagicReflectScroll), typeof(MindBlastScroll), typeof(ParalyzeScroll),
			typeof(PoisonFieldScroll), typeof(SummonCreatureScroll), typeof(DispelScroll), typeof(EnergyBoltScroll),
			typeof(ExplosionScroll), typeof(InvisibilityScroll), typeof(MarkScroll), typeof(MassCurseScroll),
			typeof(ParalyzeFieldScroll), typeof(RevealScroll), typeof(ChainLightningScroll), typeof(EnergyFieldScroll),
			typeof(FlamestrikeScroll), typeof(GateTravelScroll), typeof(ManaVampireScroll), typeof(MassDispelScroll),
			typeof(MeteorSwarmScroll), typeof(PolymorphScroll), typeof(EarthquakeScroll), typeof(EnergyVortexScroll),
			typeof(ResurrectionScroll), typeof(SummonAirElementalScroll), typeof(SummonDaemonScroll),
			typeof(SummonEarthElementalScroll), typeof(SummonFireElementalScroll), typeof(SummonWaterElementalScroll)
		};

        public static Type[] RegularScrollTypes { get { return m_RegularScrollTypes; } }

        private static readonly Type[] m_NewWandTypes = new[]
		{
			typeof(FireballWand), typeof(LightningWand), typeof(MagicArrowWand), typeof(GreaterHealWand), typeof(HarmWand),
			typeof(HealWand)
		};

        public static Type[] NewWandTypes { get { return m_NewWandTypes; } }

        private static readonly Type[] m_WandTypes = new[] { typeof(ClumsyWand), typeof(FeebleWand), typeof(ManaDrainWand), typeof(WeaknessWand) };

        public static Type[] WandTypes { get { return m_WandTypes; } }

        private static readonly Type[] m_OldWandTypes = new[] { typeof(IDWand) };
        public static Type[] OldWandTypes { get { return m_OldWandTypes; } }

        private static readonly Type[] m_ClothingTypes = new[]
		{
			typeof(Cloak), typeof(Bonnet), typeof(Cap), typeof(FeatheredHat), typeof(FloppyHat), typeof(JesterHat),
			typeof(Surcoat), typeof(SkullCap), typeof(StrawHat), typeof(TallStrawHat), typeof(TricorneHat), typeof(WideBrimHat),
			typeof(WizardsHat), typeof(BodySash), typeof(Doublet), typeof(Boots), typeof(FullApron), typeof(JesterSuit),
			typeof(Sandals), typeof(Tunic), typeof(Shoes), typeof(Shirt), typeof(Kilt), typeof(Skirt), typeof(FancyShirt),
			typeof(FancyDress), typeof(ThighBoots), typeof(LongPants), typeof(PlainDress), typeof(Robe), typeof(ShortPants),
			typeof(HalfApron)
		};

        public static Type[] ClothingTypes { get { return m_ClothingTypes; } }

        private static readonly Type[] m_HatTypes = new[]
		{
			typeof(SkullCap), typeof(Bandana), typeof(FloppyHat), typeof(Cap), typeof(WideBrimHat), typeof(StrawHat),
			typeof(TallStrawHat), typeof(WizardsHat), typeof(Bonnet), typeof(FeatheredHat), typeof(TricorneHat),
			typeof(JesterHat)
		};

        public static Type[] HatTypes { get { return m_HatTypes; } }

        private static readonly Type[] m_LibraryBookTypes = new[]
		{
			typeof(GrammarOfOrcish), typeof(CallToAnarchy), typeof(ArmsAndWeaponsPrimer), typeof(SongOfSamlethe),
			typeof(TaleOfThreeTribes), typeof(GuideToGuilds), typeof(BirdsOfBritannia), typeof(BritannianFlora),
			typeof(ChildrenTalesVol2), typeof(TalesOfVesperVol1), typeof(DeceitDungeonOfHorror), typeof(DimensionalTravel),
			typeof(EthicalHedonism), typeof(MyStory), typeof(DiversityOfOurLand), typeof(QuestOfVirtues), typeof(RegardingLlamas)
			, typeof(TalkingToWisps), typeof(TamingDragons), typeof(BoldStranger), typeof(BurningOfTrinsic), typeof(TheFight),
			typeof(LifeOfATravellingMinstrel), typeof(MajorTradeAssociation), typeof(RankingsOfTrades),
			typeof(WildGirlOfTheForest), typeof(TreatiseOnAlchemy), typeof(VirtueBook)
		};

        public static Type[] LibraryBookTypes { get { return m_LibraryBookTypes; } }
        #endregion

        #region Accessors
        public static BaseWand RandomWand()
        {
            if (Core.ML)
            {
                return Construct(m_NewWandTypes) as BaseWand;
            }

            if (Core.AOS)
            {
                return Construct(m_WandTypes, m_NewWandTypes) as BaseWand;
            }

            return Construct(m_OldWandTypes, m_WandTypes, m_NewWandTypes) as BaseWand;
        }

        public static BaseClothing RandomClothing()
        {

            return Construct(m_ClothingTypes) as BaseClothing;
        }

        public static BaseWeapon RandomRangedWeapon()
        {

            return Construct(m_RangedWeaponTypes) as BaseWeapon;
        }

        public static BaseWeapon RandomWeapon()
        {

            return Construct(m_WeaponTypes) as BaseWeapon;
        }

        public static Item RandomWeaponOrJewelry()
        {
            return Construct(m_WeaponTypes, m_JewelryTypes);
        }

        public static BaseJewel RandomJewelry()
        {
            return Construct(m_JewelryTypes) as BaseJewel;
        }

        public static BaseArmor RandomArmor()
        {
            return Construct(m_ArmorTypes) as BaseArmor;
        }

        public static BaseHat RandomHat()
        {
            return Construct(m_HatTypes) as BaseHat;
        }

        public static Item RandomArmorOrHat()
        {
            return Construct(m_ArmorTypes, m_HatTypes);
        }

        public static BaseShield RandomShield()
        {
            return Construct(m_ShieldTypes) as BaseShield;
        }

        public static BaseArmor RandomArmorOrShield()
        {
            return Construct(m_ArmorTypes, m_ShieldTypes) as BaseArmor;
        }

        public static Item RandomArmorOrShieldOrJewelry()
        {
            return Construct(m_ArmorTypes, m_HatTypes, m_ShieldTypes, m_JewelryTypes);
        }

        public static Item RandomArmorOrShieldOrWeapon()
        {
            return Construct(m_WeaponTypes, m_RangedWeaponTypes, m_ArmorTypes, m_HatTypes, m_ShieldTypes);
        }

        public static Item RandomArmorOrShieldOrWeaponOrJewelry()
        {
            return Construct(m_WeaponTypes, m_RangedWeaponTypes, m_ArmorTypes, m_HatTypes, m_ShieldTypes, m_JewelryTypes);
        }

        public static Item RandomGem()
        {
            return Construct(m_GemTypes);
        }

        public static Item RandomReagent()
        {
            return Construct(m_RegTypes);
        }

        public static Item RandomPotion()
        {
            return Construct(m_PotionTypes);
        }

        public static BaseInstrument RandomInstrument()
        {
            return Construct(m_InstrumentTypes) as BaseInstrument;
        }

        public static Item RandomStatue()
        {
            return Construct(m_StatueTypes);
        }

        public static SpellScroll RandomScroll(int minIndex, int maxIndex)
        {
            return Construct(m_RegularScrollTypes, Utility.RandomMinMax(minIndex, maxIndex)) as SpellScroll;
        }

        public static BaseBook RandomLibraryBook()
        {
            return Construct(m_LibraryBookTypes) as BaseBook;
        }

        #endregion

        #region Construction methods
        public static Item Construct(Type type)
        {
            Item item;
            try
            {
                item = Activator.CreateInstance(type) as Item;
            }
            catch
            {
                return null;
            }

            return item;
        }

        public static Item Construct(Type[] types)
        {
            if (types.Length > 0)
            {
                return Construct(types, Utility.Random(types.Length));
            }

            return null;
        }

        public static Item Construct(Type[] types, int index)
        {
            if (index >= 0 && index < types.Length)
            {
                return Construct(types[index]);
            }

            return null;
        }

        public static Item Construct(params Type[][] types)
        {
            int totalLength = 0;

            for (int i = 0; i < types.Length; ++i)
            {
                totalLength += types[i].Length;
            }

            if (totalLength > 0)
            {
                int index = Utility.Random(totalLength);

                for (int i = 0; i < types.Length; ++i)
                {
                    if (index >= 0 && index < types[i].Length)
                    {
                        return Construct(types[i][index]);
                    }

                    index -= types[i].Length;
                }
            }

            return null;
        }
        #endregion
    }
}