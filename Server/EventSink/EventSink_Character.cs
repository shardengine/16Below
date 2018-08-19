using System;
using Server.Accounting;
using Server.Network;

namespace Server
{
    public delegate void CharacterCreatedEventHandler(CharacterCreatedEventArgs e);
    public delegate void DeleteRequestEventHandler(DeleteRequestEventArgs e);
    public delegate void PaperdollRequestEventHandler(PaperdollRequestEventArgs e);
    public delegate void ProfileRequestEventHandler(ProfileRequestEventArgs e);
    public delegate void ChangeProfileRequestEventHandler(ChangeProfileRequestEventArgs e);
    public delegate void RenameRequestEventHandler(RenameRequestEventArgs e);
    public delegate void HungerChangedEventHandler(HungerChangedEventArgs e);
    public delegate void PoisonCuredEventHandler(PoisonCuredEventArgs e);

    public struct SkillNameValue
    {
        private readonly SkillName m_Name;
        private readonly int m_Value;

        public SkillName Name { get { return m_Name; } }
        public int Value { get { return m_Value; } }

        public SkillNameValue(SkillName name, int value)
        {
            m_Name = name;
            m_Value = value;
        }
    }

    public static partial class EventSink   // NEEDS EC SPLIT OUT SOLUTION ATM?? Fraz
    {
        public static event CharacterCreatedEventHandler CharacterCreated;
        public static event DeleteRequestEventHandler DeleteRequest;
        public static event PaperdollRequestEventHandler PaperdollRequest;
        public static event ProfileRequestEventHandler ProfileRequest;
        public static event ChangeProfileRequestEventHandler ChangeProfileRequest;
        public static event RenameRequestEventHandler RenameRequest;
        public static event HungerChangedEventHandler HungerChanged;
        public static event PoisonCuredEventHandler PoisonCured;

        public static void InvokeCharacterCreated(CharacterCreatedEventArgs e)
        {
            CharacterCreated?.Invoke(e);
        }

        public static void InvokeDeleteRequest(DeleteRequestEventArgs e)
        {
            DeleteRequest?.Invoke(e);
        }

        public static void InvokePaperdollRequest(PaperdollRequestEventArgs e)
        {
            PaperdollRequest?.Invoke(e);
        }

        public static void InvokeProfileRequest(ProfileRequestEventArgs e)
        {
            ProfileRequest?.Invoke(e);
        }

        public static void InvokeChangeProfileRequest(ChangeProfileRequestEventArgs e)
        {
            ChangeProfileRequest?.Invoke(e);
        }

        public static void InvokeRenameRequest(RenameRequestEventArgs e)
        {
            RenameRequest?.Invoke(e);
        }

        public static void InvokeHungerChanged(HungerChangedEventArgs e)
        {
            HungerChanged?.Invoke(e);
        }

        public static void InvokePoisonCured(PoisonCuredEventArgs e)
        {
            PoisonCured?.Invoke(e);
        }

        private static void ResetCharacter()
        {
            CharacterCreated = null;
            DeleteRequest = null;
            PaperdollRequest = null;
            ProfileRequest = null;
            ChangeProfileRequest = null;
            RenameRequest = null;
            HungerChanged = null;
            PoisonCured = null;
        }
    }

    public class CharacterCreatedEventArgs : EventArgs
    {
        private NetState m_State;
        private IAccount m_Account;
        private CityInfo m_City;
        private SkillNameValue[] m_Skills;
        private int m_ShirtHue, m_PantsHue;
        private int m_HairID, m_HairHue;
        private int m_BeardID, m_BeardHue;
        private string m_Name;
        private bool m_Female;
        private int m_Hue;
        private int m_Str, m_Dex, m_Int;
        private int m_Profession;
        private Mobile m_Mobile;

        private Race m_Race;

