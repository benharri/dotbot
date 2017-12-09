using Discord.Commands;
using dotbot.Core;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace dotbot.Commands
{
    [Group("img")]
    public class Images : ModuleBase<SocketCommandContext>
    {
        public DotbotDb db { get; set; }
        private readonly IConfigurationRoot _config;

        public Images(IConfigurationRoot config)
        {
            _config = config;
        }


        [Command]
        [Alias("image", "pic")]
        [Priority(0)]
        [Summary("get a saved image")]
        public async Task GetImage([Summary("image name")] string name)
        {
            if (db.Images.Any(i => i.Id == name))
            {
                await Context.Channel.TriggerTypingAsync();
                await Context.Message.DeleteAsync();
                var img = db.Images.Find(name);
                await Context.Channel.SendFileAsync($"UploadedImages/{img.FilePath}", $"{img.Id} by {Context.User.Mention}");
            }
            else await ReplyAsync($"no image `{name}` found. you can save one if you want by attaching an image with this command {_config["prefix"]}img save <name>");
        }


        [Command("list")]
        [Alias("ls")]
        [Priority(1)]
        [Summary("list all saved images")]
        public async Task ListImages() => await ReplyAsync($"here are the available images: \n```{string.Join(", ", db.Images.Select(i => i.Id))}```");


        [Command("save")]
        [Priority(1)]
        [Summary("save an image for later")]
        public async Task SaveImage([Summary("image name")] string name)
        {
            await Context.Channel.TriggerTypingAsync();
            var attached = Context.Message.Attachments;
            if (attached.Count != 1)
            {
                await ReplyAsync($"please attach 1 image");
                return;
            }
            var img = attached.First();

            (new WebClient { Proxy = null }).DownloadFile(img.Url, $"UploadedImages/{img.Filename}");

            if (db.Images.Any(i => i.Id == name))
                db.Images.Find(name).FilePath = img.Filename;
            else
                db.Images.Add(new Image { Id = name, FilePath = img.Filename });
            db.SaveChanges();

            await Context.Message.DeleteAsync();
            await ReplyAsync($"the image has been saved as {name}");
        }


        [Command("rm")]
        [Alias("del", "remove", "delete")]
        [Priority(1)]
        [Summary("delete a saved image")]
        public async Task DeleteImage([Summary("image name")] string name)
        {
            await Context.Channel.TriggerTypingAsync();
            if (db.Images.Any(i => i.Id == name))
            {
                File.Delete($"UploadedImages/{name}");
                db.Images.Remove(db.Images.Find(name));
                db.SaveChanges();
                await ReplyAsync($"{name} has been deleted");
            }
            else await ReplyAsync($"{name} didn't exist in the first place");
        }


    }
}
