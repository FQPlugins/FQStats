using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FQStats
{
    public class Main : RocketPlugin<Config>
    {
        public static Main Instance;
        public static DBManager Database;
        protected override void Load()
        {
            UnturnedPlayerEvents.OnPlayerDeath += UnturnedPlayerEvents_OnPlayerDeath;
            Instance = this;
            Database = new DBManager();
        }

        private void UnturnedPlayerEvents_OnPlayerDeath(Rocket.Unturned.Player.UnturnedPlayer player, SDG.Unturned.EDeathCause cause, SDG.Unturned.ELimb limb, Steamworks.CSteamID murderer)
        {
            var playerInfo = GetPlayerInfo(player.CSteamID.ToString()).Result;
            foreach (var info in playerInfo)
            {
                UnturnedChat.Say(info);
            }

            if (cause == SDG.Unturned.EDeathCause.GUN || cause == SDG.Unturned.EDeathCause.PUNCH || cause == SDG.Unturned.EDeathCause.MISSILE || cause == SDG.Unturned.EDeathCause.MELEE)
            {

            }
        }

        public static async Task<List<String>> GetPlayerInfo(String steam64) => await Database.GetPlayerInfo(steam64);

        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerDeath -= UnturnedPlayerEvents_OnPlayerDeath;
        }
    }
}
