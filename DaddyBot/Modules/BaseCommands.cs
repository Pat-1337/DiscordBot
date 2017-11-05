using Discord.Commands;
using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Daddy.Database;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
/*using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;//Install-Package Microsoft.CodeAnalysis.CSharp.Scripting //Microsoft.CodeAnalysis.Scripting
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.FxCopAnalyzers;*/
using System.Threading;
using System.Reactive.Linq;
using System.Reactive.Subjects;
//using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Reflection;

namespace Daddy.Modules
{
    public class BaseCommands : ModuleBase
    {
        public static Random _ran = new Random();
        public static List<ulong> noSend = new List<ulong>();
        public Dictionary<Commands, bool> inPerm = new Dictionary<Commands, bool>() { { Commands.Mute, true }, { Commands.Kick, true }, { Commands.Softban, true }, { Commands.Ban, true }, { Commands.Prune, true },
            { Commands.Iprune, true }, { Commands.Avatar, true }, { Commands.Channels, true }, { Commands.Info, true }, { Commands.Echo, false }, { Commands.Hug, true }, { Commands.Kiss, true }, { Commands.Laugh, true },
            { Commands.Punch, true }, { Commands.Slap, true }, { Commands.Pat, true }, { Commands.Pout, true }, { Commands.Cry, true }, { Commands.Rage, true }, { Commands.Nosebleed, true }, { Commands.Cat, true }, { Commands.Dog, true },
            { Commands.Bird, true }, { Commands.Help, true }, { Commands.Eval, false }, { Commands.Top, true }, { Commands.Normie, false }, { Commands.Boss, false }, { Commands.Airplane, true }, { Commands.Random, true }, { Commands.Hue, true },
            { Commands.Write, true }, { Commands.Welcome, false }, { Commands.Leave, false }, { Commands.Permissions, false }, { Commands.Discriminator, true } };
        public Dictionary<ulong, Dictionary<Commands, bool>> perm = new Dictionary<ulong, Dictionary<Commands, bool>>();

        public Dictionary<Settings, string> inWelcome = new Dictionary<Settings, string>() { { Settings.Welcome, "Join message" }, { Settings.Leave, "Leave message" }, { Settings.JoinRole, "N/A" }, { Settings.CharSpam, "7" }, { Settings.WordSpam, "4" }, { Settings.EmojiSpam, "7" }, { Settings.Prefix, "." } };
        public Dictionary<string, Dictionary<Settings, string>> welcome = new Dictionary<string, Dictionary<Settings, string>>();
        private IEnumerable<Type> _types = new Type[] { typeof(CommandService), typeof(Main.Daddy), typeof(Emoji), typeof(File), typeof(Discord.Rpc.DiscordRpcClient), typeof(DiscordSocketClient), typeof(IQueryable), typeof(Task), typeof(Process), typeof(Globals) };
        private static Color Success = new Color(52, 249, 59);
        private static Color Error = new Color(249, 52, 52);
        private static Color Warning = new Color(255, 230, 40);


        JSON jSon = new JSON();


