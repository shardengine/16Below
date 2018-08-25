using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Network;

namespace Server.Configuration
{
    public enum PasswordProtection
    {
        None,
        Crypt,
        NewCrypt 
    }

    public partial class Config
    {
        public class Server
        {
            public static string    ServerName          = "16Below";
            public static Expansion Expansion           = Expansion.LBR;          // Default: None  T2A  UOR  UOTD  LBR
            public static bool      TestCenterEnabled   = false;
        }

        public class Network
        {
            public static bool      AutoDetect  = true;
            public static string    Address     = "";                             // Default: ""  LoopBack: "127.0.0.1"
            public static int       Port        = 2593;
        }

        public class DataPath
        { 
            public static string    CustomPath          = "";
            public static bool      IgnoreStandardPaths = false;
        }

        public class Accounts
        {
            public static int                   HouseLimit              = 1;
            public static int                   MaxAccountsPerIP        = 5;
            public static bool                  AutoAccountCreation     = true;
            public static bool                  RestrictDeletion        = false;
            public static TimeSpan              DeleteDelay             = TimeSpan.FromDays(0.0);
            public static bool                  PasswordCommandEnabled  = false;
            public static PasswordProtection    ProtectPasswords        = PasswordProtection.NewCrypt;
            public static TimeSpan              InactiveDuration        = TimeSpan.FromDays(180.0);
            public static TimeSpan              EmptyInactiveDuration   = TimeSpan.FromDays(30.0);
            public static bool                  IPLimiterEnabled        = true;
            public static bool                  SocketBlock             = true;
            public static int                   MaxAddresses            = 10;
        }

        public class WorldSave
        {
            public static bool      AutoSavesEnabled    = true;
            public static TimeSpan  Delay               = TimeSpan.FromMinutes(30.0);
            public static TimeSpan  Warning             = TimeSpan.FromSeconds(15.0);
        }

        public class Staff
        {
            public static string CommandPrefix  = "[";

            public static bool Staffbody        = false;
            public static bool UseColoring      = true;
            public static bool GiveBoots        = true;
            public static bool CutHair          = false;
            public static bool CutFacialHair    = false;
            // Coloring configuration
            public static int Owner             = 1001;
            public static int Developer         = 1001;
            public static int Administrator     = 1001;
            public static int Seer              = 467;
            public static int GameMaster        = 39;
            public static int Counselor         = 3;
        }

        public class Player
        {
            public static TimeSpan YoungDuration = TimeSpan.FromHours(40.0);

            public static CityInfo[] StartingCities = new CityInfo[]
            {
            //  new CityInfo("New Haven",   "New Haven Bank",           1150168, 3667,  2625,  0),
                new CityInfo("Yew",         "The Empath Abbey",         1075072, 633,   858,   0),
                new CityInfo("Minoc",       "The Barnacle",             1075073, 2476,  413,   15),
                new CityInfo("Britain",     "The Wayfarer's Inn",       1075074, 1602,  1591,  20),
                new CityInfo("Moonglow",    "The Scholars Inn",         1075075, 4408,  1168,  0),
                new CityInfo("Trinsic",     "The Traveler's Inn",       1075076, 1845,  2745,  0),
                new CityInfo("Magincia",    "The Great Horns Tavern",   1075077, 3738,  2223,  0),
                new CityInfo("Jhelom",      "The Mercenary Inn",        1075078, 1374,  3826,  0),
                new CityInfo("Skara Brae",  "The Falconer's Inn",       1075079, 618,   2234,  0),
                new CityInfo("Vesper",      "The Ironwood Inn",         1075080, 2771,  976,   0)
            };

            public static bool      EnableAntiMacro             = true;
            public static TimeSpan  AntiMacroExpire             = TimeSpan.FromMinutes(5.0); 

            public static int       SkillCap                    = 1000;
            public static int       TotalSkillCap               = 7000;         // update PlayerMobile and BaseCreature -Fraz
            public static int       StatCap                     = 125;          
            public static int       TotalStatCap                = 225;          // update PlayerMobile and BaseCreature -Fraz

            public static double    PlayerChanceToGainStats     = 5.0;          // Default is 5% from OSI publish 45 notes.
            public static double    PetChanceToGainStats        = 5.0;

            public static bool      EnablePlayerStatTimeDelay   = true;
            public static TimeSpan  PlayerStatTimeDelay         = TimeSpan.FromMinutes(15.0);
                
            public static bool      EnablePetStatTimeDelay      = false;
            public static TimeSpan  PetStatTimeDelay            = TimeSpan.FromMinutes(5.0);

        }

        public class Factions
        {
            // If true, the Council of Mages stronghold at the south of Moonglow 
            // is used instead of the old location in the Magincia Parlament Building.
            public static bool NewCoMLocation = false;
        }
        
        public class Vendors
        {
            public static TimeSpan  RestockDelay = TimeSpan.FromHours(1.0);      
            public static int       MaxSell      = 500;                          
        }

        public class TreasureMap
        {           
            public static bool      Randomized  = false;                        
            public static double    LootChance  = .01;
            public static TimeSpan  ResetTime   = TimeSpan.FromDays(1.0);
        }

    }
}
