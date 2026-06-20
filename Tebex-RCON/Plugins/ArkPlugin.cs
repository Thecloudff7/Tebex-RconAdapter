using Tebex_RCON.RCON;
using Tebex_RCON.RCON.Protocol;
using Tebex_RCON.Tebex;

namespace Tebex_RCON.Plugins
{
    public class ArkPlugin : RconPlugin
    {
        private String _lastPlayerList;
        
        public ArkPlugin(TebexRconAdapter adapter) : base(adapter)
        {
            TebexRconAdapter.ExecuteEvery(TimeSpan.FromSeconds(5), () =>
            {
                Adapter.LogDebug("listplayers");
                
                if (Adapter.GetRcon() == null)
                {
                    Adapter.LogDebug("no rcon");
                    return;
                }

                if (!Adapter.GetRcon().IsConnected())
                {
                    return;
                }
                
                var listPlayersCommand = Adapter.GetRcon().Send("listplayers");
                RconPacket listPlayersResponse;
                listPlayersResponse = Adapter.GetRcon().ReceiveNext();
                
                // Keep Alive packets seem to knock things out of order, we can just ignore when we grab the wrong response
                // because we should be updating the list of players every few seconds anyway.
                if (listPlayersResponse != null && !listPlayersResponse.Message.Contains("But no response!!"))
                {
                    Adapter.LogDebug("received player list: " + listPlayersResponse.Message);
                    _lastPlayerList = listPlayersResponse.Message;
                }
            });
        }

        public override string GetPluginVersion()
        {
            return "1.0.1";
        }

        public override bool IsPlayerOnline(TebexApi.DuePlayer player)
        {
            bool foundUuid = _lastPlayerList.Contains(player.Uuid);
            if (!foundUuid)
            {
                Adapter.LogDebug("did not find " + player.Name + " by uuid in player list: " + player.Uuid);
                bool foundName = _lastPlayerList.Contains(player.Name);
                if (!foundName)
                {
                    Adapter.LogDebug("did not find " + player.Name + " by name in player list");
                    return false;
                }
                Adapter.LogDebug("successfully found " + player.Name + " by name in player list");
                return true;
            }
            return foundUuid;
        }

        public override string ExpandGameUsernameVariables(string cmd, object playerObj)
        {
            return cmd;
        }
        
        public override RconConnection CreateRconConnection(string host, int port, string password)
        {
            return new RconConnection(Adapter, host, port, password);
        }
    }   
}