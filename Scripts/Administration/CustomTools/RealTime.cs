using System;

namespace Server.Commands
{
    public class RealTime
    {
        public static void Initialize()
        {
            CommandSystem.Register("RealTime", AccessLevel.Player, new CommandEventHandler(CheckTime_OnCommand));
        }

        [Usage("RealTime")]
        [Description("Check's Your Servers Current Date And Time")]
        public static void CheckTime_OnCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;
            DateTime now = DateTime.UtcNow;
            m.SendMessage("The Current Date And Time Is " + now + "(EST)");         
        }
    }
}