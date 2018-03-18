using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Diagnostics;
using Imouto.BooruParser;
using Imouto.BooruParser.Loaders;
using Daddy.Ext;

namespace Daddy.Modules
{
    public class Emoticon : ModuleBase
    {
        private static readonly string[] hugGifs = new string[] { "https://myanimelist.cdn-dena.com/s/common/uploaded_files/1461073447-335af6bf0909c799149e1596b7170475.gif",
            "http://gifimage.net/wp-content/uploads/2017/01/Anime-hug-GIF-Image-Download-20.gif", "https://m.popkey.co/fca5d5/bXDgV.gif", "https://media.giphy.com/media/l4FGza2kjFT3BJ3K8/giphy.gif",
            "https://media.giphy.com/media/3oKIPmUzjvyx1xCmru/giphy.gif", "https://i.giphy.com/143v0Z4767T15e.gif", "https://d.wattpad.com/story_parts/314301302/images/1476c62b891dce82219301612602.gif",
            "https://data.whicdn.com/images/135392484/original.gif", "https://media.giphy.com/media/svXXBgduBsJ1u/giphy.gif" };
        private static readonly string[] patGifs = new string[] { "https://m.popkey.co/a5cfaf/1x6lW.gif", "https://media.tenor.co/images/bf646b7164b76efe82502993ee530c78/tenor.gif",
            "https://68.media.tumblr.com/cc0451847fa08b202f4bd7a1cb9bd327/tumblr_o2js2xhINq1tydz8to1_500.gif", "https://media.tenor.co/images/68d981347bf6ee8c7d6b78f8a7fe3ccb/tenor.gif",
            "https://i.giphy.com/iGZJRDVEM6iOc.gif", "https://68.media.tumblr.com/71d93048022df065a1d2af96ab71afa3/tumblr_olykrec0DB1qbvovho1_500.gif" };
        private static readonly string[] punchGifs = new string[] { "https://i.giphy.com/10Im1VWMHQYfQI.gif", "https://s-media-cache-ak0.pinimg.com/originals/e6/f9/7c/e6f97cb321e8b8a0fed85195d47d7832.gif",
            "http://i3.kym-cdn.com/photos/images/original/001/117/646/bf9.gif", "https://media.giphy.com/media/iWAqMe8hBWKVq/giphy-downsized-large.gif", "https://i.giphy.com/LdsJrFnANh6HS.gif" };
        private static readonly string[] kissGifs = new string[] { "https://i.giphy.com/12VXIxKaIEarL2.gif", "https://media.tenor.co/images/802f7fa791471c2e33dc06475d2b54c8/tenor.gif",
            "https://i.giphy.com/QGc8RgRvMonFm.gif", "http://24.media.tumblr.com/dc0496ce48c1c33182f24b1535521af2/tumblr_mo77fusajy1spwngeo1_500.gif",
            "https://68.media.tumblr.com/d07fcdd5deb9d2cf1c8c44ffad04e274/tumblr_ok1kd5VJju1vlvf9to1_500.gif", "https://68.media.tumblr.com/60c27235f6440d9d6ebd8168bb75c384/tumblr_nxd3nn8iJ81rcikyeo1_500.gif",
            "https://media3.giphy.com/media/VXsUx3zjzwMhi/giphy.gif" };
        private static readonly string[] laughGifs = new string[] { "https://i.giphy.com/mnBsYB19OQCdy.gif", "http://i.imgur.com/5EMqe6Y.gif", "https://media.tenor.co/images/766b72ce843075ebe5a3d10841a3651b/tenor.gif",
            "https://s-media-cache-ak0.pinimg.com/originals/c0/7c/28/c07c28b2f57bc12ec07d947c8877bfe7.gif", "https://i.giphy.com/QKO4Fvdlrop6o.gif" };
        private static readonly string[] slapGifs = new string[] { "http://i0.kym-cdn.com/photos/images/newsfeed/000/940/326/086.gif", "https://31.media.tumblr.com/a6db390ca2a6daaf930cac40cfe85743/tumblr_n8m1dib4eP1tet3lvo1_500.gif",
            "https://i.giphy.com/LeTDEozJwatvW.gif", "https://i.giphy.com/Zau0yrl17uzdK.gif", "https://i.giphy.com/nesNeNkOb9Tz2.gif", "https://i.giphy.com/jLeyZWgtwgr2U.gif" };
        private static readonly string[] poutGifs = new string[] { "http://pa1.narvii.com/5805/6f8b4788caf1fc9f343ced4a93d43787a4022477_hq.gif", "https://myanimelist.cdn-dena.com/s/common/uploaded_files/1453266384-80b5e1f6f3080634c4692c0ca5a584f0.gif",
            "https://media.giphy.com/media/ZJ5IjUYXbC13y/giphy.gif", "https://s-media-cache-ak0.pinimg.com/originals/a0/c2/64/a0c264ad6b12b28d7c58871d7f5a999c.gif", "https://68.media.tumblr.com/9b5fe189479356b47c6fede60fe12576/tumblr_nr7vttUbjG1s21xzoo3_500.gif"};
        private static readonly string[] cryGifs = new string[] { "http://vignette3.wikia.nocookie.net/shigatsu-wa-kimi-no-uso/images/9/92/Tumblr_ndxgg75AmD1sgtx3io2_500.gif", "http://i.giphy.com/yarJ7WfdKiAkE.gif",
            "https://68.media.tumblr.com/7a62299e92bfbe10194ecf7850e0769c/tumblr_okckzleiEg1vh9ej2o1_500.gif", "https://68.media.tumblr.com/56fea5a4d682cd26178c17d80f7ee82a/tumblr_ofedni0ELT1vztiw8o1_500.gif",
            "https://68.media.tumblr.com/6ecacbb69194f280a98ec5b52af54adc/tumblr_nk4kui4YGF1uo1owvo1_500.gif" };
        private static readonly string[] angryGifs = new string[] { "https://media.giphy.com/media/RMUKZW6Wmy2mk/giphy.gif", "https://media.giphy.com/media/yFLSs5jbhUgeI/giphy.gif",
            "http://66.media.tumblr.com/5f9d9c003a73a78d5f2195d0b0f36f44/tumblr_oeedu4hsVP1vgf8i8o1_500.gif", "http://img1.liveinternet.ru/images/attach/c/5/87/401/87401949_tumblr_lo5ntp6FIl1qcff4ao1_500.gif", "https://media.tenor.co/images/386fb4996e952415422e4de3f7ff9273/tenor.gif" };
        private static readonly string[] birdsGifs = new string[] { "https://media.giphy.com/media/OUuwn1HCfQjIY/giphy.gif", "https://media.giphy.com/media/4QaKX7FHE4Wqc/giphy.gif", "http://24.media.tumblr.com/cb38cff91b45f80dc493ae80ab8b181e/tumblr_n25qcoUHx71rvlx3so1_400.gif",
            "https://s-media-cache-ak0.pinimg.com/originals/1c/22/e0/1c22e0d4aac2b7ebd306e11519af58df.gif", "http://25.media.tumblr.com/tumblr_m7n0vkkChr1r2wrwho1_400.gif", "http://24.media.tumblr.com/b7ff23b167f2c9762e3a910d86f36cb9/tumblr_n0d0kfoy3y1rvlx3so2_r1_400.gif",
            "https://s-media-cache-ak0.pinimg.com/originals/51/c5/3e/51c53e5ed1ae0d880637cdb6ec70c5dd.gif", "https://media.giphy.com/media/JRQ0Y6hvMSCQw/giphy.gif" };
        private static readonly string[] nosebleedGifs = new string[] { "https://cdn.discordapp.com/attachments/306168780717817859/310790689896136705/giphy.gif", "https://cdn.discordapp.com/attachments/306168780717817859/310790853209751552/tumblr_nnlf8qSkMa1rdilz7o1_500.gif",
            "https://cdn.discordapp.com/attachments/306168780717817859/310790927025176576/tenor.gif", "https://cdn.discordapp.com/attachments/306168780717817859/310791083913379842/giphy.gif", "https://cdn.discordapp.com/attachments/306168780717817859/310791169372323840/giphy.gif",
            "https://cdn.discordapp.com/attachments/306168780717817859/310791367901052939/tumblr_opexdpY5O21vjzwpyo1_500.gif" };
        private static readonly string boss = "https://cdn.discordapp.com/attachments/290983102333976578/361283224128978974/634.png";
        private static readonly string[] airplane = new string[] { "https://lh3.googleusercontent.com/z31TVe9M2cR-uS9H4aPaZd7w8PnN_hxiq76nHbAhLmqZxfWsRHNBYpcQeEfd2Ot4Mdc=w300", "http://www.boeing.com/resources/boeingdotcom/commercial/assets/images/current-products/767.jpg",
            "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3e/FA-18_Hornet_VFA-41.jpg/300px-FA-18_Hornet_VFA-41.jpg", "https://sofrep.com/wp-content/uploads/2017/04/super-hornet-905x604.jpg",
            "https://i2.wp.com/fightersweep.com/wp-content/uploads/2015/12/AJ400_F18_NTU_SEPT06_P1FS.jpg?resize=630%2C418&ssl=1", "https://upload.wikimedia.org/wikipedia/commons/thumb/a/ab/AC-130H_Spectre_jettisons_flares.jpg/1200px-AC-130H_Spectre_jettisons_flares.jpg",
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRsR4dS3xvNcFjY4DpFwPeykfGQ5LxTM-t0rCNvRTaQE9vZ_J3Z", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTFaonnn869i8t8jehy6pcX1V-2Lj_ogzy-Le_b1_6uqewS5taM9w",
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRLbGIP7aOJRqJDsWVoGl-g7iiOjRBK3Coiqxrq2CVpWFKhCbBW_g", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ8UCkX6N2pxe7RKIEeq9gmxLmpECw7fOp2ckIRjPhZQzyHEvYKEQ",
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQxvWSbzvSd4LbooCRQNbAKCg6kBsy1PmCo1tnlAeWWX0puGfHY", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS95s6AqYA_QQgUXNBGNbCGHOzLN3se7SccvNyMw1elk8o7unfPSA",
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS_VAcgnFJgD1CqUnad_HQY5UdIkj2ycgf3G1PZ9VkBPKYeiXNZ", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRrULLtO2Hv3Ot8AzwUXr1hqF3ELcHPVhlT6717zjiD5NPRCNub",
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRRe3tUPsJNQr1dbd1Y5W-B_u1QFWIxBJWdRl4TjE9nzAsAec8kcw", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcShYKIFg1-zNMn5PTdDUCrrcfkCqBVQImdD_vslqoM2utGA5MfV",
            "http://s.newsweek.com/sites/www.newsweek.com/files/styles/lg/public/2014/09/20/f22.jpg" };

