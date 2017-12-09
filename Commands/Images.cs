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
        public DotbotDb db { get; set; }

        [Command]
        [Alias("image", "pic")]
        [Priority(0)]
        [Summary("get a saved image")]
        public async Task GetImage([Summary("image name")] string name)
        {

        }

        
        [Command("list")]
        [Alias("ls")]
        [Priority(1)]
        [Summary("list all saved images")]
        public async Task ListImages()
        {

        }


        [Command("save")]
        [Priority(1)]
        [Summary("save an image for later")]
        public async Task SaveImage([Summary("image name")] string name)
        {

        }



    }
}
