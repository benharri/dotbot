using Discord.Commands;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WenceyWang.FIGlet;

namespace dotbot.Commands
{
    [Group("ascii")]
    public class Ascii : ModuleBase<SocketCommandContext>
    {
        private readonly IConfigurationRoot _config;

        public Ascii(IConfigurationRoot config) {
            _config = config;
        }

        [Command]
        [Summary("creates ascii word art")]
        public async Task CreateAsciiArt(
            [Summary("font you want to use")] string fontName,
            [Remainder] [Summary("text to convert")] string message = ""
        ) {
            if (fontName == "list")
                await GetFontList();
            else if (File.Exists($"Fonts/{fontName}.flf"))
                using (var fs = File.OpenRead($"Fonts/{fontName}.flf"))
                    await ReplyAsync($"```\n{new AsciiArt(message, font: new FIGletFont(fs))}\n```");
            else
                await ReplyAsync($"```\n{new AsciiArt(fontName + message)}\n```");
        }


        [Command("list")]
        [Alias("fonts", "fontlist")]
        [Summary("show list of available ascii fonts")]
        public async Task GetFontList()
        {
            var fontList = Directory.GetFiles("Fonts").ToList().Select(Path.GetFileNameWithoutExtension);
            await ReplyAsync($"available fonts for use with `{_config["prefix"]}ascii`:\n```{string.Join(", ", fontList)}\n```");
        }

    }
}