        public static Random _ran = new Random();
        Database.JSON jSon = new Database.JSON();
        public static Emoticon instance = new Emoticon();

        [Command("hug", RunMode = RunMode.Async), Summary("Hugs a person."), Ratelimit(3, 0.5, Measure.Minutes)]
        public async Task Hug([Summary("Hugged person."), Remainder()]IUser arg = null)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Hug))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder();
                if (arg != null)
                {
                    if (!arg.Equals(Context.User))
                    {
                        builder.Title = $"{Context.User.Username} hugs {arg.Username}!";
                        builder.WithImageUrl(hugGifs[_ran.Next(hugGifs.Length)]);
                    }
                    else
                    {
                        builder.Title = $"{Context.User.Username} is a lonely fuck.";
                        builder.WithImageUrl("https://media.giphy.com/media/sLA17korcDnz2/giphy.gif");
                    }
                }
                else
                {
                    builder.Title = $"{Context.Client.CurrentUser.Username} hugs {Context.User.Username}! Cute and cuddly!";
                    builder.WithImageUrl(hugGifs[_ran.Next(hugGifs.Length)]);
                }
                builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                builder.WithCurrentTimestamp();
                sw.Stop();
                await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("pat", RunMode = RunMode.Async), Summary("Pats a person."), Ratelimit(3, 0.5, Measure.Minutes)]
        public async Task Pat([Summary("Patted person."), Remainder()]IUser arg)
        {
            try
            {
                await Console.Out.WriteLineAsync("1");
                if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Pat))
                {
                    await Console.Out.WriteLineAsync("2");
                    var sw = Stopwatch.StartNew();
                    EmbedBuilder builder = new EmbedBuilder();
                    if (arg != null)
                    {
                        await Console.Out.WriteLineAsync("3");
                        if (!BaseCommands.SpecialPeople(arg.Id))
                        {
                            await Console.Out.WriteLineAsync("4");
                            if (!arg.Equals(Context.User))
                            {
                                await Console.Out.WriteLineAsync("5");
                                builder.Title = $"{Context.User.Username} pats {arg.Username}! Awww, cute!";
                                builder.WithImageUrl(patGifs[_ran.Next(patGifs.Length)]);
                            }
                            else
                            {
                                await Console.Out.WriteLineAsync("5");
                                builder.Title = $"{Context.User.Username} pats their lonely head!";
                                builder.WithImageUrl("https://i.imgur.com/aykz290.gif");
                            }
                        }
                        else
                        {
                            await Console.Out.WriteLineAsync("6");
                            builder.Title = "no.";
                            builder.WithImageUrl("http://i.imgur.com/F0nMzoJ.gif");
                        }
                    }
                    else
                    {
                        await Console.Out.WriteLineAsync("3");
                        builder.Title = $"{Context.Client.CurrentUser.Username} pats {Context.User.Username}! Awww, cute!";
                        builder.WithImageUrl(patGifs[_ran.Next(patGifs.Length)]);
                    }
                    await Console.Out.WriteLineAsync("7");
                    builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                    builder.WithCurrentTimestamp();
                    sw.Stop();
                    await Console.Out.WriteLineAsync("8");
                    await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                    await Console.Out.WriteLineAsync("9");
                }
                else
                {
                    await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.ToString());
            }
        }

        [Command("kiss", RunMode = RunMode.Async), Summary("Kisses a person."), Ratelimit(3, 0.5, Measure.Minutes)]
        public async Task Kiss([Summary("Kissed person."), Remainder()]IUser arg = null)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Kiss))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder();
                if (arg != null)
                {
                    if (!arg.Equals(Context.User))
                    {
                        builder.Title = $"{Context.User.Username} kissed {arg.Username}! >///< :hearts:";
                    }
                    else
                    {
                        builder.Title = $"{Context.User.Username} wanted a kiss, so {Context.Client.CurrentUser.Username} gave them one :hearts:";
                    }
                }
                else
                {
                    builder.Title = $"{Context.Client.CurrentUser.Username} kissed {Context.User.Username}! :hearts:";
                }
                builder.WithImageUrl(kissGifs[_ran.Next(kissGifs.Length)]);
                builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                builder.WithCurrentTimestamp();
                sw.Stop();
                await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("laugh", RunMode = RunMode.Async), Summary("Laugh at person."), Ratelimit(3, 0.5, Measure.Minutes)]
        public async Task Laugh([Summary("User")]IUser arg = null)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Laugh))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder();
                if (arg != null)
                {
                    if (!arg.Equals(Context.User))
                    {
                        builder.Title = $"{Context.User.Username} laughs at {arg.Username}! HA-HA!";
                    }
                    else
                    {
                        builder.Title = $"{Context.User.Username} bursts out laughing! HA-HA!";
                    }
                }
                else
                {
                    builder.Title = $"{Context.User.Username} laughs! HA-HA!";
                }
                builder.WithImageUrl(laughGifs[_ran.Next(laughGifs.Length)]);
                builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                builder.WithCurrentTimestamp();
                sw.Stop();
                await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("nosebleed", RunMode = RunMode.Async), Ratelimit(2, 0.5, Measure.Minutes)]
        public async Task Nosebleed()
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Nosebleed))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder()
                {
                    Title = "(꒪ཀ꒪)",
                    ImageUrl = nosebleedGifs[_ran.Next(nosebleedGifs.Length)],
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                };
                builder.WithCurrentTimestamp();
                sw.Stop();
                await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("airplane", RunMode = RunMode.Async), Alias("plane"), Summary("Ask fucking Justino"), Ratelimit(3, 1, Measure.Minutes)]
        public async Task Airplane()
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Airplane))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder()
                {
                    Title = "ask Justino.",
                    ImageUrl = airplane[_ran.Next(airplane.Length)],
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                };
                builder.WithCurrentTimestamp();
                sw.Stop();
                await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("boss", RunMode = RunMode.Async), Ext.Ratelimit(1, 5, Measure.Minutes)]
        public async Task Boss()
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Boss))
            {
                await Context.Message.DeleteAsync();
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder()
                {
                    Title = "88",
                    ImageUrl = boss,
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                };
                builder.WithCurrentTimestamp();
                sw.Stop();
                await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("punch", RunMode = RunMode.Async), Summary("Punches a person."), Ratelimit(3, 0.5, Measure.Minutes)]
        public async Task Punch([Summary("Punched person")]IUser arg)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Punch))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder();
                if (arg != null)
                {
                    if (!BaseCommands.SpecialPeople(arg.Id))
                    {
                        builder.Title = $"{Context.User.Username} punches {arg.Username}! Ouch, that hurts!";
                        builder.WithImageUrl(punchGifs[_ran.Next(punchGifs.Length)]);
                    }
                    else
                    {
                        builder.Title = "no.";
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
                await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("slap", RunMode = RunMode.Async), Summary("Punches a person."), Ext.Ratelimit(3, 1, Measure.Minutes)]
        public async Task Slap([Summary("Punched person")]IUser arg)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Slap))
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder();
                if (arg != null)
                {
                    if (!BaseCommands.SpecialPeople(arg.Id))
                    {
                        builder.Title = $"{Context.User.Username} slaps {arg.Username}! SLAP!";
                        builder.WithImageUrl(slapGifs[_ran.Next(slapGifs.Length)]);
                    }
                    else
                    {
                        builder.Title = "no.";
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
                await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("pout", RunMode = RunMode.Async), Summary("Pout"), Ext.Ratelimit(2, 1, Measure.Minutes)]
        public async Task Pout()
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Pout))
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
                await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("cry", RunMode = RunMode.Async), Summary(":'("), Ext.Ratelimit(2, 1, Measure.Minutes)]
        public async Task Cry([Summary("IUser")]IUser arg = null)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Cry))
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
                await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("rage", RunMode = RunMode.Async), Alias("angry"), Summary("Kisses a person."), Ratelimit(2, 1, Ext.Measure.Minutes)]
        public async Task Rage([Summary("Kissed person.")]IUser arg = null)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Rage))
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
                await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("cat", RunMode = RunMode.Async), Alias("pussy"), Ext.Ratelimit(3, 0.5, Measure.Minutes)]
        public async Task Cat()
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Cat))
            {
                var sw = Stopwatch.StartNew();
                using (Stream s = new WebClient().OpenRead(new Uri("http://random.cat/meow")))
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader jr = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = ":cat:",
                        ImageUrl = serializer.Deserialize<Dictionary<string, string>>(jr).First().Value,
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    };
                    builder.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                }
            }
            else
            {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("dog", RunMode = RunMode.Async), Alias("doggo"), Ext.Ratelimit(3, 0.5, Measure.Minutes)]
        public async Task Dog()
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Dog))
            {
                var sw = Stopwatch.StartNew();
                using (Stream s = new WebClient().OpenRead(new Uri("https://random.dog/woof.json")))
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader jr = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $":dog:",
                        ImageUrl = serializer.Deserialize<Dictionary<string, string>>(jr).First().Value,
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    };
                    builder.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                }
            }
            else
            {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("bird", RunMode = RunMode.Async), Alias("birb"), Ext.Ratelimit(3, 0.5, Measure.Minutes)]
        public async Task Bird()
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Bird))
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
                await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }
    }

    public class NSFW : ModuleBase
    {
        Random _ran = new Random();
        Database.JSON jSon = new Database.JSON();

        [Command("lewd", RunMode = RunMode.Async), Summary("Sends random neko NSFW picture trough API"), Ext.Ratelimit(10, 0.5, Ext.Measure.Minutes)]
        public async Task Lewd()
        {
            if (Context.Channel._isNSFW())
            {
                var sw = Stopwatch.StartNew();
                using (Stream s = new WebClient().OpenRead(new Uri("https://nekos.life/api/lewd/neko")))
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader jr = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Title = "Neko :cat:",
                        ImageUrl = serializer.Deserialize<Dictionary<string, string>>(jr).First().Value
                    };
                    sw.Stop();
                    await ReplyAsync(string.Empty, embed: embed.WithFooter(x => x.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                }
            }
            else
            {
                await ReplyAsync("`NSFW required`");
            }
        }

        [Command("neko", RunMode = RunMode.Async), Summary("Sends random neko picture trough API"), Ext.Ratelimit(10, 0.5, Ext.Measure.Minutes)]
        public async Task Neko()
        {
            if (Context.Channel._isNSFW())
            {
                var sw = Stopwatch.StartNew();
                using (Stream s = new WebClient().OpenRead(new Uri("https://nekos.life/api/neko")))
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader jr = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Title = "Neko :cat:",
                        ImageUrl = serializer.Deserialize<Dictionary<string, string>>(jr).First().Value
                    };
                    sw.Stop();
                    await ReplyAsync(string.Empty, embed: embed.WithFooter(x => x.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                }
            }
            else
            {
                await ReplyAsync("`NSFW required`");
            }
        }

        [Command("boobs", RunMode = RunMode.Async), Alias("tits"), Summary("Sends random boob NSFW picture trough API"), Ext.Ratelimit(10, 0.5, Ext.Measure.Minutes)]
        public async Task Boobs()
        {
            if (Context.Channel._isNSFW())
            {
                var sw = Stopwatch.StartNew();
                using (Stream s = new WebClient().OpenRead(new Uri($"http://api.oboobs.ru/boobs/{_ran.Next(12000)}")))
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader jr = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        ImageUrl = $"http://media.oboobs.ru/{serializer.Deserialize<List<Dictionary<string, string>>>(jr).First()["preview"]}"
                    };
                    sw.Stop();
                    await ReplyAsync(string.Empty, embed: embed.WithFooter(x => x.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                }
            }
            else
            {
                await ReplyAsync("`NSFW required`");
            }
        }

        [Command("gif", RunMode = RunMode.Async), Alias(".gif"), Summary("Sends random NSFW gif trough API"), Ext.Ratelimit(10, 0.5, Ext.Measure.Minutes)]
        public async Task Gif()
        {
            if (Context.Channel._isNSFW())
            {
                var sw = Stopwatch.StartNew();
                EmbedBuilder embed = new EmbedBuilder()
                {
                    ImageUrl = $"https://cdn.boobbot.us/Gifs/{_ran.Next(1600, 2000)}.gif"
                };
                sw.Stop();
                await ReplyAsync(string.Empty, embed: embed.WithFooter(x => x.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await ReplyAsync("`NSFW required`");
            }
        }

        [Command("ass", RunMode = RunMode.Async), Alias("butt"), Summary("Sends random ass NSFW picture trough API"), Ext.Ratelimit(10, 0.5, Ext.Measure.Minutes)]
        public async Task Ass()
        {
            if (Context.Channel._isNSFW())
            {
                var sw = Stopwatch.StartNew();
                using (Stream s = new WebClient().OpenRead(new Uri($"http://api.obutts.ru/butts/{_ran.Next(4500)}")))
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader jr = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        ImageUrl = $"http://media.obutts.ru/{serializer.Deserialize<List<Dictionary<string, string>>>(jr).First()["preview"]}"
                    };
                    sw.Stop();
                    await ReplyAsync(string.Empty, embed: embed.WithFooter(x => x.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                }
            }
            else
            {
                await ReplyAsync("`NSFW required`");
            }
        }

        [Command("yre", RunMode = RunMode.Async), Summary("Sends NSFW picture trough API"), Ext.Ratelimit(10, 0.5, Ext.Measure.Minutes)]
        public async Task Yandere([Summary("Search term"), Remainder()]string search = null)
        {
            if (Context.Channel._isNSFW())
            {
                var sw = Stopwatch.StartNew();
                if (string.IsNullOrEmpty(search))
                {
                    using (Stream s = new WebClient().OpenRead(new Uri($"https://yande.re/post.json?limit=50")))
                    using (StreamReader sr = new StreamReader(s))
                    using (JsonReader jr = new JsonTextReader(sr))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        List<Dictionary<object, object>> myList = serializer.Deserialize<List<Dictionary<object, object>>>(jr);
                        EmbedBuilder embed = new EmbedBuilder()
                        {
                            ImageUrl = $"{myList[_ran.Next(myList.Count)]["jpeg_url"]}"
                        };
                        sw.Stop();
                        await ReplyAsync(string.Empty, embed: embed.WithFooter(x => x.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                    }
                }
                else
                {
                    using (Stream s = new WebClient().OpenRead(new Uri($"https://yande.re/post.json?tags={search}")))
                    using (StreamReader sr = new StreamReader(s))
                    using (JsonReader jr = new JsonTextReader(sr))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        List<Dictionary<object, object>> myList = serializer.Deserialize<List<Dictionary<object, object>>>(jr);
                        EmbedBuilder embed = new EmbedBuilder()
                        {
                            ImageUrl = $"{myList[_ran.Next(myList.Count)]["jpeg_url"]}"
                        };
                        sw.Stop();
                        await ReplyAsync(string.Empty, embed: embed.WithFooter(x => x.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                    }
                }
            }
            else
            {
                await ReplyAsync("`NSFW required`");
            }
        }

        [Command("e621", RunMode = RunMode.Async), Summary("Sends NSFW picture trough API"), Ext.Ratelimit(10, 0.5, Ext.Measure.Minutes)]
        public async Task E621([Summary("Search term"), Remainder()]string search = null)
        {
            if (Context.Channel._isNSFW())
            {
                var sw = Stopwatch.StartNew();
                if (string.IsNullOrEmpty(search))
                {
                    using (Stream s = new WebClient().OpenRead(new Uri($"https://e621.net/post/index.json?limit=50")))
                    using (StreamReader sr = new StreamReader(s))
                    using (JsonReader jr = new JsonTextReader(sr))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        List<Dictionary<object, object>> myList = serializer.Deserialize<List<Dictionary<object, object>>>(jr);
                        EmbedBuilder embed = new EmbedBuilder()
                        {
                            ImageUrl = $"{myList[_ran.Next(myList.Count)]["file_url"]}"
                        };
                        sw.Stop();
                        await ReplyAsync(string.Empty, embed: embed.WithFooter(x => x.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                    }
                }
                else
                {
                    using (Stream s = new WebClient().OpenRead(new Uri($"https://e621.net/post/index.json?tags={search}")))
                    using (StreamReader sr = new StreamReader(s))
                    using (JsonReader jr = new JsonTextReader(sr))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        List<Dictionary<object, object>> myList = serializer.Deserialize<List<Dictionary<object, object>>>(jr);
                        EmbedBuilder embed = new EmbedBuilder()
                        {
                            ImageUrl = $"{myList[_ran.Next(myList.Count)]["file_url"]}"
                        };
                        sw.Stop();
                        await ReplyAsync(string.Empty, embed: embed.WithFooter(x => x.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                    }
                }
            }
            else
            {
                await ReplyAsync("`NSFW required`");
            }
        }

        [Command("dan", RunMode = RunMode.Async), Summary("Sends NSFW picture trough API"), Ext.Ratelimit(10, 0.5, Ext.Measure.Minutes)]
        public async Task Danbooru()
        {
            if (Context.Channel._isNSFW())
            {
                var sw = Stopwatch.StartNew();
                var request = (HttpWebRequest)WebRequest.Create("https://danbooru.donmai.us/posts/1.json");
                request.Method = "PUT";
                /*
                var danbooruLoader = new DanbooruLoader(Database.JSON.getApiNamedb(), Database.JSON.getApiKeydb(), 1240);
                var post = await danbooruLoader.LoadPostAsync(_ran.Next(3000, 4000));
                EmbedBuilder embed = new EmbedBuilder()
                {
                    ImageUrl = post.Source
                };
                sw.Stop();
                await ReplyAsync(string.Empty, embed: embed.WithFooter(x => x.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                */
            }
            else
            {
                await ReplyAsync("`NSFW required`");
            }
        }

        [Command("hq", RunMode = RunMode.Async), Summary("Sends random hq NSFW picture trough API"), Ext.Ratelimit(10, 0.5, Ext.Measure.Minutes)]
        public async Task Hq()
        {
            if (Context.Channel._isNSFW())
            {
                var sw = Stopwatch.StartNew();
                int y = _ran.Next(1, 1460);
                EmbedBuilder embed = new EmbedBuilder()
                {
                    ImageUrl = $"https://cdn.boobbot.us/4k/4k{((y > 190 && y < 460) ? y += 1000 : ((y > 460 && y < 1000) ? _ran.Next(190) : y))}.jpg"
                };
                sw.Stop();
                await ReplyAsync(string.Empty, embed: embed.WithFooter(x => x.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else
            {
                await ReplyAsync("`NSFW required`");
            }
        }
    }
}
