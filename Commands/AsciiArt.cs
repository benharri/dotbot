using Discord.Commands;
using System.Threading.Tasks;

namespace dotbot.Commands
{
    public class AsciiArt : ModuleBase<SocketCommandContext>
    {
        [Command("ascii")]
        [Summary("creates ascii word art")]
        public async Task CreateAsciiArt([Remainder] [Summary("text to convert")] string ArtString)
        {
            await ReplyAsync($"```\n{(new WenceyWang.FIGlet.AsciiArt(ArtString)).ToString()}\n```");
        }

    }
}
