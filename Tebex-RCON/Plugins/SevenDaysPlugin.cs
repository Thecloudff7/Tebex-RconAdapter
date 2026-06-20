using Tebex_RCON.RCON;
using Tebex_RCON.RCON.Protocol;
using Tebex_RCON.Tebex;

namespace Tebex_RCON.Plugins
{
    public class SevenDaysPlugin : RconPlugin
    {
        public SevenDaysPlugin(TebexRconAdapter adapter) : base(adapter)
        {
        }

        public override RconConnection CreateRconConnection(string host, int port, string password)
        {
            return new TelnetRcon(Adapter, host, port, password);
        }

        public override string GetPluginVersion()
        {
            return "1.0.0";
        }

        public override bool IsPlayerOnline(TebexApi.DuePlayer player)
        {
            Rcon.Send("listplayers");
            
            bool found = false;
            var cmdExecMessage = Rcon.ReceiveNext();

            while (true)
            {
                // After command exec message, this will be the first connected player
                var packet = Rcon.ReceiveNext();

                //TODO possible conflict with other commands that might be ran at the same time?
                if (packet.Message.Contains("pltfmid=") && packet.Message.Contains(player.Uuid))
                {
                    found = true;
                    break;
                }

                if (packet.Message.Contains("Total of")) // End of player list is a total of the connected players
                {
                    break;
                }
            }
            
            return found;
        }

        public override string ExpandGameUsernameVariables(string cmd, object playerObj)
        {
            return cmd;
        }
    }   
}