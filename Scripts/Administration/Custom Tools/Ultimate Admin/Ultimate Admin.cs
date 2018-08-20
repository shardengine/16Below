#region References
using System;
using Server.Accounting;
using Server.Commands;
using Server.Engines.Craft;
using Server.Engines.Harvest;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using System.Collections.Generic;
using System.IO;

#endregion

namespace Server.Gumps
{
    public class UltimateAdmin : Gump
    {
        // Hold overs defined here

        public static void Initialize()
        {
        //    EventSink.OnEnterRegion += OnEnterRegion;// use for region edits?? no prob not
            EventSink.Login += OnLogin;
            CommandSystem.Register("UltimateAdmin", AccessLevel.Administrator, UltimateAdmin_OnCommand);
            CommandSystem.Register("UA", AccessLevel.Administrator, new CommandEventHandler(UltimateAdmin_OnCommand));
        }

        [Usage("UltimateAdmin")]
        [Aliases("UA")]
        [Description("Brings up the Ultimate Admin Menu")]
        private static void UltimateAdmin_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (from != null)
            {
                from.CloseGump(typeof(UltimateAdmin));
                from.SendGump(new UltimateAdmin(from));
            }
        }

        static void OnEnterRegion(OnEnterRegionEventArgs args)
        {
            Mobile from = args.From;
            if (args.Region != null) from.SendMessage("Enter Region {0}", args.Region.Name);
            else from.SendMessage("NULL REGION");

       //     from.CloseGump(typeof(UltimateAdmin));
       //     from.CloseGump(typeof(UltimateAdminMini));
       //     from.SendGump(new UltimateAdminMini(from));
        }

        static void OnLogin(LoginEventArgs args)
        {
            Mobile from = args.Mobile;
            if (from == null) return;
            if (from.AccessLevel < AccessLevel.GameMaster) return;

            from.CloseGump(typeof(UltimateAdmin));
            from.CloseGump(typeof(UltimateAdminMini));
            from.SendGump(new UltimateAdmin(from));
        }

        static string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        static string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        static string Label(string text, string value)
        {
            return String.Format("{0}: {1}", text, value);
        }

        private int GetGoldOwned()
        {
            int totalgold = 0;
            double totalbalance = 0;

            int gold; double balance;

            foreach (Account a in Accounts.GetAccounts())
            {
                a.GetGoldBalance(out gold, out balance);
                totalgold += gold;
                totalbalance += balance;

            }           
            return totalgold;
        }

        static int face1 = 0x110F; // 7
        static int face2 = 0x10FC; // 7

        static int globe = 0x3660;

        static int facecount = 0;
        static int globecount = 0;

        private void DrawFaces(Mobile from)
        {


        }

        public UltimateAdmin(Mobile from) : base(0, 0)
        {
            if (from == null) return;

            NetState ns = from.NetState;
            if (ns == null) return;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);
            AddBackground(50, 10, 600, 400, 9270);
            AddAlphaRegion(60, 20, 580, 380);

            AddImage(0, 0, 10440, 2424);// dragon1
            AddImage(620, 0, 10441, 2424);// dragon2
            
            AddImage(610, 370, 0x71, 2424);// fraz pentagram hue 2424

            AddItem(60, 20, face1 + facecount);// face1 0x110F
            AddItem(590, 20, face2 + facecount);// face20x10FC

            AddHtml(100, 25, 505, 18, String.Format("<basefont color = #DD00DD><center>{0}</center></basefont>", "ULTIMATE ADMINISTRATOR"), false, false);

            AddPage(1);

            AddItem(140, 45, globe + globecount);
            AddItem(505, 45, globe + globecount);

            // AddImage(500, 40, 0x2328, 2424);// uo symbol lame..

            if (from.Region != null && from.Region.Parent != null)
            {
                AddHtml(230, 45, 240, 18, String.Format("<basefont color = #FFFFFF><center><i>{0}</i></center></basefont>", from.Region.Parent.Name), false, false);
                AddHtml(230, 63, 240, 18, String.Format("<basefont color = #FFFFFF><center><i>{0}</i></center></basefont>", from.Region.Name), false, false);
                AddHtml(230, 81, 240, 18, String.Format("<basefont color = #FFFFFF><center><i>{0} {1} {2} {3}</i></center></basefont>", from.Location.X, from.Location.Y, from.Location.Z, from.Map.Name), false, false);
                AddHtml(230, 99, 240, 18, String.Format("<basefont color = #FFFFFF><center><i>{0}</i></center></basefont>", from.Region.GetType().Name), false, false);
            }
            else if (from.Region != null)
            {
                AddHtml(230, 45, 240, 18, String.Format("<basefont color = #FFFFFF><center><i>{0}</i></center></basefont>", from.Region.Name), false, false);
                AddHtml(230, 63, 240, 18, String.Format("<basefont color = #FFFFFF><center><i>{0} {1} {2} {3}</i></center></basefont>", from.Location.X, from.Location.Y, from.Location.Z, from.Map.Name), false, false);
                AddHtml(230, 81, 240, 18, String.Format("<basefont color = #FFFFFF><center><i>{0}</i></center></basefont>", from.Region.GetType().Name), false, false);
            }
           
