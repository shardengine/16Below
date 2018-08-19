using System;
using Server.Network;
using Server.Accounting;

namespace Server
{
    public delegate void AccountLoginEventHandler(AccountLoginEventArgs e);
    public delegate void AccountDeleteEventHandler(AccountDeleteEventArgs e);

    public static partial class EventSink
    {
        public static event AccountLoginEventHandler AccountLogin;
        public static event AccountDeleteEventHandler AccountDelete;

        public static void InvokeAccountLogin(AccountLoginEventArgs e)
        {
            AccountLogin?.Invoke(e);
        }

        public static void InvokeAccountDelete(AccountDeleteEventArgs e)
        {
            AccountDelete?.Invoke(e);
        }

        private static void ResetAccounts()
        {
            AccountLogin = null;
            AccountDelete = null;
        }
    }

    public class AccountLoginEventArgs : EventArgs
    {
        private readonly NetState m_State;
        private readonly string m_Username;
        private readonly string m_Password;

        public NetState State { get { return m_State; } }
        public string Username { get { return m_Username; } }
        public string Password { get { return m_Password; } }
        public bool Accepted { get; set; }
        public ALRReason RejectReason { get; set; }

        public AccountLoginEventArgs(NetState state, string username, string password)
        {
            m_State = state;
            m_Username = username;
            m_Password = password;
        }
    }

    public class AccountDeleteEventArgs : EventArgs
    {
        private IAccount m_Account;

        public IAccount Account
        {
            get { return m_Account; }
            set { m_Account = value; }
        }

        public AccountDeleteEventArgs(IAccount a)
        {
            m_Account = a;
        }
    }
}
