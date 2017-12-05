using Discord.Commands;
using dotbot.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotbot.Commands
{
    public class Definitions : ModuleBase<SocketCommandContext>
    {
        [Command("set")]
        [Alias("define")]
        [Summary("save some text for later")]
        public async Task SetDefinition([Summary("key to set")] string Key, [Remainder] [Summary("what to set it to")] string Value)
        {
            using (var db = new DotbotDbContext())
            {
                if (db.Defs.Any(c => c.Id == Key))
                    db.Defs.Find(Key).Def = Value;
                else
                    db.Defs.Add(new Definition { Id = Key, Def = Value });
                db.SaveChanges();
                await ReplyAsync($"`{Key}` set to `{Value}`");
            }
        }


        [Command("get")]
        [Summary("get some text that was saved")]
        public async Task GetDefinition([Summary("key to look for")] string Key)
        {
            using (var db = new DotbotDbContext())
            {
                var def = db.Defs.Find(Key);
                await ReplyAsync($"**{Key}**: {def?.Def ?? "not set"}");
            }
        }


        [Command("unset")]
        [Summary("remove something you saved before")]
        public async Task RemoveDefinition([Summary("key to remove")] string Key)
        {
            using (var db = new DotbotDbContext())
            {
                if (db.Defs.Any(d => d.Id == Key))
                {
                    db.Defs.Remove(db.Defs.Find(Key));
                    db.SaveChanges();
                    await ReplyAsync($"**{Key}** removed successfully");
                }
                else
                {
                    await ReplyAsync($"**{Key}** doesn't exist");
                }
            }
        }


        [Command("defs")]
        [Summary("print all saved definitions")]
        public async Task GetAllDefinitions()
        {
            using (var db = new DotbotDbContext())
            {
                var res = new StringBuilder();
                foreach (var def in db.Defs)
                {
                    res.AppendLine($"`{def.Id}`: {def.Def}");
                }
                await ReplyAsync(res.ToString());
            }
        }

    }

}
