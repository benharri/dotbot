using Discord.Commands;
using dotbot.Core;
using System.Linq;
using System.Threading.Tasks;

namespace dotbot.Commands
{
    public class Definitions : ModuleBase<SocketCommandContext>
    {
        public DotbotDb db { get; set; }

        [Command("set")]
        [Alias("define")]
        [Summary("save some text for later")]
        public async Task SetDefinition([Summary("key to set")] string Key, [Remainder] [Summary("what to set it to")] string Value)
        {
            if (db.Defs.Any(d => d.Id == Key))
                db.Defs.Find(Key).Def = Value;
            else
                db.Defs.Add(new Definition { Id = Key, Def = Value });
            db.SaveChanges();
            await ReplyAsync($"`{Key}` set to `{Value}`");
        }


        [Command("get")]
        [Summary("get some text that was saved")]
        public async Task GetDefinition([Summary("key to look for")] string Key) => await ReplyAsync($"**{Key}**: {db.Defs.Find(Key)?.Def ?? "not set"}");


        [Command("unset")]
        [Summary("remove something you saved before")]
        public async Task RemoveDefinition([Summary("key to remove")] string Key)
        {
            if (db.Defs.Any(d => d.Id == Key))
            {
                db.Defs.Remove(db.Defs.Find(Key));
                db.SaveChanges();
                await ReplyAsync($"**{Key}** removed successfully");
            }
            else await ReplyAsync($"**{Key}** doesn't exist");
        }


        [Command("defs")]
        [Summary("print all saved definitions")]
        public async Task AllDefs() => await ReplyAsync(string.Join("\n", db.Defs.Select(i => $"`{i.Id}`: {i.Def}")));

    }

}
