using Tebex_RCON.RCON;
using Tebex_RCON.RCON.Protocol;
using Tebex_RCON.Tebex;

namespace Tebex_RCON.Plugins
{
    public class MinecraftPlugin : RconPlugin
    {
        public MinecraftPlugin(TebexRconAdapter adapter) : base(adapter)
        {

        }

        public override string GetPluginVersion()
        {
            return "1.0.0";
        }

        public override bool IsPlayerOnline(TebexApi.DuePlayer player)
        {
            // We can allow the Minecraft server to tell us if the command succeeded or not by assuming the player is online.
            // Minecraft will return an error if the command fails which can be interpreted by the adapter.
            return true;
        }

        public override string ExpandGameUsernameVariables(string cmd, object playerObj)
        {
            return cmd;
        }
    }   
}