            AddHtml(90, 105, 140, 18, String.Format("<basefont color = #FFFF00><center><i>{0}</i></center></basefont>", "Real Time"), false, false);
            AddHtml(455, 105, 140, 18, String.Format("<basefont color = #FFFF00><center><i>{0}</i></center></basefont>", "Game Time"), false, false); 
                               
            AddHtml(80, 126, 180, 18, String.Format("<basefont color = #FFFFFF><center><i>{0}</i></center></basefont>", DateTime.Now), false, false);
       //     AddHtml(420, 126, 180, 18, String.Format("<basefont color = #FFFFFF><center><i>{0}</i></center></basefont>", Time.Time.FormatBritainTime()), false, false);

            /*
            if (!Time.Time.UseRealTime)
            {
             //   AddButton(158, 23, globe, globe, 1, GumpButtonType.Reply, 0);//9705, 9705
                AddImage(523, 23, 9705);
            }
            else
            {
            //    AddButton(523, 23, globe, globe, 2, GumpButtonType.Reply, 0);//9705, 9705
                AddImage(158, 23, 9705);
            }  
            */

            AddButton(87, 163, 4005, 4007, 1, GumpButtonType.Reply, 0);//1209, 1210

            AddHtml(125, 164, 90, 18, String.Format("<basefont color = #FFFFFF><left>{0}:</left></basefont>", "Map Season"), false, false);

            switch (from.Map.Season)
            {
                case 0:
                    AddHtml(205, 164, 80, 18, String.Format("<basefont color = #00FF00><left><i>{0}</i></left></basefont>", "Spring"), false, false);
                    break;
                case 1:
                    AddHtml(205, 164, 80, 18, String.Format("<basefont color = #00DD00><left><i>{0}</i></left></basefont>", "Summer"), false, false);
                    break;
                case 2:
                    AddHtml(205, 164, 80, 18, String.Format("<basefont color = #CC6600><left><i>{0}</i></left></basefont>", "Autumn"), false, false);
                    break;
                case 3:
                    AddHtml(205, 164, 80, 18, String.Format("<basefont color = #FFFFFF><left><i>{0}</i></left></basefont>", "Winter"), false, false);
                    break;
                case 4:
                    AddHtml(205, 164, 80, 18, String.Format("<basefont color = #AAAAAA><left><i>{0}</i></left></basefont>", "Desolation"), false, false);
                    break;
            }

            AddButton(393, 163, 4005, 4007, 2, GumpButtonType.Reply, 0);//1209, 1210

            AddHtml(430, 164, 100, 18, String.Format("<basefont color = #FFFFFF><left>{0}:</left></basefont>", "Region Season"), false, false);

            BaseRegion br = null;

            if (from.Region != null)
            {
                br = from.Region as BaseRegion;
            }

