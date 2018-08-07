using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;

namespace Server.Mobiles
{
    public class TinkerGuildmaster : BaseGuildmaster
    {
        [Constructable]
        public TinkerGuildmaster()
            : base("tinker")
        {
            this.SetSkill(SkillName.Lockpicking, 65.0, 88.0);
            this.SetSkill(SkillName.Tinkering, 90.0, 100.0);
            this.SetSkill(SkillName.RemoveTrap, 85.0, 100.0);
        }

        public TinkerGuildmaster(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild
        {
            get
            {
                return NpcGuild.TinkersGuild;
            }
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