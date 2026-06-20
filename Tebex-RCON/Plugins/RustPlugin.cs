using Tebex_RCON.RCON;
using Tebex_RCON.RCON.Protocol;
using Tebex_RCON.Tebex;

namespace Tebex_RCON.Plugins
{
    public class RustPlugin : RconPlugin
    {
        public RustPlugin(TebexRconAdapter adapter) : base(adapter) {}

        public override string GetPluginVersion()
        {
            return "1.0.0";
        }
        
        public override bool IsPlayerOnline(TebexApi.DuePlayer player)
        {
            bool found = false;
            var cmdExecMessage = Rcon.Send("list");

            int tries = 0;
            while (tries < 10)
            {
                Thread.Sleep(200); // wait for websocket response to be polled and added to responses
                var message = Rcon.ReceiveResponseTo(cmdExecMessage.Id, 10);
                if (!message.Item2.Equals("")) // no response yet, error is present
                {
                    tries++;
                    continue;
                }

                // successfully got response to our list message
                return message.Item1.Response.Message.Contains(player.Name) || message.Item1.Response.Message.Contains(player.Uuid);
            }

            return false;
        }

        public override RconConnection CreateRconConnection(string host, int port, string password)
        {
            return new WebsocketRcon(Adapter, host, port, password);
        }
    }   
}