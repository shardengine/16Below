using System;
using Server.Commands;

namespace Server.Administration.CustomTools 
{
    public class TimingTest
    {
        public static void Initialize()
        {
            CommandSystem.Register("TimerTest", AccessLevel.Player, new CommandEventHandler(TimerTest_OnCommand));
        }

        [Usage("TimerTest")]
        [Description("Testing internal timers for accuracy!")]
        private static void TimerTest_OnCommand(CommandEventArgs e)
        {
            Mobile tester = e.Mobile;
            int count = e.GetInt32(0);
            InternalTimer t = new InternalTimer(tester, count);
            t.Start();
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Tester;
            private DateTime StartTime; 

            public InternalTimer(Mobile tester, int count)
                : base(TimeSpan.FromSeconds(1.0))
            {
                if (count > 0) Delay = TimeSpan.FromSeconds(count);
                else Delay = TimeSpan.FromSeconds(1.0);
                m_Tester = tester;
                Priority = TimerPriority.FiftyMS;
                StartTime = DateTime.UtcNow;
                m_Tester.PublicOverheadMessage(0, 0x22, true, "Timer test initiated!");
            }

            protected override void OnTick()
            {
                if (m_Tester == null) return;
                string Start = string.Format("Time started: {0}", StartTime);
                string Finish = string.Format("Time ended: {0}", DateTime.UtcNow);
                m_Tester.PublicOverheadMessage(0, 0x22, true, Start);
                m_Tester.PublicOverheadMessage(0, 0x22, true, Finish);
                Console.WriteLine(Start);
                Console.WriteLine(Finish);
            }
        }
    }
}
