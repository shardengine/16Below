using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Accounting;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Server.Engines.Quests.Haven;

namespace Server.Gumps
{
    public class WorldCreator : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("WorldCreator", AccessLevel.GameMaster, new CommandEventHandler(WorldCreator_OnCommand));
            CommandSystem.Register("WC", AccessLevel.Administrator, new CommandEventHandler(WorldCreator_OnCommand));
        }

        [Usage("WorldCreator")]
        [Aliases("WC")]
        [Description("Brings up the World Creator Menu")]
        private static void WorldCreator_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.CloseGump(typeof(WorldCreator));
            from.SendGump(new WorldCreator(from));
        }

        public WorldCreator(Mobile from)
            : base(0, 0)
        {
            NetState ns = from.NetState;
            if (ns == null)
            {
                return;
            }
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(1);
            AddBackground(110, 38, 600, 509, 9200);

         //   if (!ns.IsKRClient)
         //   {
                AddImageTiled(124, 52, 568, 479, 2624);
         //   }

            AddAlphaRegion(125, 52, 567, 480);
            AddLabel(347, 66, 2728, @"World Creator");
            AddLabel(555, 509, 2728, @"@ShardEngine, 2016");
            AddImage(297, 58, 52);
            AddImage(125, 51, 5609);
            AddImage(632, 51, 5609);
            AddButton(141, 176, 4005, 4007, 0, GumpButtonType.Page, 2);
            AddButton(141, 206, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddButton(141, 235, 4005, 4007, 0, GumpButtonType.Page, 3);
            AddButton(141, 264, 4005, 4007, 4, GumpButtonType.Reply, 0);
            AddLabel(178, 178, 2728, @"Configuration Management");
            AddLabel(178, 209, 2728, @"Decoration");
            AddLabel(178, 237, 2728, @"Spawns");
            AddLabel(178, 265, 2728, @"Tools");

            AddPage(2);
            AddBackground(110, 38, 600, 509, 9200);
         //   if (!ns.IsKRClient)
         //   {
                AddImageTiled(124, 52, 568, 479, 2624);
         //   }
            //AddImageTiled(124, 52, 568, 479, 2624);
            AddAlphaRegion(125, 52, 567, 480);
            AddLabel(347, 66, 2728, @"World Creator");
            AddLabel(555, 509, 2728, @"@ShardEngine, 2016");
            AddLabel(307, 125, 2728, @"Configuration Management");
            AddImage(297, 58, 52);
            AddImage(125, 51, 5609);
            AddImage(632, 51, 5609);
            AddButton(141, 178, 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddButton(141, 205, 4005, 4007, 5, GumpButtonType.Reply, 0);
            AddButton(141, 233, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddLabel(180, 179, 2728, @"T2A Settings");
            AddLabel(180, 207, 2728, @"ML Settings");
            AddLabel(180, 233, 2728, @"Current UO Settings");
            AddButton(131, 509, 2466, 2468, 0, GumpButtonType.Page, 1);//back

            AddPage(3);
            AddBackground(110, 38, 600, 509, 9200);
        //    if (!ns.IsKRClient)
        //    {
                AddImageTiled(124, 52, 568, 479, 2624);
        //    }
            //AddImageTiled(124, 52, 568, 479, 2624);
            AddAlphaRegion(125, 52, 567, 480);
            AddLabel(347, 66, 2728, @"World Creator");
            AddLabel(555, 509, 2728, @"@ShardEngine, 2016");
            AddLabel(307, 125, 2728, @"Spawn");
            AddImage(297, 58, 52);
            AddImage(125, 51, 5609);
            AddImage(632, 51, 5609);
            AddButton(141, 180, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddButton(141, 206, 4005, 4007, 6, GumpButtonType.Reply, 0);
            AddButton(141, 232, 4005, 4007, 7, GumpButtonType.Reply, 0);
            AddButton(141, 258, 4005, 4007, 8, GumpButtonType.Reply, 0);
            AddButton(141, 284, 4005, 4007, 9, GumpButtonType.Reply, 0);
            AddButton(141, 310, 4005, 4007, 10, GumpButtonType.Reply, 0);
            AddButton(141, 336, 4005, 4007, 11, GumpButtonType.Reply, 0);
            AddButton(141, 358, 4005, 4007, 12, GumpButtonType.Reply, 0);
            AddButton(141, 380, 4005, 4007, 13, GumpButtonType.Reply, 0);
            AddLabel(174, 181, 2728, @"T2A Spawn");
            AddLabel(174, 208, 2728, @"Felucca Spawn");
            AddLabel(174, 235, 2728, @"Trammel Spawn");
            AddLabel(174, 260, 2728, @"Ilshenar Spawn");
            AddLabel(174, 286, 2728, @"Malas Spawn");
            AddLabel(174, 312, 2728, @"Tokuno Spawn");
            AddLabel(174, 338, 2728, @"TerMur Spawn");
            AddLabel(174, 359, 2728, @"Champion Spawn");
            AddLabel(174, 381, 2728, @"Gauntlet Spawn");
            AddButton(131, 509, 2466, 2468, 0, GumpButtonType.Page, 1);//back
        }

        public static void DoThis(Mobile from, string command)
        {
            string prefix = Server.Commands.CommandSystem.Prefix;
            CommandSystem.Handle(from, String.Format("{0}{1}", prefix, command));
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            switch (info.ButtonID)
            {
                case 0:
                    {
                        from.CloseGump(typeof(WorldCreator));
                        break;
                    }
                case 1:
                    {
                        from.CloseGump(typeof(WorldCreator));
                    //    from.SendGump(new WorldCreatorDecor(from));
                        break;
                    }
                case 2:
                    {
                        from.CloseGump(typeof(WorldCreator));
                    //    from.SendGump(new T2ASpawner(from));
                        break;
                    }
                case 3:
                    {
                        DoThis(from, "SettingsT2A");
                        break;
                    }
                case 4:
                    {
                        from.CloseGump(typeof(WorldCreator));
                    //    from.SendGump(new WorldCreatorTools(from));
                        break;
                    }
                case 5:
                    {
                        DoThis(from, "SettingsML");
                        break;
                    }
                case 6:
                    {
                        from.CloseGump(typeof(WorldCreator));
                    //    from.SendGump(new WorldCreatorFelSpawn(from));
                        break;
                    }
                case 7:
                    {
                        from.CloseGump(typeof(WorldCreator));
                    //    from.SendGump(new WorldCreatorTramSpawn(from));
                        break;
                    }
                case 8:
                    {
                        from.CloseGump(typeof(WorldCreator));
                    //    from.SendGump(new WorldCreatorIlshenarSpawn(from));
                        break;
                    }
                case 9:
                    {
                        from.CloseGump(typeof(WorldCreator));
                    //    from.SendGump(new WorldCreatorMalasSpawn(from));
                        break;
                    }
                case 10:
                    {
                        from.CloseGump(typeof(WorldCreator));
                    //    from.SendGump(new WorldCreatorTokunoSpawn(from));
                        break;
                    }
                case 11:
                    {
                        from.CloseGump(typeof(WorldCreator));
                    //    from.SendGump(new WorldCreatorTerMurSpawn(from));
                        break;
                    }
                case 12:
                    {
                        DoThis(from, "GenChampions");
                        break;
                    }
                case 13:
                    {
                        DoThis(from, "GenGauntlet");
                        break;
                    }
            }
        }
    }
}