            /*
            if (br != null && br.Season > -1)
            {
                switch (from.GetSeason())
                {
                    case 0:
                        AddHtml(530, 164, 70, 18, String.Format("<basefont color = #00FF00><left><i>{0}</i></left></basefont>", "Spring"), false, false);
                        break;
                    case 1:
                        AddHtml(530, 164, 70, 18, String.Format("<basefont color = #00DD00><left><i>{0}</i></left></basefont>", "Summer"), false, false);
                        break;
                    case 2:
                        AddHtml(530, 164, 70, 18, String.Format("<basefont color = #CC6600><left><i>{0}</i></left></basefont>", "Autumn"), false, false);
                        break;
                    case 3:
                        AddHtml(530, 164, 70, 18, String.Format("<basefont color = #FFFFFF><left><i>{0}</i></left></basefont>", "Winter"), false, false);
                        break;
                    case 4:
                        AddHtml(530, 164, 70, 18, String.Format("<basefont color = #AAAAAA><left><i>{0}</i></left></basefont>", "Desolation"), false, false);
                        break;
                    default:
                        AddHtml(530, 164, 70, 18, String.Format("<basefont color = #AAAAAA><left><i>{0}</i></left></basefont>", "Default"), false, false);
                        break;
                }
            } 
            else
            {
            */
                switch (from.Map.Season)
                {
                    case 0:
                        AddHtml(530, 164, 70, 18, String.Format("<basefont color = #00FF00><left><i>{0}</i></left></basefont>", "Spring"), false, false);
                        break;
                    case 1:
                        AddHtml(530, 164, 70, 18, String.Format("<basefont color = #00DD00><left><i>{0}</i></left></basefont>", "Summer"), false, false);
                        break;
                    case 2:
                        AddHtml(530, 164, 70, 18, String.Format("<basefont color = #CC6600><left><i>{0}</i></left></basefont>", "Autumn"), false, false);
                        break;
                    case 3:
                        AddHtml(530, 164, 70, 18, String.Format("<basefont color = #FFFFFF><left><i>{0}</i></left></basefont>", "Winter"), false, false);
                        break;
                    case 4:
                        AddHtml(530, 164, 70, 18, String.Format("<basefont color = #AAAAAA><left><i>{0}</i></left></basefont>", "Desolation"), false, false);
                        break;
                    default:
                        AddHtml(530, 164, 70, 18, String.Format("<basefont color = #AAAAAA><left><i>{0}</i></left></basefont>", "None"), false, false);
                        break;
                }
        //    }

            // RIGHT SIDE LIGHT LEVELS!

            /*
            if (br != null && br.Weather == null)
            {
                br = from.Region.Parent as BaseRegion;
            }

            AddButton(393, 193, 4005, 4007, 3, GumpButtonType.Reply, 0); // 1209, 1210

            AddHtml(430, 194, 100, 18, String.Format("<basefont color = #FFFFFF><left>{0}:</left></basefont>", "Region Weather"), false, false);

            if (br != null && br.Weather != null)
            {
                switch((int)br.Weather.Type)
                {
                    case 0:
                        AddHtml(532, 194, 68, 18, String.Format("<basefont color = #0000FF><left><i>{0}</i></left></basefont>", "Rain"), false, false);
                        break;
                    case 1:
                        AddHtml(532, 194, 68, 18, String.Format("<basefont color = #A0A0A0><left><i>{0}</i></left></basefont>", "Storm"), false, false);
                        break;
                    case 2:
                        AddHtml(532, 194, 68, 18, String.Format("<basefont color = #FFFFFF><left><i>{0}</i></left></basefont>", "Snow"), false, false);
                        break;
                    case 3:
                        AddHtml(532, 194, 68, 18, String.Format("<basefont color = #FFFFFF><left><i>{0}</i></left></basefont>", "Fierce"), false, false);
                        break;
                    case 255:
                        AddHtml(532, 194, 68, 18, String.Format("<basefont color = #66B2FF><left><i>{0}</i></left></basefont>", "Clear Sky"), false, false);
                        break;
                }

                AddHtml(430, 224, 170, 18, String.Format("<basefont color = #FFFFFF><right>Temperature:    {0}</right></basefont>", br.Weather.Temperature), false, false);

                int density = br.Weather.Density;
                if (density == 255) density = 0;

                AddHtml(430, 244, 170, 18, String.Format("<basefont color = #FFFFFF><right>Humidity:       {0}</right></basefont>", density), false, false);

            }
            else AddHtml(532, 194, 68, 18, String.Format("<basefont color = #AAAAAA><left><i>{0}</i></left></basefont>", "None"), false, false);
            */

            AddButton(87, 163, 4005, 4007, 1, GumpButtonType.Reply, 0);//1209, 1210

            AddHtml(125, 164, 90, 18, String.Format("<basefont color = #FFFFFF><left>{0}:</left></basefont>", "Map Season"), false, false);

            AddButton(87, 274, 4005, 4007, 3, GumpButtonType.Reply, 0); // 1209, 1210
            AddHtml(125, 275, 170, 18, String.Format("<basefont color = #FFFFFF><right>Global Light:    {0}</right></basefont>", LightCycle.LevelOverride), false, false);


            AddButton(87, 304, 4005, 4007, 3, GumpButtonType.Reply, 0); // 1209, 1210
            AddHtml(125, 305, 170, 18, String.Format("<basefont color = #FFFFFF><right>Personal Light:  {0}</right></basefont>", LightCycle.ComputeLevelFor(from)), false, false);


