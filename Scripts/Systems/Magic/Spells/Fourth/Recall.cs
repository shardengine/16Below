using System;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Fourth
{
    public class RecallSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Recall", "Kal Ort Por",
            239,
            9031,
            Reagent.BlackPearl,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot);

        public RecallSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Fourth;
            }
        }

        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public override bool CheckCast()
        {
            if (Factions.Sigil.ExistsOn(this.Caster))
            {
                this.Caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
                return false;
            }
            else if (this.Caster.Criminal)
            {
                this.Caster.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
                return false;
            }
            else if (SpellHelper.CheckCombat(this.Caster))
            {
                this.Caster.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                return false;
            }
            else if (Server.Misc.WeightOverloading.IsOverloaded(this.Caster))
            {
                this.Caster.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
                return false;
            }

            return SpellHelper.CheckTravel(this.Caster, TravelCheckType.RecallFrom);
        }

        public void Effect(Point3D loc, Map map, bool checkMulti)
        {
            if (Factions.Sigil.ExistsOn(this.Caster))
            {
                this.Caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
            }
            else if (map == null || (!Core.AOS && this.Caster.Map != map))
            {
                this.Caster.SendLocalizedMessage(1005569); // You can not recall to another facet.
            }
            else if (!SpellHelper.CheckTravel(this.Caster, TravelCheckType.RecallFrom))
            {
            }
            else if (!SpellHelper.CheckTravel(this.Caster, map, loc, TravelCheckType.RecallTo))
            {
            }
            else if (map == Map.Felucca && this.Caster is PlayerMobile && ((PlayerMobile)this.Caster).Young)
            {
                this.Caster.SendLocalizedMessage(1049543); // You decide against traveling to Felucca while you are still young.
            }
            else if (this.Caster.Kills >= 5 && map != Map.Felucca)
            {
                this.Caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.
            }
            else if (this.Caster.Criminal)
            {
                this.Caster.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
            }
            else if (SpellHelper.CheckCombat(this.Caster))
            {
                this.Caster.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
            }
            else if (Server.Misc.WeightOverloading.IsOverloaded(this.Caster))
            {
                this.Caster.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
            }
            else if (!map.CanSpawnMobile(loc.X, loc.Y, loc.Z))
            {
                this.Caster.SendLocalizedMessage(501942); // That location is blocked.
            }
            else if ((checkMulti && SpellHelper.CheckMulti(loc, map)))
            {
                this.Caster.SendLocalizedMessage(501942); // That location is blocked.
            }
            else if (this.Caster.Holding != null)
            {
                this.Caster.SendLocalizedMessage(1071955); // You cannot teleport while dragging an object.
            }
            else if (this.CheckSequence())
            {
                BaseCreature.TeleportPets(this.Caster, loc, map, true);

                this.Caster.PlaySound(0x1FC);
                this.Caster.MoveToWorld(loc, map);
                this.Caster.PlaySound(0x1FC);
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly RecallSpell m_Owner;
            public InternalTarget(RecallSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.None)
            {
                this.m_Owner = owner;

                owner.Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 501029); // Select Marked item.
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is RecallRune)
                {
                    RecallRune rune = (RecallRune)o;

                    if (rune.Marked)
                        this.m_Owner.Effect(rune.Target, rune.TargetMap, true);
                    else
                        from.SendLocalizedMessage(501805); // That rune is not yet marked.
                }
                else if (o is Key && ((Key)o).KeyValue != 0 && ((Key)o).Link is BaseBoat)
                {
                    BaseBoat boat = ((Key)o).Link as BaseBoat;

                    if (!boat.Deleted && boat.CheckKey(((Key)o).KeyValue))
                        this.m_Owner.Effect(boat.GetMarkedLocation(), boat.Map, false);
                    else
                        from.Send(new MessageLocalized(from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "")); // I can not recall from that object.
                }
                else if (o is HouseRaffleDeed && ((HouseRaffleDeed)o).ValidLocation())
                {
                    HouseRaffleDeed deed = (HouseRaffleDeed)o;

                    this.m_Owner.Effect(deed.PlotLocation, deed.PlotFacet, true);
                }
                else
                {
                    from.Send(new MessageLocalized(from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "")); // I can not recall from that object.
                }
            }

            protected override void OnNonlocalTarget(Mobile from, object o)
            {
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}