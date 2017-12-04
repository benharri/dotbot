using Discord.Commands;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace dotbot.Commands
{
    public class AsciiArt : ModuleBase<SocketCommandContext>
    {
        private readonly IConfigurationRoot _config;

        public AsciiArt(IConfigurationRoot config) {
            _config = config;
        }

        [Command("ascii")]
        [Summary("creates ascii word art")]
        public async Task CreateAsciiArt(
            [Summary("font you want to use")] string fontName,
            [Remainder] [Summary("text to convert")] string ArtString
        ) {
            if (fontName == "list") {
                return await ReplyAsync($"available fonts for use with `{_config["prefix"]}ascii`:\n```{string.Join(", ", Directory.GetFiles("Fonts").Select(Path.GetFileNameWithoutExtension))}```");
            } else if (File.Exists($"Fonts/{fontName}.flf")) {
                var font = new WenceyWang.FIGlet.FIGletFont(File.Open($"Fonts/{fontName}.flf"));
                await ReplyAsync($"```\n{(new WenceyWang.FIGlet.AsciiArt(ArtString, font: font)).ToString()}\n```");
            } else {
                await ReplyAsync($"```\n{(new WenceyWang.FIGlet.AsciiArt(fontName + ArtString)).ToString()}\n```");
            }
        }
    }
}

