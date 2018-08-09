#region Header
// **********
// 16Below - EventLog.cs
// **********
#endregion

#region References
using System;
using System.Diagnostics;

using DiagELog = System.Diagnostics.EventLog;
#endregion

namespace Server
{
	public static class EventLog
	{
		static EventLog()
		{
			if (!DiagELog.SourceExists("16Below"))
			{
				DiagELog.CreateEventSource("16Below", "Application");
			}
		}

		public static void Error(int eventID, string text)
		{
			DiagELog.WriteEntry("16Below", text, EventLogEntryType.Error, eventID);
		}

		public static void Error(int eventID, string format, params object[] args)
		{
			Error(eventID, String.Format(format, args));
		}

		public static void Warning(int eventID, string text)
		{
			DiagELog.WriteEntry("16Below", text, EventLogEntryType.Warning, eventID);
		}

		public static void Warning(int eventID, string format, params object[] args)
		{
			Warning(eventID, String.Format(format, args));
		}

		public static void Inform(int eventID, string text)
		{
			DiagELog.WriteEntry("16Below", text, EventLogEntryType.Information, eventID);
		}

		public static void Inform(int eventID, string format, params object[] args)
		{
			Inform(eventID, String.Format(format, args));
		}
	}
}