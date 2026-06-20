using Tebex_RCON.RCON;
using Tebex_RCON.RCON.Protocol;
using Tebex_RCON.Tebex;

namespace Tebex_RCON.Plugins
{
    public class ConanExilesPlugin : RconPlugin
    {

        private List<ConanPlayerInfo> _lastPlayerList = new List<ConanPlayerInfo>();
        
        public ConanExilesPlugin(TebexRconAdapter adapter) : base(adapter)
        {
            TebexRconAdapter.ExecuteEvery(TimeSpan.FromSeconds(45), () =>
            {
                try
                {
                    GetOnlinePlayers();
                }
                catch (Exception e)
                {
                    Adapter.LogError($"Error while getting online players: {e.Message}");
                }
            });
        }

        public class ConanPlayerInfo
        {
            public int Idx { get; set; }
            public string CharName { get; set; }
            public string PlayerName { get; set; }
            public string UserId { get; set; }
            public string PlatformId { get; set; }
            public string PlatformName { get; set; }
            
            public static List<ConanPlayerInfo> ParsePlayerList(string? input)
            {
                var playerInfoList = new List<ConanPlayerInfo>();
                if (input == null)
                {
                    return playerInfoList;
                }
                
                string[] lines = input.Trim().Split('\n');
        
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] data = lines[i].Split('|');

                    playerInfoList.Add(new ConanPlayerInfo
                    {
                        Idx = int.Parse(data[0].Trim()),
                        CharName = data[1].Trim(),
                        PlayerName = data[2].Trim(),
                        UserId = data[3].Trim(),
                        PlatformId = data[4].Trim(),
                        PlatformName = data[5].Trim()
                    });
                }

                return playerInfoList;
            }
        }

        public void GetOnlinePlayers()
        {
            Adapter.LogDebug($"Querying server for online player list...");
            var listPacket = Rcon.Send("listplayers");
            var listResponse = Rcon.ReceiveNext();
            
            var currentPlayerList = ConanPlayerInfo.ParsePlayerList(listResponse.Message);
            Adapter.LogDebug($"Detected {currentPlayerList.Count} online Conan players");
            
            List<string> oldJoins = new List<string>();
            foreach (var playerInfo in _lastPlayerList)
            {
                oldJoins.Add(playerInfo.PlatformId);
            }
            
            List<string> newJoins = new List<string>();
            foreach (var playerInfo in currentPlayerList)
            {
                if (!oldJoins.Contains(playerInfo.PlatformId))
                {
                    newJoins.Add(playerInfo.PlatformId);
                }
            }

            _lastPlayerList = currentPlayerList;
            foreach (var id in newJoins)
            {
                //TODO Player IP is not accurate
                Adapter.OnUserConnected(id, "0.0.0.0");
            }
        }

        public override bool IsPlayerOnline(TebexApi.DuePlayer duePlayer)
        {
            foreach (var player in _lastPlayerList)
            {
                if (player.PlatformId.Equals(duePlayer.Uuid) || player.CharName == duePlayer.Name)
                {
                    return true;
                }
            }

            return false;
        }

        public override string GetPluginVersion()
        {
            return "1.0.0";
        }

        public override object GetPlayerRef(string idOrUsername)
        {
            return _getPlayerPositionId(idOrUsername);
        }

        public override string ExpandGameUsernameVariables(string cmd, object playerObj)
        {
            foreach (var playerInfo in _lastPlayerList)
            {
                if (playerInfo.Idx == (int)playerObj) //playerObj is player position ID for Conan Exiles
                {
                    cmd = cmd.Replace("{playercharactername}", playerInfo.CharName);
                    break;
                }
            }

            return cmd;
        }

        /**
         * Conan Exiles identifies its players in commands via their positional ID in the players list. This
         * searches for the player in the players list and returns their "Idx" or their position in the list.
         */
        private int _getPlayerPositionId(string idOrUsername)
        {
            // Refreshes lastPlayerList
            GetOnlinePlayers();
            
            foreach (var playerInfo in _lastPlayerList)
            {
                if (playerInfo.PlatformId.Equals(idOrUsername) || playerInfo.CharName.Equals(idOrUsername))
                {
                    return playerInfo.Idx;
                }
            }

            return -1;
        }

        public override bool HasCustomPlayerRef()
        {
            return true;
        }
    }   
}