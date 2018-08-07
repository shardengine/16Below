using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Misc
{
	class MathHelper
	{
		public static int Scale(int input, int percent)
		{
			return (input * percent) / 100;
		}

        public static int GetLabel(SkillName skill)
        {
            switch (skill)
            {
                case SkillName.EvalInt:
                    return 1002070; // Evaluate Intelligence
                case SkillName.Forensics:
                    return 1002078; // Forensic Evaluation
                case SkillName.Lockpicking:
                    return 1002097; // Lockpicking
                default:
                    return 1044060 + (int)skill;
            }
        }
	}
}
