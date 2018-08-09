using System;
using Server.Guilds;

namespace Server
{
    public delegate void CreateGuildHandler(CreateGuildEventArgs e);
    public delegate void GuildGumpRequestHandler(GuildGumpRequestArgs e);

    public static partial class EventSink
    {
        public static event CreateGuildHandler CreateGuild;
        public static event GuildGumpRequestHandler GuildGumpRequest;
        
        public static void InvokeCreateGuild(CreateGuildEventArgs e)
        {
            CreateGuild?.Invoke(e);
        }

        public static void InvokeGuildGumpRequest(GuildGumpRequestArgs e)
        {
            GuildGumpRequest?.Invoke(e);
        }

        private static void ResetGuilds()
        {
            CreateGuild = null;
            GuildGumpRequest = null;
        }
    }

    public class CreateGuildEventArgs : EventArgs
    {
        public int Id { get; set; }

        public BaseGuild Guild { get; set; }

        public CreateGuildEventArgs(int id)
        {
            Id = id;
        }
    }

    public class GuildGumpRequestArgs : EventArgs
    {
        private readonly Mobile m_Mobile;

        public Mobile Mobile { get { return m_Mobile; } }

        public GuildGumpRequestArgs(Mobile mobile)
        {
            m_Mobile = mobile;
        }
    }
}
