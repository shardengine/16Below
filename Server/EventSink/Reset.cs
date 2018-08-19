namespace Server
{
    public static partial class EventSink
    {
        private static void ResetCustoms()
        {
        //    ResetEC();          // MAKE AN IF EC CHECK HERE.. Fraz
        }

        public static void Reset()
        {
            ResetAbilities();
            ResetAccounts();
            ResetBOD();
            ResetCharacter();
            ResetChat();
            ResetClients();
            ResetCombat();
            ResetCommands();
            ResetDeath();
            ResetGuilds();
            ResetHelp();
            ResetItems();
            ResetMacro();
            ResetMaps();
            ResetMovement();
            ResetNetwork();
            ResetQuests();
            ResetRegions();
            ResetServer();
            ResetSkills();
            ResetSpeech();
            ResetSpells();
            ResetVirtue();
            ResetWorld();
        }
    }
    
}

