using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Configuration
{
    public partial class Config
    {
        public class Champions
        {
            public static bool Enabled = false;

            public static TimeSpan RotateDelay = TimeSpan.FromDays(1.0);

            public static int GoldPiles = 50;
            public static int GoldMin = 2500;
            public static int GoldMax = 7500;

            public static int HarrowerGoldPiles = 75;
            public static int HarrowerGoldMin = 5000;
            public static int HarrowerGoldMax = 10000;

            public static int PowerScrolls = 6;
            public static int StatScrolls = 16;

            // Percent chance to get a Scroll of Transcendence or Scroll of Power from
            public static double ScrollChance = 0.1;
            public static double TranscendenceChance = 50.0;

            // How many red skulls required to move the spawn to rank 2
            public static int Rank2RedSkulls = 5;
            // How many red skulls required to move the spawn to rank 3
            public static int Rank3RedSkulls = 10;
            // How many red skulls required to move the spawn to rank 4 (at 17 red skulls the champion will always appear)
            public static int Rank4RedSkulls = 13;
            // Number of kills needed to advance from level to level while in rank 1
            public static int Rank1MaxKills = 256;
            // Number of kills needed to advance from level to level while in rank 2
            public static int Rank2MaxKills = 128;
            // Number of kills needed to advance from level to level while in rank 3
            public static int Rank3MaxKills = 64;
            //ne Number of kills needed to advance from level to level while in rank 4
            public static int Rank4MaxKills = 32;
        }
    }
}