            if (br != null)
            {
                AddButton(393, 274, 4005, 4007, 3, GumpButtonType.Reply, 0); // 1209, 1210
                AddHtml(430, 275, 170, 18, String.Format("<basefont color = #FFFFFF><right>Region Light:    {0}</right></basefont>", from.Region.Area.Length), false, false);

                AddButton(393, 304, 4005, 4007, 3, GumpButtonType.Reply, 0); // 1209, 1210
                AddHtml(430, 305, 170, 18, String.Format("<basefont color = #FFFFFF><right>XY Coordinates:  {0}</right></basefont>", from.Region.Area.Length), false, false);

                if (br.Spawns != null)
                {
                    AddButton(393, 334, 4005, 4007, 3, GumpButtonType.Reply, 0); // 1209, 1210
                    AddHtml(430, 335, 170, 18, String.Format("<basefont color = #FFFFFF><right>Region Spawn:    {0}</right></basefont>", br.Spawns.Length), false, false);
                }

            //    AddButton(393, 304, 4005, 4007, 3, GumpButtonType.Reply, 0); // 1209, 1210
            //    AddHtml(430, 305, 170, 18, String.Format("<basefont color = #FFFFFF><right>Region Music:    {0}</right></basefont>", from.Region.Music.ToString()), false, false);


            }

            //     AddHtml(100, 151, 505, 18, String.Format("<basefont color = #00FF00><center>{0}</center></basefont>", "Regional Control"), false, false);

            //   AddHtml(40, 200, 500, 18, String.Format("<basefont color = #000000><center>{0}</center></basefont>", "Season Change"), false, false);


            //   AddHtml(20, 76, 160, 18, String.Format("<basefont color = #07FF00><center><i>Total Gold: {0}</i></center></basefont>", GetGold()), false, false);
            //   AddHtml(20, 76, 160, 18, String.Format("<basefont color = #FACC2E><center><i>Total Banked Gold: {0}</i></center></basefont>", GetGoldOwned()), false, false);

            //    AddHtml(167, 76, 35, 18, , false, false);

            /*
            AddBackground(239, 22, 130, 40, 9270);
            AddButton(250, 29, 2445, 2445, 0, GumpButtonType.Reply, 0);// check this with kr client!
            AddAlphaRegion(239, 22, 130, 40);
            AddLabel(269, 31, 1370, "ADMIN MENU");
            */

            new UltimateAdminTimer(from).Start();
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null) return;
            if (info.ButtonID < 1)
            {              
                from.CloseGump(typeof(UltimateAdmin));
                from.SendGump(new UltimateAdminMini(from));
                return;
            }

        } 

        public class UltimateAdminTimer : Timer
        {
            private readonly Mobile m_Owner;

            public UltimateAdminTimer(Mobile owner)
                : base(TimeSpan.FromSeconds(0.5))
            {
                this.m_Owner = owner;
                this.Priority = TimerPriority.EveryTick;
            }

            protected override void OnTick()
            {
                if (this.m_Owner.HasGump(typeof(UltimateAdmin)))
                {
                    this.m_Owner.CloseGump(typeof(UltimateAdmin));

                    if (facecount < 6) facecount++;
                    else facecount = 0;

                    if (globecount < 15) globecount++;
                    else globecount = 0;

                    this.m_Owner.SendGump(new UltimateAdmin(this.m_Owner));
                }
            }
        }

    }

    public class UltimateAdminMini : Gump
    {
        static string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        static string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        static string Label(string text, string value)
        {
            return String.Format("{0}: {1}", text, value);
        }
        
        public UltimateAdminMini(Mobile from) : base(0, 0)
        {
            if (from == null) return;

            NetState ns = from.NetState;
            if (ns == null) return;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);
            AddBackground(79, 72, 130, 45, 9270);
            AddButton(90, 83, 2445, 2445, 1, GumpButtonType.Reply, 0);// check this with kr client!
            AddImageTiled(84, 77, 120, 35, 2624);

        //  AddLabel(104, 85, 2727, "ADMIN MENU");//1370

            /*
            if (ns.IsKRClient)
            {
                AddHtml(109, 87, 125, 35, Color(Center("ADMIN MENU"), 0xFFFFFF), false, false);
            }
            else
            {
            */
                AddHtml(84, 86, 125, 35, Color(Center("ADMIN MENU"), 0xFFFFFF), false, false);
         //   }

            AddAlphaRegion(84, 77, 125, 35);

        }
                  
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null) return;
            from.CloseGump(typeof(UltimateAdminMini));
            from.SendGump(new UltimateAdmin(from));
        }
    }
}