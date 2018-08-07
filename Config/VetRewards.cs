using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Configuration
{
    public partial class Config
    {
        public class VetRewards
        {
            public static bool      Enabled             = false;
            public static bool      AgeCheckOnUse       = false;                    // N.B. Broken right now? -Fraz?
            public static bool      SkillCapRewards     = false;
            public static int       SkillCapBonus       = 200;                      // SkillCapMaxBonus
            public static int       SkillCapBonusLevels = 4;
            public static TimeSpan  RewardInterval      = TimeSpan.FromDays(30.0);  // On OSI servers this would be one year
            public static int       StartingLevel       = 0;
        }
    }
}