        private static readonly string[] hugGifs = new string[] { "https://myanimelist.cdn-dena.com/s/common/uploaded_files/1461073447-335af6bf0909c799149e1596b7170475.gif",
            "http://gifimage.net/wp-content/uploads/2017/01/Anime-hug-GIF-Image-Download-20.gif", "https://m.popkey.co/fca5d5/bXDgV.gif", "https://media.giphy.com/media/l4FGza2kjFT3BJ3K8/giphy.gif",
            "https://media.giphy.com/media/3oKIPmUzjvyx1xCmru/giphy.gif", "https://i.giphy.com/143v0Z4767T15e.gif" };
        private static readonly string[] patGifs = new string[] { "https://m.popkey.co/a5cfaf/1x6lW.gif", "https://media.tenor.co/images/bf646b7164b76efe82502993ee530c78/tenor.gif",
            "https://68.media.tumblr.com/cc0451847fa08b202f4bd7a1cb9bd327/tumblr_o2js2xhINq1tydz8to1_500.gif", "https://media.tenor.co/images/68d981347bf6ee8c7d6b78f8a7fe3ccb/tenor.gif",
            "https://i.giphy.com/iGZJRDVEM6iOc.gif", "https://68.media.tumblr.com/71d93048022df065a1d2af96ab71afa3/tumblr_olykrec0DB1qbvovho1_500.gif" };
        private static readonly string[] punchGifs = new string[] { "https://i.giphy.com/10Im1VWMHQYfQI.gif", "https://s-media-cache-ak0.pinimg.com/originals/e6/f9/7c/e6f97cb321e8b8a0fed85195d47d7832.gif",
            "http://i3.kym-cdn.com/photos/images/original/001/117/646/bf9.gif", "https://media.giphy.com/media/iWAqMe8hBWKVq/giphy-downsized-large.gif", "https://i.giphy.com/LdsJrFnANh6HS.gif" };
        private static readonly string[] kissGifs = new string[] { "https://i.giphy.com/12VXIxKaIEarL2.gif", "https://media.tenor.co/images/802f7fa791471c2e33dc06475d2b54c8/tenor.gif",
            "https://i.giphy.com/QGc8RgRvMonFm.gif", "http://24.media.tumblr.com/dc0496ce48c1c33182f24b1535521af2/tumblr_mo77fusajy1spwngeo1_500.gif",
            "https://68.media.tumblr.com/d07fcdd5deb9d2cf1c8c44ffad04e274/tumblr_ok1kd5VJju1vlvf9to1_500.gif", "https://68.media.tumblr.com/60c27235f6440d9d6ebd8168bb75c384/tumblr_nxd3nn8iJ81rcikyeo1_500.gif" };
        private static readonly string[] plantGifs = new string[] { "http://i.giphy.com/E5QTH5mGZkzza.gif", "https://68.media.tumblr.com/b1a7f3c3019e2d0dcda18e456ad44cf6/tumblr_nm3vsdtXpu1sx3znro1_500.gif",
            "https://i.giphy.com/n91KooWNnPQ88.gif", "https://i.giphy.com/3g9eVrKlBhCy4.gif", "http://24.media.tumblr.com/a8035ab7b91730a76f8490a67e8079fc/tumblr_n4cd49lhzb1t11472o2_500.gif" };
        private static readonly string[] laughGifs = new string[] { "https://i.giphy.com/mnBsYB19OQCdy.gif", "http://i.imgur.com/5EMqe6Y.gif", "https://media.tenor.co/images/766b72ce843075ebe5a3d10841a3651b/tenor.gif",
            "https://s-media-cache-ak0.pinimg.com/originals/c0/7c/28/c07c28b2f57bc12ec07d947c8877bfe7.gif", "https://i.giphy.com/QKO4Fvdlrop6o.gif" };
        private static readonly string[] slapGifs = new string[] { "http://i0.kym-cdn.com/photos/images/newsfeed/000/940/326/086.gif", "https://31.media.tumblr.com/a6db390ca2a6daaf930cac40cfe85743/tumblr_n8m1dib4eP1tet3lvo1_500.gif",
            "https://i.giphy.com/LeTDEozJwatvW.gif", "https://i.giphy.com/Zau0yrl17uzdK.gif", "https://i.giphy.com/nesNeNkOb9Tz2.gif", "https://i.giphy.com/jLeyZWgtwgr2U.gif" };
        private static readonly string[] poutGifs = new string[] { "http://pa1.narvii.com/5805/6f8b4788caf1fc9f343ced4a93d43787a4022477_hq.gif", "https://myanimelist.cdn-dena.com/s/common/uploaded_files/1453266384-80b5e1f6f3080634c4692c0ca5a584f0.gif",
            "https://media.giphy.com/media/ZJ5IjUYXbC13y/giphy.gif", "https://s-media-cache-ak0.pinimg.com/originals/a0/c2/64/a0c264ad6b12b28d7c58871d7f5a999c.gif", "https://68.media.tumblr.com/9b5fe189479356b47c6fede60fe12576/tumblr_nr7vttUbjG1s21xzoo3_500.gif"};
        private static readonly string[] cryGifs = new string[] { "http://vignette3.wikia.nocookie.net/shigatsu-wa-kimi-no-uso/images/9/92/Tumblr_ndxgg75AmD1sgtx3io2_500.gif", "http://i.giphy.com/yarJ7WfdKiAkE.gif",
            "https://68.media.tumblr.com/7a62299e92bfbe10194ecf7850e0769c/tumblr_okckzleiEg1vh9ej2o1_500.gif", "https://68.media.tumblr.com/56fea5a4d682cd26178c17d80f7ee82a/tumblr_ofedni0ELT1vztiw8o1_500.gif", "https://68.media.tumblr.com/6ecacbb69194f280a98ec5b52af54adc/tumblr_nk4kui4YGF1uo1owvo1_500.gif" };
        private static readonly string[] angryGifs = new string[] { "https://media.giphy.com/media/RMUKZW6Wmy2mk/giphy.gif", "https://media.giphy.com/media/yFLSs5jbhUgeI/giphy.gif",
            "http://66.media.tumblr.com/5f9d9c003a73a78d5f2195d0b0f36f44/tumblr_oeedu4hsVP1vgf8i8o1_500.gif", "http://img1.liveinternet.ru/images/attach/c/5/87/401/87401949_tumblr_lo5ntp6FIl1qcff4ao1_500.gif", "https://media.tenor.co/images/386fb4996e952415422e4de3f7ff9273/tenor.gif" };
        private static readonly string[] catsGifs = new string[] { "http://i.imgur.com/VuItY0T.gif", "http://i.imgur.com/gv0eFnA.gif", "http://24.media.tumblr.com/416c1ae6f6d14c8e10bf24d293af7a34/tumblr_n2taenVhxf1rudcwro3_500.gif", "http://i.imgur.com/U0aCPSv.gif",
            "http://i.imgur.com/JKH5xBZ.gif", "http://i.imgur.com/sHQQJG5.gif", "http://i.imgur.com/vmExtY7.gif", "http://i.imgur.com/O6fLPBB.gif", "http://i.imgur.com/cgLcO8C.gif", "http://i.imgur.com/n6HzYFq.gif" };
        private static readonly string[] dogsGifs = new string[] { "http://post.barkbox.com/wp-content/uploads/2013/02/tumblr_mhdh3aaPE51r2afs6o1_500.gif", "http://post.barkbox.com/wp-content/uploads/2013/02/tumblr_mbl5larwCV1qdoqhwo1_500.gif",
            "http://post.barkbox.com/wp-content/uploads/2013/02/tumblr_mf39gmuHjZ1rccyxzo1_500.gif", "https://media.tenor.co/images/dc7ea17d880215c4ba2860c1a8a647d6/tenor.gif", "http://a2.cdn.whatstrending.com/post_items/images/000/023/971/original/tumblr_meo8moUf1Q1rkh2rbo1_500.gif",
            "http://1.bp.blogspot.com/--fUv9QV-rF8/VXB2zXKnBaI/AAAAAAAADWE/MWveEnnEkpI/s1600/funny-dogs-gif-picture-2015-17.gif", "http://post.barkbox.com/wp-content/uploads/2013/02/tumblr_mb4impeV181rudh26o1_500.gif",
            "https://media.giphy.com/media/100QWMdxQJzQC4/giphy.gif", "http://post.barkbox.com/wp-content/uploads/2013/02/tumblr_m4g6y8VrjG1qhq793o1_500.gif" };
        private static readonly string[] birdsGifs = new string[] { "https://media.giphy.com/media/OUuwn1HCfQjIY/giphy.gif", "https://media.giphy.com/media/4QaKX7FHE4Wqc/giphy.gif", "http://24.media.tumblr.com/cb38cff91b45f80dc493ae80ab8b181e/tumblr_n25qcoUHx71rvlx3so1_400.gif",
            "https://s-media-cache-ak0.pinimg.com/originals/1c/22/e0/1c22e0d4aac2b7ebd306e11519af58df.gif", "http://25.media.tumblr.com/tumblr_m7n0vkkChr1r2wrwho1_400.gif", "http://24.media.tumblr.com/b7ff23b167f2c9762e3a910d86f36cb9/tumblr_n0d0kfoy3y1rvlx3so2_r1_400.gif",
            "https://s-media-cache-ak0.pinimg.com/originals/51/c5/3e/51c53e5ed1ae0d880637cdb6ec70c5dd.gif", "https://media.giphy.com/media/JRQ0Y6hvMSCQw/giphy.gif" };
        private static readonly string[] nosebleedGifs = new string[] { "https://cdn.discordapp.com/attachments/306168780717817859/310790689896136705/giphy.gif", "https://cdn.discordapp.com/attachments/306168780717817859/310790853209751552/tumblr_nnlf8qSkMa1rdilz7o1_500.gif",
            "https://cdn.discordapp.com/attachments/306168780717817859/310790927025176576/tenor.gif", "https://cdn.discordapp.com/attachments/306168780717817859/310791083913379842/giphy.gif", "https://cdn.discordapp.com/attachments/306168780717817859/310791169372323840/giphy.gif", "https://cdn.discordapp.com/attachments/306168780717817859/310791367901052939/tumblr_opexdpY5O21vjzwpyo1_500.gif" };
        private static readonly string boss = "https://cdn.discordapp.com/attachments/290983102333976578/361283224128978974/634.png";
        private static readonly string[] airplane = new string[] { "https://lh3.googleusercontent.com/z31TVe9M2cR-uS9H4aPaZd7w8PnN_hxiq76nHbAhLmqZxfWsRHNBYpcQeEfd2Ot4Mdc=w300", "http://www.boeing.com/resources/boeingdotcom/commercial/assets/images/current-products/767.jpg", "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3e/FA-18_Hornet_VFA-41.jpg/300px-FA-18_Hornet_VFA-41.jpg",
            "https://sofrep.com/wp-content/uploads/2017/04/super-hornet-905x604.jpg", "https://i2.wp.com/fightersweep.com/wp-content/uploads/2015/12/AJ400_F18_NTU_SEPT06_P1FS.jpg?resize=630%2C418&ssl=1", "https://upload.wikimedia.org/wikipedia/commons/thumb/a/ab/AC-130H_Spectre_jettisons_flares.jpg/1200px-AC-130H_Spectre_jettisons_flares.jpg",
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRsR4dS3xvNcFjY4DpFwPeykfGQ5LxTM-t0rCNvRTaQE9vZ_J3Z", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTFaonnn869i8t8jehy6pcX1V-2Lj_ogzy-Le_b1_6uqewS5taM9w", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRLbGIP7aOJRqJDsWVoGl-g7iiOjRBK3Coiqxrq2CVpWFKhCbBW_g",
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ8UCkX6N2pxe7RKIEeq9gmxLmpECw7fOp2ckIRjPhZQzyHEvYKEQ", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQxvWSbzvSd4LbooCRQNbAKCg6kBsy1PmCo1tnlAeWWX0puGfHY", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS95s6AqYA_QQgUXNBGNbCGHOzLN3se7SccvNyMw1elk8o7unfPSA",
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS_VAcgnFJgD1CqUnad_HQY5UdIkj2ycgf3G1PZ9VkBPKYeiXNZ", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRrULLtO2Hv3Ot8AzwUXr1hqF3ELcHPVhlT6717zjiD5NPRCNub", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRRe3tUPsJNQr1dbd1Y5W-B_u1QFWIxBJWdRl4TjE9nzAsAec8kcw",
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcShYKIFg1-zNMn5PTdDUCrrcfkCqBVQImdD_vslqoM2utGA5MfV", "http://s.newsweek.com/sites/www.newsweek.com/files/styles/lg/public/2014/09/20/f22.jpg" };

        [Command("shutdown", RunMode = RunMode.Async), Summary("Stops the bot")]
        public async Task Shutdown()
        {
            if (SpecialPeople(Context.User.Id))
            {
                await Context.Message.DeleteAsync();
                await Context.Channel.SendMessageAsync("Eval shutdown! Bye bye butterfly");
                Main.Daddy._instance.Stop();
                //Main.Daddy._map.Get<Main.Daddy>().Stop();
                await Context.Client.StopAsync();
                await Task.CompletedTask;
            }
        }

        [Command("profile", RunMode = RunMode.Async), Alias("pfp"), Summary("Changes bot picture")]
        public async Task pfp(string URL)
        {
            if (SpecialPeople(Context.User.Id))
            {
                await Context.Message.DeleteAsync();
                await Main.Daddy._client.CurrentUser.ModifyAsync(new Action<SelfUserProperties>((SelfUserProperties x) => x.Avatar = new Image(GetStreamFromUrl(URL))));
            }
            else
            {
                await Context.Channel.SendMessageAsync($"This command requires `BOT_OWNER` - you have `{getHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
            }
        }

        [Command("hue", RunMode = RunMode.Async), Summary("Hueansohn")]
        public async Task ColoredMessage(float hue, float sat, float lit, string title, [Remainder]string text)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Hue))
            {
                await Context.Message.DeleteAsync();
                var sw = Stopwatch.StartNew();
                var color = CustomColor.FromHSL(hue, sat, lit);
                //var (r, g, b) = (color.R, color.G, color.B);
                var embedbuilder = new EmbedBuilder().WithColor(color).WithDescription(text).WithTitle(title);
                sw.Stop();
                await ReplyAsync("", embed: embedbuilder.WithFooter(x => x.WithText($"In {sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("echo", RunMode = RunMode.Async)]
        public async Task Echo([Summary("Message you want to repeat back."), Remainder()]string arg)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Echo))
            {
                var sw = Stopwatch.StartNew();
                await Context.Message.DeleteAsync();
                if (string.IsNullOrEmpty(arg))
                {
                    return;
                }
                else
                {
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = "Echo command",
                        Description = arg,
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    };
                    builder.WithCurrentTimestamp();
                    sw.Stop();
                    await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("type", RunMode = RunMode.Async), Alias("write"), Summary("typewritermode"), Ext.Ratelimit(1, 1, Ext.Measure.Minutes)]
        public async Task TypeWriter([Remainder]string text = null)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Write))
            {
                await Context.Message.DeleteAsync();
                if (text.Split(' ').Length > 10)
                {
                    await Context.Channel.SendMessageAsync($"Max words: `10`");
                    return;
                }
                await Context.Channel.SendMessageAsync("…");
                await Task.Delay(1000);
                IMessage msg = (await Context.Channel.GetMessagesAsync(Context.Message, Direction.After, 1).Flatten()).Where(x => x.Author.Id == Main.Daddy._client.CurrentUser.Id).Take(1).ToList().First();
                string message = string.Empty;
                if (text == null) return;
                foreach (var c in removeAllMention(text).Split(' '))
                {
                    if (c == null || string.IsNullOrEmpty(c))
                    {
                        continue;
                    }
                    await Task.Delay(1000);
                    await (msg as IUserMessage).ModifyAsync(x =>
                    {
                        message += $"{c} ";
                        x.Content = message;
                    });
                    if (msg.Content.Equals("…") || string.IsNullOrEmpty(msg.Content))
                    {
                        break;
                    }
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("ev", RunMode = RunMode.Async), Summary("Evaluate code")]
        public async Task Eval([Summary("Evaluating string"), Remainder()]string arg)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Eval))
            {
                if (SpecialPeople(Context.User.Id))
                {
                    await _Eval(arg, false);
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"This command requires `BOT_OWNER` - you have `{getHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("Mev", RunMode = RunMode.Async), Summary("Evaluate code")]
        public async Task JsonEval([Remainder] string code)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Eval))
            {
                if (SpecialPeople(Context.User.Id))
                {
                    await _Eval(code, true, true);
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"This command requires `BOT_OWNER` - you have `{getHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("?!Mev", RunMode = RunMode.Async), Summary("Evaluate code")]
        public async Task SingleStaticEval(string methodName, [Remainder] string code)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Eval))
            {
                if (SpecialPeople(Context.User.Id))
                {
                    await _Eval(code, true, true, true, methodName);
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"This command requires `BOT_OWNER` - you have `{getHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("?Mev", RunMode = RunMode.Async), Summary("Evaluate code")]
        public async Task SingleEval(string methodName, [Remainder] string code)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Eval))
            {
                if (SpecialPeople(Context.User.Id))
                {
                    await _Eval(code, true, true, methodName: methodName);
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"This command requires `BOT_OWNER` - you have `{getHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }


        [Command("!Mev", RunMode = RunMode.Async), Summary("Evaluate code")]
        public async Task StaticEval([Remainder] string code)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Eval))
            {
                if (SpecialPeople(Context.User.Id))
                {
                    await _Eval(code, true, true, true);
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"This command requires `BOT_OWNER` - you have `{getHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("Tev", RunMode = RunMode.Async), Summary("Evaluate code")]
        public async Task MEval([Remainder] string code)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Eval))
            {
                if (SpecialPeople(Context.User.Id))
                {
                    await _Eval(code, true);
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"This command requires `BOT_OWNER` - you have `{getHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("?Tev", RunMode = RunMode.Async), Summary("Evaluate code")]
        public async Task FiltJEval(string name, [Remainder] string code)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Eval))
            {
                if (SpecialPeople(Context.User.Id))
                {
                    await _Eval(code, true, methodName: name);
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"This command requires `BOT_OWNER` - you have `{getHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("?ev", RunMode = RunMode.Async), Summary("Evaluate code")]
        public async Task FiltEval(string name, [Remainder] string code)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Eval))
            {
                if (SpecialPeople(Context.User.Id))
                {
                    await _Eval(code, false, methodName: name);
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"This command requires `BOT_OWNER` - you have `{getHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("json", RunMode = RunMode.Async), Alias("reload", "load")]
        public async Task JSON()
        {
            if (SpecialPeople(Context.User.Id))
            {
                await Context.Message.DeleteAsync();
                createJson(Context.Guild);
                createSettings(Context.Guild);
            }
            else
            {
                await Context.Channel.SendMessageAsync($"This command requires `BOT_OWNER` -> you have `{getHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
            }
        }

        [Command("role", RunMode = RunMode.Async)]
        public async Task SetRole([Summary("role")]string arg)
        {
            await Context.Message.DeleteAsync();
            if (isVip(Context.User as SocketUser) && (Context.User as SocketGuildUser).GuildPermissions.Administrator)
            {
                if (!doesExistSettings(Context.Guild)) { createSettings(Context.Guild); }
                JObject rss = JObject.Parse(File.ReadAllText($@"Settings\settings_{Context.Guild.Id}.json"));
                JObject welcome = (JObject)rss["Settings"];
                welcome["JoinRole"] = arg;
                File.WriteAllText($@"Settings\settings_{Context.Guild.Id}.json", rss.ToString());
                await Context.Channel.SendMessageAsync($"New default join role is: `{arg}`");
            }
        }

        [Command("schar", RunMode = RunMode.Async)]
        public async Task antiCharSpam([Summary("char spam")]string arg)
        {
            if (isVip(Context.User as SocketUser))
            {
                await Context.Message.DeleteAsync();
                if (!doesExistSettings(Context.Guild)) { createSettings(Context.Guild); }
                JObject rss = JObject.Parse(File.ReadAllText($@"Settings\settings_{Context.Guild.Id}.json"));
                JObject welcome = (JObject)rss["Settings"];
                welcome["CharSpam"] = arg;
                File.WriteAllText($@"Settings\settings_{Context.Guild.Id}.json", rss.ToString());
                await Context.Channel.SendMessageAsync($"New message char spam trigger is: `{arg}`");
            }
        }

        [Command("sword", RunMode = RunMode.Async)]
        public async Task antiWordSpam([Summary("word spam"), Remainder()]string arg)
        {
            if (isVip(Context.User as SocketUser) && !string.IsNullOrEmpty(jSon.Settings(Context.Guild, Settings.WordSpam)))
            {
                await Context.Message.DeleteAsync();
                if (!doesExistSettings(Context.Guild)) { createSettings(Context.Guild); }
                JObject rss = JObject.Parse(File.ReadAllText($@"Settings\settings_{Context.Guild.Id}.json"));
                JObject welcome = (JObject)rss["Settings"];
                welcome["WordSpam"] = arg;
                File.WriteAllText($@"Settings\settings_{Context.Guild.Id}.json", rss.ToString());
                await Context.Channel.SendMessageAsync($"New message word spam trigger is: `{arg}`");
            }
        }

        [Command("semoji", RunMode = RunMode.Async)]
        public async Task antiEmojiSpam([Summary("word spam"), Remainder()]string arg)
        {
            if (isVip(Context.User as SocketUser) && !string.IsNullOrEmpty(jSon.Settings(Context.Guild, Settings.WordSpam)))
            {
                await Context.Message.DeleteAsync();
                if (!doesExistSettings(Context.Guild)) { createSettings(Context.Guild); }
                JObject rss = JObject.Parse(File.ReadAllText($@"Settings\settings_{Context.Guild.Id}.json"));
                JObject welcome = (JObject)rss["Settings"];
                welcome["EmojiSpam"] = arg;
                File.WriteAllText($@"Settings\settings_{Context.Guild.Id}.json", rss.ToString());
                await Context.Channel.SendMessageAsync($"New message emoji spam trigger is: `{arg}`");
            }
        }

        [Command("welcome", RunMode = RunMode.Async)]
        public async Task Welcome([Summary("msg"), Remainder()]string arg)
        {
            if (isVip(Context.User as SocketUser) && !string.IsNullOrEmpty(jSon.Settings(Context.Guild, Settings.Welcome)))
            {
                await Context.Message.DeleteAsync();
                if (!doesExistSettings(Context.Guild)) { createSettings(Context.Guild); }
                JObject rss = JObject.Parse(File.ReadAllText($@"Settings\settings_{Context.Guild.Id}.json"));
                JObject welcome = (JObject)rss["Settings"];
                welcome["Welcome"] = arg;
                File.WriteAllText($@"Settings\settings_{Context.Guild.Id}.json", rss.ToString());
                await Context.Channel.SendMessageAsync($"New welcome messages is: `{arg}`");
            }
        }

        [Command("leave", RunMode = RunMode.Async)]
        public async Task Leave([Summary("msg"), Remainder()]string arg)
        {
            if (isVip(Context.User as SocketUser) && !string.IsNullOrEmpty(jSon.Settings(Context.Guild, Settings.Leave)))
            {
                await Context.Message.DeleteAsync();
                if (!doesExistSettings(Context.Guild)) { createSettings(Context.Guild); }
                JObject rss = JObject.Parse(File.ReadAllText($@"Settings\settings_{Context.Guild.Id}.json"));
                JObject welcome = (JObject)rss["Settings"];
                welcome["Leave"] = arg;
                File.WriteAllText($@"Settings\settings_{Context.Guild.Id}.json", rss.ToString());
                await Context.Channel.SendMessageAsync($"New leave message is: `{arg}`");
            }
        }

        [Command("prefix", RunMode = RunMode.Async)]
        public async Task Prefix([Summary("prefix"), Remainder()]char arg)
        {
            if (isVip(Context.User as SocketUser) && (Context.User as SocketGuildUser).GuildPermissions.Administrator && !string.IsNullOrEmpty(jSon.Settings(Context.Guild, Settings.Prefix)))
            {
                await Context.Message.DeleteAsync();
                if (Convert.ToChar(jSon.Settings(Context.Guild, Settings.Prefix)).Equals(arg))
                {
                    await Context.Channel.SendMessageAsync($"`{arg}` is already server prefix!");
                    return;
                }
                if (!doesExistSettings(Context.Guild)) { createSettings(Context.Guild); }
                JObject rss = JObject.Parse(File.ReadAllText($@"Settings\settings_{Context.Guild.Id}.json"));
                JObject welcome = (JObject)rss["Settings"];
                welcome["Prefix"] = arg.ToString();
                File.WriteAllText($@"Settings\settings_{Context.Guild.Id}.json", rss.ToString());
                await Context.Channel.SendMessageAsync($"New server prefix is: `{arg}`");
            }
        }

        [Command("cenable", RunMode = RunMode.Async), Alias("callow")]
        public async Task cEnable([Summary("command")]string arg)
        {
            if (isVip(Context.User as SocketUser) && (Context.User as SocketGuildUser).GuildPermissions.ManageRoles)
            {
                await Context.Message.DeleteAsync();
                if (Enum.IsDefined(typeof(Commands), FirstCharToUpper(arg)))
                {
                    if (!doesExistJson(Context.Guild)) { createJson(Context.Guild); }
                    JObject rss = JObject.Parse(File.ReadAllText($@"json\jayson_{Context.Guild.Id}.json"));
                    JObject channel = (JObject)rss[Context.Channel.Id];
                    channel[FirstCharToUpper(arg)] = true;
                    File.WriteAllText($@"json\jayson_{Context.Guild.Id}.json", rss.ToString());
                    await Context.Channel.SendMessageAsync($"{FirstCharToUpper(arg)} is enabled!");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"`{FirstCharToUpper(arg)}` doesn't exist!");
                }
            }
        }

        [Command("enable", RunMode = RunMode.Async), Alias("allow")]
        public async Task Enable([Summary("command")]string arg)
        {
            if (isVip(Context.User as SocketUser) && (Context.User as SocketGuildUser).GuildPermissions.ManageRoles)
            {
                await Context.Message.DeleteAsync();
                if (Enum.IsDefined(typeof(Commands), FirstCharToUpper(arg)))
                {
                    if (!doesExistJson(Context.Guild)) { createJson(Context.Guild); }
                    JObject rss = JObject.Parse(File.ReadAllText($@"json\jayson_{Context.Guild.Id}.json"));
                    foreach (var x in rss)
                    {
                        x.Value[FirstCharToUpper(arg)] = true;
                    }
                    File.WriteAllText($@"json\jayson_{Context.Guild.Id}.json", rss.ToString());
                    await Context.Channel.SendMessageAsync($"{FirstCharToUpper(arg)} is enabled!");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"`{FirstCharToUpper(arg)}` doesn't exist!");
                }
            }
        }

        [Command("cdisable", RunMode = RunMode.Async), Alias("cforbid")]
        public async Task cDisable([Summary("command")]string arg)
        {
            if (isVip(Context.User as SocketUser) && (Context.User as SocketGuildUser).GuildPermissions.ManageRoles)
            {
                await Context.Message.DeleteAsync();
                if (Enum.IsDefined(typeof(Commands), FirstCharToUpper(arg)))
                {
                    if (!doesExistJson(Context.Guild)) { createJson(Context.Guild); }
                    JObject rss = JObject.Parse($"{File.ReadAllText($@"json\jayson_{Context.Guild.Id}.json")}");
                    JObject channel = (JObject)rss[Context.Channel.Id];
                    channel[FirstCharToUpper(arg)] = false;
                    File.WriteAllText($@"json\jayson_{Context.Guild.Id}.json", rss.ToString());
                    await Context.Channel.SendMessageAsync($"{FirstCharToUpper(arg)} is disabled!");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"`{FirstCharToUpper(arg)}` doesn't exist in json file");
                }
            }
        }

