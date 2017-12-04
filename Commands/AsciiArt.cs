using Discord.Commands;
using System.Threading.Tasks;

namespace dotbot.Commands
{
    public class AsciiArt : ModuleBase<SocketCommandContext>
    {
        [Command("ascii")]
        [Summary("creates ascii word art")]
        public async Task CreateAsciiArt(
            [Summaery("font you want to use")] string fontName,
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