        public NetState State { get { return m_State; } }
        public IAccount Account { get { return m_Account; } }
        public Mobile Mobile { get { return m_Mobile; } set { m_Mobile = value; } }
        public string Name { get { return m_Name; } }
        public bool Female { get { return m_Female; } }
        public int Hue { get { return m_Hue; } }
        public int Str { get { return m_Str; } }
        public int Dex { get { return m_Dex; } }
        public int Int { get { return m_Int; } }
        public CityInfo City { get { return m_City; } }
        public SkillNameValue[] Skills { get { return m_Skills; } }
        public int ShirtHue { get { return m_ShirtHue; } }
        public int PantsHue { get { return m_PantsHue; } }
        public int HairID { get { return m_HairID; } }
        public int HairHue { get { return m_HairHue; } }
        public int BeardID { get { return m_BeardID; } }
        public int BeardHue { get { return m_BeardHue; } }
        public int Profession { get { return m_Profession; } set { m_Profession = value; } }
        public Race Race { get { return m_Race; } }

        public CharacterCreatedEventArgs(NetState state, IAccount a, string name, bool female, int hue, int str, int dex, int intel, CityInfo city, SkillNameValue[] skills, int shirtHue, int pantsHue, int hairID, int hairHue, int beardID, int beardHue, int profession, Race race)
        {
            m_State = state;
            m_Account = a;
            m_Name = name;
            m_Female = female;
            m_Hue = hue;
            m_Str = str;
            m_Dex = dex;
            m_Int = intel;
            m_City = city;
            m_Skills = skills;
            m_ShirtHue = shirtHue;
            m_PantsHue = pantsHue;
            m_HairID = hairID;
            m_HairHue = hairHue;
            m_BeardID = beardID;
            m_BeardHue = beardHue;
            m_Profession = profession;
            m_Race = race;
        }
    }

    public class DeleteRequestEventArgs : EventArgs
    {
        private readonly NetState m_State;
        private readonly int m_Index;

        public NetState State { get { return m_State; } }
        public int Index { get { return m_Index; } }

        public DeleteRequestEventArgs(NetState state, int index)
        {
            m_State = state;
            m_Index = index;
        }
    }

    public class PaperdollRequestEventArgs : EventArgs
    {
        private readonly Mobile m_Beholder;
        private readonly Mobile m_Beheld;

        public Mobile Beholder { get { return m_Beholder; } }
        public Mobile Beheld { get { return m_Beheld; } }

        public PaperdollRequestEventArgs(Mobile beholder, Mobile beheld)
        {
            m_Beholder = beholder;
            m_Beheld = beheld;
        }
    }

    public class ProfileRequestEventArgs : EventArgs
    {
        private readonly Mobile m_Beholder;
        private readonly Mobile m_Beheld;

        public Mobile Beholder { get { return m_Beholder; } }
        public Mobile Beheld { get { return m_Beheld; } }

        public ProfileRequestEventArgs(Mobile beholder, Mobile beheld)
        {
            m_Beholder = beholder;
            m_Beheld = beheld;
        }
    }

    public class ChangeProfileRequestEventArgs : EventArgs
    {
        private readonly Mobile m_Beholder;
        private readonly Mobile m_Beheld;
        private readonly string m_Text;

        public Mobile Beholder { get { return m_Beholder; } }
        public Mobile Beheld { get { return m_Beheld; } }
        public string Text { get { return m_Text; } }

        public ChangeProfileRequestEventArgs(Mobile beholder, Mobile beheld, string text)
        {
            m_Beholder = beholder;
            m_Beheld = beheld;
            m_Text = text;
        }
    }

    public class RenameRequestEventArgs : EventArgs
    {
        private readonly Mobile m_From;
        private readonly Mobile m_Target;
        private readonly string m_Name;

        public Mobile From { get { return m_From; } }
        public Mobile Target { get { return m_Target; } }
        public string Name { get { return m_Name; } }

        public RenameRequestEventArgs(Mobile from, Mobile target, string name)
        {
            m_From = from;
            m_Target = target;
            m_Name = name;
        }
    }

    public class HungerChangedEventArgs : EventArgs
    {
        private readonly Mobile m_Mobile;
        private readonly int m_OldValue;

        public Mobile Mobile { get { return m_Mobile; } }
        public int OldValue { get { return m_OldValue; } }

        public HungerChangedEventArgs(Mobile mobile, int oldValue)
        {
            m_Mobile = mobile;
            m_OldValue = oldValue;
        }
    }

    public class PoisonCuredEventArgs : EventArgs
    {
        public Mobile Mobile { get; private set; }
        public Poison Poison { get; private set; }

        public PoisonCuredEventArgs(Mobile mobile, Poison poison)
        {
            Mobile = mobile;
            Poison = poison;
        }
    }
}