        [Command("disable", RunMode = RunMode.Async), Alias("forbid")]
        public async Task Disable([Summary("command")]string arg)
        {
            if (isVip(Context.User as SocketUser) && (Context.User as SocketGuildUser).GuildPermissions.ManageRoles)
            {
                await Context.Message.DeleteAsync();
                if (Enum.IsDefined(typeof(Commands), FirstCharToUpper(arg)))
                {
                    if (!doesExistJson(Context.Guild)) { createJson(Context.Guild); }
                    JObject rss = JObject.Parse($"{File.ReadAllText($@"json\jayson_{Context.Guild.Id}.json")}");
                    foreach (var x in rss)
                    {
                        x.Value[FirstCharToUpper(arg)] = false;
                    }
                    File.WriteAllText($@"json\jayson_{Context.Guild.Id}.json", rss.ToString());
                    await Context.Channel.SendMessageAsync($"{FirstCharToUpper(arg)} is disabled!");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"`{FirstCharToUpper(arg)}` doesn't exist in json file");
                }
            }
        }

        [Command("gjson", RunMode = RunMode.Async)]
        public async Task gjson([Summary("argument"), Remainder()]string arg)
        {
            if (SpecialPeople(Context.User.Id))
            {
                string[] a = arg.Split(new string[] { "->" }, StringSplitOptions.None);
                JObject rss = null;
                try
                {
                    rss = JObject.Parse($"{File.ReadAllText($@"{a[0]}\{a[1]}_{Context.Guild.Id}.json")}");
                }
                catch
                {
                    await Context.Channel.SendMessageAsync($"Doesn't exist!\narg0: `{a[0]}`\narg1: `{a[1]}`\narg2: `{a[2]}`\n");
                    return;
                }
                if (a.Length.Equals(3))
                {
                    await Context.Channel.SendMessageAsync($"```json\n{rss[a[2]]}```");
                    return;
                }
                else
                {
                    foreach (var x in rss)
                    {
                        await Context.Channel.SendMessageAsync($"```json\n{x.Key}\n{x.Value}```");
                    }
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync($"This command requires `BOT_OWNER` - you have `{getHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
            }
        }

        [Command("eval", RunMode = RunMode.Async)]
        public async Task fEval([Summary("argument"), Remainder()]string arg)
        {
            if (SpecialPeople(Context.User.Id))
            {
                string[] a = arg.Split(new string[] { "->" }, StringSplitOptions.None);
                object[] args = a.Skip(3).Take(a.Length - 3).ToArray();
                if (a.Length <= 3)
                {
                    args = null;
                }
                if (a[0].Equals("d"))
                {
                    Main.Daddy _d = new Main.Daddy();
                    if (a[1].Equals("r"))
                    {
                        await Context.Channel.SendMessageAsync($"{typeof(Main.Daddy).GetMethod(a[2]).Invoke(_d, args)}");
                    }
                    else if (a[1].Equals("e"))
                    {
                        //typeof(Main.Daddy).GetMethod(a[2]).Invoke(_d, args);
                        //await ExtensionMethods.InvokeAsync(typeof(Main.Daddy).GetMethod(a[2]), _d, args);
                    }
                }
                else if (a[0].Equals("bc"))
                {
                    BaseCommands _bc = new BaseCommands();
                    if (a[1].Equals("r"))
                    {
                        await Context.Channel.SendMessageAsync($"{typeof(BaseCommands).GetMethod(a[2]).Invoke(_bc, args)}");
                    }
                    else if (a[1].Equals("a"))
                    {
                        switch (a[2])
                        {
                            case "Info":
                                await Task.Run(Info);
                                break;
                            case "Cat":
                                await Task.Run(Cat);
                                break;
                            case "Dog":
                                await Task.Run(Dog);
                                break;
                            case "Bird":
                                await Task.Run(Bird);
                                break;
                            case "Help":
                                await Task.Run(Help);
                                break;
                            default:
                                return;
                        }
                        //typeof(BaseCommands).GetMethod(a[2]).Invoke(_bc, args);
                        //await ExtensionMethods.InvokeAsync(typeof(BaseCommands).GetMethod(a[2]), _bc, args);
                        //await (Task)typeof(BaseCommands).GetTypeInfo().GetDeclaredMethod(a[2]).Invoke(_bc, args);
                        //await (dynamic)typeof(BaseCommands).GetTypeInfo().GetDeclaredMethod(a[2]).Invoke(_bc, args);
                    }
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync($"This command requires `BOT_OWNER` - you have `{getHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
            }
        }

        public string getText(object text)
        {
            return text.ToString();
        }

        [Command("info", RunMode = RunMode.Async), Summary("Gives some info about server")]
        public async Task Info()
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Info))
            {
                var sw = Stopwatch.StartNew();
                EmbedFieldBuilder field1 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "Created at",
                    Value = $"{Context.Guild.CreatedAt.Day}/{Context.Guild.CreatedAt.Month}/{Context.Guild.CreatedAt.Year} "
                };
                EmbedFieldBuilder field2 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "Owner",
                    Value = Context.Guild.GetOwnerAsync().GetAwaiter().GetResult()
                };
                EmbedFieldBuilder field3 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "Region",
                    Value = Context.Guild.VoiceRegionId
                };
                EmbedFieldBuilder field4 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "Available roles",
                    Value = ((SocketGuild)Context.Guild).Roles.Count
                };
                EmbedFieldBuilder field5 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "Members",
                    Value = ((SocketGuild)Context.Guild).MemberCount//new List<IGuildUser>(await Context.Guild.GetUsersAsync()).Count()
                };
                EmbedFieldBuilder field6 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "Channels",
                    Value = $"{((SocketGuild)Context.Guild).Channels.Count}  [{((SocketGuild)Context.Guild).VoiceChannels.Count} :microphone2:]"//new List<IGuildChannel>(await Context.Guild.GetChannelsAsync()).Count()
                };
                EmbedAuthorBuilder author = new EmbedAuthorBuilder()
                {
                    Name = "DaddyBot",
                    IconUrl = "http://i.imgur.com/YLpktWj.png"
                };
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Author = author,
                    Title = $"{Context.Guild.Name}'s informations.",
                    Description = $"Bot made by: [Pat](https://steamcommunity.com/id/0xPat/)\nBot made for: [M&D](https://discord.gg/dFKerFY)\nBot created: {Main.Daddy._client.CurrentUser.CreatedAt}",
                    ThumbnailUrl = Context.Guild.IconUrl,
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                };
                embed.AddField(field1);
                embed.AddField(field2);
                embed.AddField(field3);
                embed.AddField(field4);
                embed.AddField(field5);
                embed.AddField(field6);
                embed.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: embed.WithFooter(x => x.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("uinfo", RunMode = RunMode.Async), Alias("dox", "whois", "stats"), Summary("Gives some info about user")]
        public async Task userInfo([Remainder()]IGuildUser user = null)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Info))
            {
                if (user == null) { user = (IGuildUser)Context.User; }
                var sw = Stopwatch.StartNew();
                EmbedFieldBuilder field1 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "ID",
                    Value = user.Id
                };
                EmbedFieldBuilder field2 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "Nickname",
                    Value = $"{(string.IsNullOrEmpty(user.Nickname) ? "None" : user.Nickname)}"
                };
                EmbedFieldBuilder field3 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "Status",
                    Value = user.Status
                };
                EmbedFieldBuilder field4 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "Game",
                    Value = $"{(user.Game.HasValue ? user.Game.Value.ToString() : "None")}"
                };
                EmbedFieldBuilder field5 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "Joined",
                    Value = $"{(user.JoinedAt.HasValue ? $"{user.JoinedAt.Value.Day}/{user.JoinedAt.Value.Month}/{user.JoinedAt.Value.Year} [D/M/Y]" : "NULL")}"
                };
                EmbedFieldBuilder field6 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "Registered",
                    Value = $"{user.CreatedAt.Day}/{user.CreatedAt.Month}/{user.CreatedAt.Year} [D/M/Y]"
                };
                EmbedAuthorBuilder author = new EmbedAuthorBuilder()
                {
                    Name = user.Username,
                    IconUrl = user.GetAvatarUrl()
                };
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Author = author,
                    Title = $"{user.Username}'s informations.",
                    Description = $"[Profile picture]({user.GetAvatarUrl(ImageFormat.Auto, (ushort)256)})",
                    ThumbnailUrl = user.GetAvatarUrl(ImageFormat.Auto, (ushort)256),
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                };
                embed.AddField(field1);
                embed.AddField(field2);
                embed.AddField(field3);
                embed.AddField(field4);
                embed.AddField(field5);
                embed.AddField(field6);
                embed.WithThumbnailUrl(user.GetAvatarUrl(ImageFormat.Auto, (ushort)256));
                embed.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: embed.WithFooter(x => x.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("help", RunMode = RunMode.Async), Summary("A list of cmds.")]
        public async Task Help()
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Help))
            {
                var sw = Stopwatch.StartNew();
                EmbedAuthorBuilder author = new EmbedAuthorBuilder()
                {
                    Name = "DaddyBot",
                    IconUrl = "http://i.imgur.com/YLpktWj.png"
                };
                EmbedBuilder em = new EmbedBuilder()
                {
                    Author = author,
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                };
                EmbedFieldBuilder cmds = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "Commands",
                    Value = "echo \nhug \nkiss \npat \npunch \nslap \nlaugh \nplant \ninfo \navatar \nchannels \n[ADMIN] \nkick \nban \nsoftban \nallow \nforbid \nmute \nprune"
                };
                EmbedFieldBuilder desc = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "Description",
                    Value = "Makes the bot say something. \nHugs someone. \nKisses someone. \nPats someone. \nPunches someone. \nSlaps someone. \nLaughs at someone. \nSends a random picture of plants. \nGives some informations about the server. \nSends someone's profile picture. \nGives channels names and their IDs. \n \nKicks someone. \nBans someone. \nBans someone and deletes all their msgs then unban them. \nAllows using a command. \nForbids using a command. \nMutes someone. \nDeletes a specific amount of msgs/pictures."
                };
                em.AddField(cmds);
                em.AddField(desc);
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} Please check your DM for a list of commands!");
                IDMChannel x = await (Context.User as SocketUser).GetOrCreateDMChannelAsync();
                em.WithCurrentTimestamp();
                sw.Stop();
                await x.SendMessageAsync(string.Empty, false, embed: em);
                await Context.Message.DeleteAsync();
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("hug", RunMode = RunMode.Async), Summary("Hugs a person.")]
        public async Task Hug([Summary("Hugged person.")]IUser arg = null)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Hug))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder();
                if (arg != null)
                {
                    builder.Title = $"{Context.User.Username} hugs {arg.Username}!";
                }
                else
                {
                    builder.Title = $"{Context.Client.CurrentUser.Username} hugs {Context.User.Username}! Cute and cuddly!";
                }
                builder.WithImageUrl(hugGifs[_ran.Next(hugGifs.Length)]);
                builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                builder.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("pat", RunMode = RunMode.Async), Summary("Pats a person.")]
        public async Task Pat([Summary("Patted person.")]IUser arg)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Pat))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder();
                if (arg != null)
                {
                    if (!SpecialPeople(arg.Id))
                    {
                        builder.Title = $"{Context.User.Username} pats {arg.Username}! Awww, cute!";
                        builder.WithImageUrl(patGifs[_ran.Next(patGifs.Length)]);
                    }
                    else
                    {
                        builder.Title = $"no.";
                        builder.WithImageUrl("http://i.imgur.com/F0nMzoJ.gif");
                    }
                }
                else
                {
                    builder.Title = $"{Context.Client.CurrentUser.Username} pats {Context.User.Username}! Awww, cute!";
                    builder.WithImageUrl(patGifs[_ran.Next(patGifs.Length)]);
                }
                builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                builder.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("kiss", RunMode = RunMode.Async), Summary("Kisses a person.")]
        public async Task Kiss([Summary("Kissed person.")]IUser arg = null)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Kiss))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder();
                if (arg != null)
                {
                    builder.Title = $"{Context.User.Username} kissed {arg.Username}! >///< :hearts:";
                }
                else
                {
                    builder.Title = $"{Context.Client.CurrentUser.Username} kissed {Context.User.Username}! :hearts:";
                }
                builder.WithImageUrl(kissGifs[_ran.Next(kissGifs.Length)]);
                builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                builder.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("laugh", RunMode = RunMode.Async), Summary("Laugh at person.")]
        public async Task Laugh([Summary("User")]IUser arg = null)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Laugh))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder();
                if (arg != null)
                {
                    builder.Title = $"{Context.User.Username} laughs at {arg.Username}! HA-HA!";
                }
                else
                {
                    builder.Title = $"{Context.Client.CurrentUser.Username} laughs at {Context.User.Username}! HA-HA!";
                }
                builder.WithImageUrl(laughGifs[_ran.Next(laughGifs.Length)]);
                builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                builder.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("nosebleed", RunMode = RunMode.Async)]
        public async Task Nosebleed()
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Nosebleed))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder()
                {
                    Title = $"(꒪ཀ꒪)",
                    ImageUrl = nosebleedGifs[_ran.Next(nosebleedGifs.Length)],
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                };
                builder.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("airplane", RunMode = RunMode.Async), Alias("plane")]
        public async Task Airplane()
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Airplane))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder()
                {
                    Title = $"ask Justino.",
                    ImageUrl = airplane[_ran.Next(airplane.Length)],
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                };
                builder.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("boss", RunMode = RunMode.Async)]
        public async Task Boss()
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Boss))
            {
                await Context.Message.DeleteAsync();
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder()
                {
                    Title = $"88",
                    ImageUrl = boss,
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                };
                builder.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("punch", RunMode = RunMode.Async), Summary("Punches a person.")]
        public async Task Punch([Summary("Punched person")]IUser arg)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Punch))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder();
                if (arg != null)
                {
                    if (!SpecialPeople(arg.Id))
                    {
                        builder.Title = $"{Context.User.Username} punches {arg.Username}! Ouch, that hurts!";
                        builder.WithImageUrl(punchGifs[_ran.Next(punchGifs.Length)]);
                    }
                    else
                    {
                        builder.Title = $"no.";
                        builder.WithImageUrl("http://i.imgur.com/F0nMzoJ.gif");
                    }
                }
                else
                {
                    builder.Title = $"{Context.Client.CurrentUser.Username} punches {Context.User.Username}! Ouch, that hurts!";
                    builder.WithImageUrl(punchGifs[_ran.Next(punchGifs.Length)]);
                }
                builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                builder.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("slap", RunMode = RunMode.Async), Summary("Punches a person.")]
        public async Task Slap([Summary("Punched person")]IUser arg)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Slap))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder();
                if (arg != null)
                {
                    if (!SpecialPeople(arg.Id))
                    {
                        builder.Title = $"{Context.User.Username} slaps {arg.Username}! SLAP!";
                        builder.WithImageUrl(slapGifs[_ran.Next(slapGifs.Length)]);
                    }
                    else
                    {
                        builder.Title = $"no.";
                        builder.WithImageUrl("http://i.imgur.com/F0nMzoJ.gif");
                    }
                }
                else
                {
                    builder.Title = $"{Context.Client.CurrentUser.Username} slaps {Context.User.Username}! SLAP!";
                    builder.WithImageUrl(slapGifs[_ran.Next(slapGifs.Length)]);
                }
                builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                builder.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("pout", RunMode = RunMode.Async), Summary("Pout")]
        public async Task Pout()
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Pout))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder()
                {
                    Title = $"{Context.User.Username} pouts!",
                    ImageUrl = poutGifs[_ran.Next(poutGifs.Length)],
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255))),
                };
                builder.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("cry", RunMode = RunMode.Async), Summary(":'(")]
        public async Task Cry([Summary("IUser")]IUser arg = null)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Cry))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder();
                if (arg != null)
                {
                    builder.Title = $"{Context.User.Username}'s cries on {arg.Username} shoulder! :cry:";
                }
                else
                {
                    builder.Title = $"{Context.User.Username} is crying! What happened?!";
                }
                builder.WithImageUrl(cryGifs[_ran.Next(cryGifs.Length)]);
                builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                builder.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("rage", RunMode = RunMode.Async), Alias("angry"), Summary("Kisses a person.")]
        public async Task Rage([Summary("Kissed person.")]IUser arg = null)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Rage))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder();
                if (arg != null)
                {
                    builder.Title = $"{Context.User.Username} is angry at {arg.Username}! :rage:";
                }
                else
                {
                    builder.Title = $"{Context.User.Username} is filled with rage!";
                }
                builder.WithImageUrl(angryGifs[_ran.Next(angryGifs.Length)]);
                builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                builder.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("cat", RunMode = RunMode.Async), Alias("pussy")]
        public async Task Cat()
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Cat))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder()
                {
                    Title = $":cat:",
                    ImageUrl = catsGifs[_ran.Next(catsGifs.Length)],
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                };
                builder.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("dog", RunMode = RunMode.Async), Alias("doggo")]
        public async Task Dog()
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Dog))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder()
                {
                    Title = $":dog:",
                    ImageUrl = dogsGifs[_ran.Next(dogsGifs.Length)],
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                };
                builder.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("bird", RunMode = RunMode.Async), Alias("birb")]
        public async Task Bird()
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Bird))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder()
                {
                    Title = $":bird:",
                    ImageUrl = birdsGifs[_ran.Next(birdsGifs.Length)],
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                };
                builder.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("avatar", RunMode = RunMode.Async), Alias("pfp"), Summary("Gets avatar.")]
        public async Task Avatar([Summary("User")]IUser arg = null)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Avatar))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder();
                if (arg != null)
                {
                    builder.Title = arg.Username;
                    builder.WithImageUrl(arg.GetAvatarUrl(ImageFormat.Auto, (ushort)256));//256
                }
                else
                {
                    builder.Title = Context.User.Username;
                    builder.WithImageUrl(Context.User.GetAvatarUrl(ImageFormat.Auto, (ushort)256));
                }
                builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                builder.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                await Context.Message.DeleteAsync();
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("channels", RunMode = RunMode.Async), Alias("channel"), Summary("Gets channel IDs.")]
        public async Task Channels([Summary("m/v")]string type = null)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Channels))
            {
                var sw = Stopwatch.StartNew();
                StringBuilder str0 = new StringBuilder();
                StringBuilder str1 = new StringBuilder();
                IEnumerable<IGuildChannel> channels = null;
                if (string.IsNullOrEmpty(type) || type.ToLower().Equals("t"))
                {
                    channels = (await Context.Guild.GetChannelsAsync()).Where(x => x is SocketTextChannel).ToList().OrderBy(x => x.Position);
                }
                else if (type.ToLower().Equals("v"))
                {
                    channels = (await Context.Guild.GetChannelsAsync()).Where(x => x is SocketVoiceChannel).ToList().OrderBy(x => x.Position);
                }
                channels.ToList().ForEach(x => str0.Append($"{x.Name}\n"));
                channels.ToList().ForEach(x => str1.Append($"{x.Id}\n"));
                EmbedFieldBuilder field0 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "Name",
                    Value = str0.ToString()
                };
                EmbedFieldBuilder field1 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "ID",
                    Value = str1.ToString()
                };
                EmbedBuilder builder = new EmbedBuilder();
                builder.AddField(field0);
                builder.AddField(field1);
                builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                builder.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("perms", RunMode = RunMode.Async), Alias("permissions"), Summary("Gets perms [API]")]
        public async Task Perms()
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Permissions))
            {
                await Context.Message.DeleteAsync();
                var sw = Stopwatch.StartNew();
                StringBuilder str0 = new StringBuilder();
                GuildPermissions perms = (Context.User as SocketGuildUser).GuildPermissions;
                perms.ToList().ForEach(x => str0.Append($"{x}\n"));
                EmbedFieldBuilder field0 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = $"Permissions - {Context.User.Username}",
                    Value = str0.ToString()
                };
                EmbedBuilder builder = new EmbedBuilder();
                builder.AddField(field0);
                builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                builder.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("random", RunMode = RunMode.Async), Summary("Random user")]
        public async Task randomUser()
        {
            await Context.Message.DeleteAsync();
            if (isVip(Context.User as SocketGuildUser) && jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Random))
            {
                IGuildUser user = (await Context.Guild.GetUsersAsync()).Where(x => x is SocketGuildUser).ToList()[_ran.Next(((SocketGuild)Context.Guild).MemberCount)];
                await Context.Channel.SendMessageAsync($"{user.Mention}");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("disc", RunMode = RunMode.Async), Summary("Discriminator search")]
        public async Task discriminatorSearch()
        {
            await Context.Message.DeleteAsync();
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Discriminator))
            {
                bool y = true;
                SocketGuildUser me = (Context.User as SocketGuildUser);
                (await Context.Guild.GetUsersAsync()).Where(x => x is SocketGuildUser).ToList().ForEach(async z => 
                {
                    if (z.DiscriminatorValue.Equals(me.DiscriminatorValue) && !z.Username.Equals(me.Username))
                    {
                        y = false;
                        await Context.Channel.SendMessageAsync($"{z.Username} - {z.DiscriminatorValue}");
                    }
                });
                if (y)
                {
                    await Context.Channel.SendMessageAsync($"`No users found!`");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("top", RunMode = RunMode.Async), Alias("users"), Summary("Gets first users")]
        public async Task topUsers([Summary("Number")]int users2find = 10)
        {
            if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Top))
            {
                await Context.Message.DeleteAsync();
                if (users2find > 25 || users2find > (Context.Guild as SocketGuild).MemberCount) {
                    await Context.Channel.SendMessageAsync("Whoa, slow down there buddy!");
                    return;
                }
                var sw = Stopwatch.StartNew();
                StringBuilder str0 = new StringBuilder();
                StringBuilder str1 = new StringBuilder();
                IEnumerable<IGuildUser> users = (await Context.Guild.GetUsersAsync()).Where(x => x is SocketGuildUser).ToList().OrderBy(x => x.JoinedAt).Take(users2find);
                users.ToList().ForEach(x => str0.Append($"{x.Username}\n"));
                users.ToList().ForEach(x => str1.Append($"{x.JoinedAt}\n"));
                EmbedFieldBuilder field0 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "Name",
                    Value = str0.ToString()
                };
                EmbedFieldBuilder field1 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "Joined at",
                    Value = str1.ToString()
                };
                EmbedBuilder builder = new EmbedBuilder();
                builder.AddField(field0);
                builder.AddField(field1);
                builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                builder.WithCurrentTimestamp();
                sw.Stop();
                await Context.Channel.SendMessageAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("mute", RunMode = RunMode.Async), Summary("Mutes a person.")]
        public async Task Mute([Summary("User")]SocketGuildUser arg, [Summary("Reason"), Remainder()]string reason = null)
        {
            if (isVip(Context.User as SocketUser) && (Context.User as SocketGuildUser).GuildPermissions.KickMembers)
            {
                if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Mute))
                {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    await arg.AddRolesAsync(arg.Guild.Roles.Where(y => y.Name.ToLower().Equals("muted")));
                    EmbedBuilder builder = new EmbedBuilder();
                    EmbedAuthorBuilder author = new EmbedAuthorBuilder();
                    builder.Title = $"**{arg.Username} has been muted!**{(string.IsNullOrEmpty(reason) ? "" : $"\n**Reason: {reason}**")}";
                    author.IconUrl = arg.GetAvatarUrl();
                    builder.WithAuthor(author);
                    builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                    builder.WithCurrentTimestamp();
                    sw.Stop();
                    await Context.Channel.SendMessageAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                    await sendReasonEmbedToUserDM(arg, reason, Commands.Mute);
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
        }

        [Command("fmute", RunMode = RunMode.Async), Summary("Mutes a person.")]
        public async Task fMute([Summary("User")]SocketGuildUser arg, [Summary("Reason"), Remainder()]string reason = null)
        {
            if (isVip(Context.User as SocketUser) && (Context.User as SocketGuildUser).GuildPermissions.KickMembers)
            {
                if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Mute))
                {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    await arg.RemoveRolesAsync(arg.Roles.Where(x => x.Id != Context.Guild.EveryoneRole.Id && x.Position < x.Guild.CurrentUser.Hierarchy));
                    await arg.AddRolesAsync(arg.Guild.Roles.Where(y => y.Name.ToLower().Equals("muted")));
                    EmbedBuilder builder = new EmbedBuilder();
                    EmbedAuthorBuilder author = new EmbedAuthorBuilder();
                    builder.Title = $"**{arg.Username} has been muted!**{(string.IsNullOrEmpty(reason) ? "" : $"\n**Reason: {reason}**")}";
                    author.IconUrl = arg.GetAvatarUrl();
                    builder.WithAuthor(author);
                    builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                    builder.WithCurrentTimestamp();
                    sw.Stop();
                    await Context.Channel.SendMessageAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                    await sendReasonEmbedToUserDM(arg, reason, Commands.Mute);
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
        }

        [Command("normie", RunMode = RunMode.Async), Summary("Normies a person.")]
        public async Task Normie([Summary("User")]SocketGuildUser arg, [Summary("Reason"), Remainder()]string reason = null)
        {
            if (isVip(Context.User as SocketUser) && (Context.User as SocketGuildUser).GuildPermissions.KickMembers)
            {
                if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Normie))
                {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    await arg.RemoveRolesAsync(arg.Roles.Where(x => x.Id != Context.Guild.EveryoneRole.Id && x.Position < x.Guild.CurrentUser.Hierarchy));
                    await arg.AddRolesAsync(arg.Guild.Roles.Where(y => y.Name.ToLower().Equals("normie")));
                    EmbedBuilder builder = new EmbedBuilder();
                    EmbedAuthorBuilder author = new EmbedAuthorBuilder();
                    builder.Title = $"**{arg.Username} has been Normied!**{(string.IsNullOrEmpty(reason) ? "" : $"\n**Reason: {reason}**")}";
                    author.IconUrl = arg.GetAvatarUrl();
                    builder.WithAuthor(author);
                    builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                    builder.WithCurrentTimestamp();
                    sw.Stop();
                    await Context.Channel.SendMessageAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                    await sendReasonEmbedToUserDM(arg, reason, Commands.Normie);
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
        }

        [Command("kick", RunMode = RunMode.Async), Summary("Kicks a person.")]
        public async Task Kick([Summary("User")]SocketGuildUser arg, [Summary("Reason"), Remainder()]string reason = null)
        {
            if (isVip(Context.User as SocketUser) && (Context.User as SocketGuildUser).GuildPermissions.KickMembers)
            {
                if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Kick))
                {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    noSend.Add(arg.Id);
                    await sendReasonEmbedToUserDM(arg, reason, Commands.Kick);
                    await arg.KickAsync(reason: reason);
                    EmbedBuilder builder = new EmbedBuilder();
                    EmbedAuthorBuilder author = new EmbedAuthorBuilder();
                    builder.Description = $"**{arg.Username} has been kicked!**{(string.IsNullOrEmpty(reason) ? "" : $"\n**Reason: {reason}**")}";
                    author.IconUrl = arg.GetAvatarUrl();
                    builder.WithAuthor(author);
                    builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                    builder.WithCurrentTimestamp();
                    sw.Stop();
                    await Context.Channel.SendMessageAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
        }

        [Command("ban", RunMode = RunMode.Async), Summary("Bans a person.")]
        public async Task Ban([Summary("User")]SocketGuildUser arg, [Summary("Reason"), Remainder()]string reason = null)
        {
            if (isVip(Context.User as SocketUser) && (Context.User as SocketGuildUser).GuildPermissions.BanMembers)
            {
                if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Ban))
                {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    noSend.Add(arg.Id);
                    await sendReasonEmbedToUserDM(user: arg, reason: reason, r: Commands.Ban);
                    await Context.Guild.AddBanAsync(user: arg, pruneDays: 1, reason: reason);
                    EmbedBuilder builder = new EmbedBuilder();
                    EmbedAuthorBuilder author = new EmbedAuthorBuilder();
                    builder.Description = $"**{arg.Username} has been banned!**{(string.IsNullOrEmpty(reason) ? "" : $"\n**Reason: {reason}**")}";
                    author.IconUrl = arg.GetAvatarUrl();
                    builder.WithAuthor(author);
                    builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                    builder.WithCurrentTimestamp();
                    sw.Stop();
                    await Context.Channel.SendMessageAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
        }

        [Command("softban", RunMode = RunMode.Async), Summary("softBans a person.")]
        public async Task Softban([Summary("User")]SocketGuildUser arg, [Summary("Reason"), Remainder()]string reason = null)
        {
            if (isVip(Context.User as SocketUser) && (Context.User as SocketGuildUser).GuildPermissions.BanMembers)
            {
                if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Softban))
                {
                    var sw = Stopwatch.StartNew();
                    noSend.Add(arg.Id);
                    await sendReasonEmbedToUserDM(arg, reason, Commands.Softban);
                    await Context.Guild.AddBanAsync(arg, 5);
                    EmbedBuilder builder = new EmbedBuilder();
                    EmbedAuthorBuilder author = new EmbedAuthorBuilder();
                    builder.Description = $"**{arg.Username} has been softbanned!**{(string.IsNullOrEmpty(reason) ? "" : $"\n**Reason: {reason}**")}";
                    author.IconUrl = arg.GetAvatarUrl();
                    builder.WithAuthor(author);
                    builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                    builder.WithCurrentTimestamp();
                    sw.Stop();
                    await Context.Channel.SendMessageAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                    await Context.Guild.RemoveBanAsync(arg);
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
        }

        [Command("prune", RunMode = RunMode.Async), Summary("Prune messages")]
        public async Task Prune([Summary("Number of msgs")]int arg1 = 100, [Summary("User")]SocketGuildUser arg2 = null)
        {
            if (isVip(Context.User as SocketUser) && (Context.User as SocketGuildUser).GuildPermissions.ManageMessages)
            {
                if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Prune))
                {
                    var sw = Stopwatch.StartNew();
                    IEnumerable<IMessage> msgs = await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, arg1).Flatten();
                    if (arg2 != null)
                    {
                        msgs = (await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, 350).Flatten()).Where(x => x.Author.Id == arg2.Id).Take(arg1);
                    }
                    await Context.Message.DeleteAsync();
                    await Context.Channel.DeleteMessagesAsync(msgs);
                    EmbedBuilder builder = new EmbedBuilder();
                    builder.Description = $"**{msgs.Count()} messages deleted!**";
                    builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                    builder.WithCurrentTimestamp();
                    sw.Stop();
                    await Context.Channel.SendMessageAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
        }

        [Command("iprune", RunMode = RunMode.Async), Summary("Prune messages")]
        public async Task iPrune([Summary("Number of msgs")]int arg1 = 100, [Summary("User")]SocketGuildUser arg2 = null)
        {
            if (isVip(Context.User as SocketUser) && (Context.User as SocketGuildUser).GuildPermissions.ManageMessages)
            {
                if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, Commands.Iprune))
                {
                    var sw = Stopwatch.StartNew();
                    IEnumerable<IMessage> msgs = (await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, 350).Flatten()).Where(x => x.Attachments.Count != 0).Take(arg1);
                    if (arg2 != null)
                    {
                        msgs = (await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, 550).Flatten()).Where(x => x.Author.Id == arg2.Id && x.Attachments.Count != 0).Take(arg1);
                    }
                    await Context.Message.DeleteAsync();
                    await Context.Channel.DeleteMessagesAsync(msgs);
                    EmbedBuilder builder = new EmbedBuilder();
                    builder.Description = $"**{msgs.Count()} messages deleted!**";
                    builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                    builder.WithCurrentTimestamp();
                    sw.Stop();
                    await Context.Channel.SendMessageAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
        }

        public void fuckoff()
        {
            Console.WriteLine("fuck off");
        }

        private async Task _Eval(string code, bool withType = false, bool methods = false, bool statics = false, string methodName = null)
        {
            await Context.Message.DeleteAsync();
            /*
            if (methods) withType = true;
            var sw = Stopwatch.StartNew();
            //var prog = _map.Get<Main.Daddy>();
            var prog2 = new Main.Daddy();
            Color color;
            var cts = new CancellationTokenSource();
            cts.CancelAfter(10_000);
            var source = new Subject<Ext.FieldValue>();
            var embedbuilder = Ext.Extensions.BuildBotAnswer(source);
            source.OnNext(new Ext.FieldValue { Code = "cs", Content = code, Title = "Input" });
            var scriptOptions = _types.Aggregate(ScriptOptions.Default.AddReferences(Assembly.GetEntryAssembly()), (acc, x) => acc.AddReferences(AssemblyMetadata.CreateFromFile(x.GetTypeInfo().Assembly.Location).GetReference()))
                .AddImports(
                    "Daddy", "System.Linq", "System", "Daddy.Main", "Daddy.Database",
                    "System.IO", "System.Threading", "System.Threading.Tasks",
                    "System.Diagnostics", "System.Collections.Generic",
                    "Discord", "Discord.Commands", "Discord.WebSocket", "Discord.Audio",
                    "Discord.Rest", "Discord.Rpc"
                );
            var globals = new Globals { client = Context.Client, context = Context as SocketCommandContext, bot = prog2, _types = _types };
            object result = null;
            var vizual = new StringBuilder();
            string type = "";
            bool success = false;
            try
            {
                var ctask = CSharpScript.EvaluateAsync(code, globals: globals, cancellationToken: cts.Token, options: scriptOptions);
                ctask.Wait(cts.Token);
                result = await ctask;
                if (result != null)
                {
                    //prog.ObjectMemory.Push(result);
                }
                type = Format.Sanitize(Ext.TypeInspector.VisualizeType(result));
                if (methods)
                {
                    vizual.Append(Ext.TypeInspector.VisualizeMethods(result, !statics, methodName));
                }
                else
                {
                    vizual.Append(Ext.TypeInspector.VisualizeValue(result, withType, findMember: methodName));
                }
                await Main.Daddy.log(vizual.ToString());
                color = Success;
                success = true;
                //await Context.Message.DeleteAsync();
            }
            catch (CompilationErrorException e)
            {
                await Main.Daddy.log("Error", exception: e);

                foreach (var item in e.Diagnostics)
                {
                    if (withType)
                    {
                        vizual.AppendLine(Format.Bold("Error"));
                        vizual.AppendLine(Format.Code(Ext.Extensions.StripBlock(item.ToString())));
                    }
                    source.OnNext(new Ext.FieldValue { Content = item.ToString(), Code = "", Title = "Error" });
                }
                if (withType)
                {
                    await ReplyAsync(vizual.ToString());
                    return;
                }
                color = Error;
            }
            catch (OperationCanceledException e)
            {
                await Main.Daddy.log("Error", exception: e);
                source.OnNext(new Ext.FieldValue { Content = e.Message, Code = "", Title = "Timeout" });
                color = Warning;
            }
            catch (JsonSerializationException e)
            {
                await Main.Daddy.log("Error", exception: e);
                source.OnNext(new Ext.FieldValue { Content = e.Message, Code = "", Title = "Error" });
                color = Error;
            }
            catch (TargetInvocationException e)
            {
                await Main.Daddy.log("Error", exception: e);
                source.OnNext(new Ext.FieldValue { Content = e.InnerException.Message, Code = "", Title = "Error" });
                color = Error;
            }
            catch (Exception e)
            {
                await Main.Daddy.log("Error", exception: e);
                source.OnNext(new Ext.FieldValue { Content = e.Message, Code = "", Title = "Error" });
                color = Error;
            }

            try
            {
                var lastpart = "";
                if (withType)
                {
                    var s = new StringBuilder();
                    s.AppendLine(Format.Bold("Input"));
                    s.AppendLine(Format.Code(Ext.Extensions.StripBlock(code), "cs"));
                    s.AppendLine(Format.Bold($"Result<{type}>"));
                    await ReplyAsync(s.ToString());
                }

                // Build message
                if (success)
                {
                    if (vizual.Length > (withType ? 1800 : 1000))
                    {
                        var vizString = vizual.ToString();
                        var header = vizString.TrimStart('\r', '\n', ' ', '\t').Split(new char[] { '\n' }, 2)[0];
                        vizString = vizString.Substring(header.Length + 1);
                        var continuation = false;
                        while (true)
                        {
                            var end = vizString.IndexOf('\n', Math.Min(withType ? 1800 : 800, vizString.Length));
                            if (end < 0)
                            {
                                end = vizString.Length;
                            }
                            var part = vizString.Substring(0, end);
                            vizString = vizString.Substring(Math.Min(end + 1, vizString.Length));
                            if (continuation)
                            {
                                source.OnNext(new Ext.FieldValue { Content = $"{header}\n{part}", Code = "", Title = $"Continuation<{type}>" });
                            }
                            else
                            {
                                source.OnNext(new Ext.FieldValue { Content = $"{header}\n{part}", Code = "", Title = $"Result<{type}>" });
                            }
                            if (vizString.Length <= 0)
                            {
                                lastpart = $"{header}\n{part}";
                                break;
                            }
                            // Send

                            source.OnCompleted();
                            var res = (withType) ? await ReplyAsync(Format.Code(Ext.Extensions.StripBlock($"{header}\n{part}"))) : await ReplyAsync("", false, (await embedbuilder).WithColor(color));
                            await Main.Daddy.log(res.Content);
                            source = new Subject<Ext.FieldValue>();
                            embedbuilder = Ext.Extensions.BuildBotAnswer(source);
                            continuation = true;
                        }
                    }
                    else
                    {
                        lastpart = vizual.ToString();
                        source.OnNext(new Ext.FieldValue { Content = lastpart, Code = "", Title = $"Result<{type}>" });
                    }
                }
                else
                {
                    if (withType)
                    {
                        source.OnCompleted();
                        await ReplyAsync("", embed: (await embedbuilder).WithColor(color));
                        return;
                    }
                }
                if (type.Equals("String"))
                {
                    string str = result.ToString();
                    source.OnNext(new Ext.FieldValue { Content = str.Substring(0, Math.Min(str.Length, 500)), Code = "", Title = "Value" });
                }

                source.OnCompleted();
                sw.Stop();
                var reply = (withType) ? await ReplyAsync(Format.Code(lastpart)) : await ReplyAsync("", false, (await embedbuilder).WithColor(color).WithFooter(x => x.WithText($"In {sw.ElapsedMilliseconds}ms")));
                await Main.Daddy.log(reply.Content);
            }
            catch (Exception e)
            {
                await Main.Daddy.log("Error", exception: e);
                source = new Subject<Ext.FieldValue>();
                embedbuilder = Ext.Extensions.BuildBotAnswer(source);
                source.OnNext(new Ext.FieldValue { Content = e.Message, Code = "", Title = "Error" });
                source.OnCompleted();
                color = Error;
                await ReplyAsync("", false, (await embedbuilder).WithColor(color).WithFooter(x => x.WithText($"In {sw.ElapsedMilliseconds}ms")));
            }*/
        }

        public async Task sendReasonEmbedToUserDM(SocketUser user, string reason, Commands r)
        {
            EmbedBuilder builder = new EmbedBuilder();
            EmbedAuthorBuilder author = new EmbedAuthorBuilder();
            var sw = Stopwatch.StartNew();
            switch (r)
            {
                case Commands.Mute:
                    builder.Description = $"**You are Muted!**{(string.IsNullOrEmpty(reason) ? "" : $"\n**Reason: {reason}**")}";
                    break;
                case Commands.Kick:
                    builder.Description = $"**You are Kicked!**{(string.IsNullOrEmpty(reason) ? "" : $"\n**Reason: {reason}**")}";
                    break;
                case Commands.Softban:
                    builder.Description = $"**You are Softbanned!**{(string.IsNullOrEmpty(reason) ? "" : $"\n**Reason: {reason}**")}";
                    break;
                case Commands.Ban:
                    builder.Description = $"**You are Banned!**{(string.IsNullOrEmpty(reason) ? "" : $"\n**Reason: {reason}**")}";
                    break;
                case Commands.Normie:
                    builder.Description = $"**You are Normied!**{(string.IsNullOrEmpty(reason) ? "" : $"\n**Reason: {reason}**")}";
                    break;
                default:
                    builder.Description = $"**null**";
                    break;
            }
            author.IconUrl = user.GetAvatarUrl();
            builder.WithAuthor(author);
            builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
            IDMChannel x = await user.GetOrCreateDMChannelAsync();
            builder.WithCurrentTimestamp();
            sw.Stop();
            await x.SendMessageAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
        }

        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                throw new ArgumentException("ARGH!");
            }
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        public static Stream GetStreamFromUrl(string url)
        {
            WebRequest req = WebRequest.Create(url);
            Task<WebResponse> res = Task.Factory.FromAsync(req.BeginGetResponse, x => req.EndGetResponse(x), (object)null);
            return res.ContinueWith(x => x.Result.GetResponseStream()).Result;
        }

        public string getHighestPerm(GuildPermissions perms)
        {
            if (perms.Administrator)
            {
                return "ADMIN";
            }
            else if (perms.ManageRoles)
            {
                return "MOD";
            }
            else
            {
                return "USER";
            }
        }

        public static bool isVip(SocketUser user)
        {
            return ((user as SocketGuildUser).Roles.Where(x => x.Position > x.Guild.CurrentUser.Hierarchy).Count() > 0);
        }

        public static bool doesExistJson(IGuild arg)
        {
            return File.Exists($@"json\jayson_{arg.Id}.json");
        }

        public static async void createJson(IGuild arg)
        {
            BaseCommands _bc = new BaseCommands();
            IEnumerable<IGuildChannel> channels = (await arg.GetChannelsAsync()).Where(y => y is SocketTextChannel).ToList().OrderBy(y => y.Position);
            _bc.perm.Clear();
            channels.ToList().ForEach(z => _bc.perm.Add(z.Id, _bc.inPerm));
            File.WriteAllText($@"json\jayson_{arg.Id}.json", JsonConvert.SerializeObject(_bc.perm, Formatting.Indented));
        }

        public static bool doesExistSettings(IGuild arg)
        {
            return File.Exists($@"Settings\settings_{arg.Id}.json");
        }

        public static void createSettings(IGuild arg)
        {
            BaseCommands _bc = new BaseCommands();
            _bc.welcome.Clear();
            _bc.welcome.Add("Settings", _bc.inWelcome);
            File.WriteAllText($@"Settings\settings_{arg.Id}.json", JsonConvert.SerializeObject(_bc.welcome, Formatting.Indented));
        }

        public readonly ulong[] IDs = new ulong[] { 170695871510347778 };
        public bool SpecialPeople(ulong ID)
        {
            if (IDs.ToList().Where(x => x.Equals(ID)).Count() != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static ulong mention2role(string arg)
        {
            return Convert.ToUInt64(Regex.Replace(arg, "[^0-9.]", string.Empty));
        }

        public static string removeMention(string str)
        {
            return Regex.Replace(str, @"\<.*\>+", string.Empty);
        }

        public static string removeAllMention(string str)
        {
            StringBuilder sb = new StringBuilder(Regex.Replace(str, @"\<.*?\>+", string.Empty));
            return sb   .Replace("@here", string.Empty)
                        .Replace("@everyone", string.Empty)
                        .ToString();
        }

        public enum Commands
        {
            Mute = 0,
            Kick = 1,
            Softban = 2,
            Ban = 3,
            Prune = 4,
            Iprune = 5,
            Avatar = 6,
            Channels = 7,
            Info = 8,
            Echo = 9,
            Hug = 10,
            Kiss = 11,
            Laugh = 12,
            Punch = 13,
            Slap = 14,
            Pat = 15,
            Pout = 16,
            Cry = 17,
            Rage = 18,
            Nosebleed = 19,
            Cat = 20,
            Dog = 21,
            Bird = 22,
            Help = 23,
            Eval = 24,
            Top = 25,
            Normie = 26,
            Boss = 27,
            Airplane = 28,
            Random = 29,
            Hue = 30,
            Write = 31, 
            Welcome = 32,
            Leave = 33,
            Permissions = 34,
            Discriminator = 35
        }

        public enum Settings
        {
            Welcome = 0,
            Leave = 1,
            JoinRole = 2,
            CharSpam = 3,
            WordSpam = 4,
            EmojiSpam = 5,
            Prefix = 6
        }
    }

    public static class ExtensionMethods
    {
        public static async Task<object> InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
        {
            dynamic awaitable = @this.Invoke(obj, parameters);
            await awaitable;
            return awaitable.GetAwaiter().GetResult();
        }
    }
}