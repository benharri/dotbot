using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace dotbot.Commands
{
    public class Util : ModuleBase<SocketCommandContext>
    {
        [Command("exit")]
        [Alias("die", "shutdown")]
        [RequireOwner]
        public async Task ShutDown()
        {
            Environment.Exit(0);
        }
    }
}
