using Discord.Commands;
using dotbot.Core;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace dotbot.Commands
{
    [Group("img")]
    public class Images : ModuleBase<SocketCommandContext>
    {
        [Command]
        [Alias("image", "pic")]
        [Summary("get a saved image")]
        public async Task GetImage([Summary("image name")] string name)
        {

        }
    }
}
