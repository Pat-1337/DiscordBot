using Daddy.Database;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;//Install-Package Microsoft.CodeAnalysis.CSharp.Scripting //Microsoft.CodeAnalysis.Scripting
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

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
            { Commands.Write, true }, { Commands.Welcome, true }, { Commands.Leave, true }, { Commands.Permissions, false }, { Commands.Discriminator, true }, { Commands.Role, true }, { Commands.Gamble, false } };

        public Dictionary<Settings, object> inWelcome = new Dictionary<Settings, object>() { { Settings.Welcome, "{user} joined {server}!\n[Please change welcome message with .welcome <message> or disable it with .disable welcome]" },
            { Settings.Leave, "{user} left {server}\n[Please change leave message with .leave <message> or disable it with .disable leave]!" }, { Settings.JoinRole, 0 },
            { Settings.CharSpam, 0 }, { Settings.WordSpam, 0 }, { Settings.EmojiSpam, 0 }, { Settings.Prefix, '.' }, { Settings.DMmsg, 102 }, { Settings.LogCHN, 0 }, { Settings.Premium, 0 }, { Settings.WelcomeCHN, 0 } };
        private IEnumerable<Type> _types = new Type[] { typeof(CommandService), typeof(Main.Daddy), typeof(BaseCommands), typeof(JSON), typeof(Emoji),
            typeof(File), typeof(Discord.Rpc.DiscordRpcClient), typeof(DiscordSocketClient), typeof(IQueryable), typeof(Task), typeof(Process), typeof(Globals) };
        private static Color Success = new Color(52, 249, 59);
        private static Color Error = new Color(249, 52, 52);
        private static Color Warning = new Color(255, 230, 40);
        private static ulong token = 0;
        private static List<ulong> PremiumTokens = new List<ulong>();


        JSON jSon = new JSON();
        Ext._vMem _vMem = new Ext._vMem();

        [Command("shutdown", RunMode = RunMode.Async), Summary("Stops the bot")]
        public async Task Shutdown()
        {
            if (SpecialPeople(Context.User.Id)) {
                await Context.Message.DeleteAsync();
                await ReplyAsync("Eval shutdown! Bye bye butterfly");
                Main.Daddy._instance.Stop();
                //Main.Daddy._map.Get<Main.Daddy>().Stop();
                await Context.Client.StopAsync();
                await Task.CompletedTask;
            }
            else {
                await ReplyAsync($"This command requires `BOT_OWNER` - you have `{GetHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
            }
        }

        [Command("token", RunMode = RunMode.Async), Summary("Sets a token")]
        public async Task _token(ulong id)
        {
            if (SpecialPeople(Context.User.Id)) {
                await Context.Message.DeleteAsync();
                token = id;
                await Task.CompletedTask;
            }
            else {
                await ReplyAsync($"This command requires `BOT_OWNER` - you have `{GetHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
            }
        }

        [Command("gen", RunMode = RunMode.Async), Summary("Generates a token for premium")]
        public async Task _gen(string time)
        {
            if (SpecialPeople(Context.User.Id))
            {
                try
                {
                    await Context.Message.DeleteAsync();
                    ulong code;
                    switch (time.ToLower())
                    {
                        case "week":
                            code = _genNum(0);
                            break;
                        case "month":
                            code = _genNum(1);
                            break;
                        case "lifetime":
                            code = _genNum(2);
                            break;
                        default:
                            return;
                    }
                    PremiumTokens.Add(code);
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**Premium code created!**",
                        Description = $"code: {code.ToString()}",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255))),
                    }.WithCurrentTimestamp();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{Context.User}")).Build());
                }
                catch (Exception e)
                {
                    await Console.Out.WriteLineAsync(e.ToString());
                }
            }
            else
            {
                await ReplyAsync($"This command requires `BOT_OWNER` - you have `{GetHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
            }
        }

        [Command("activate", RunMode = RunMode.Async), Summary("Activates Premium")]
        public async Task _act(ulong code)
        {
            if (IsVip(Context.User as SocketUser)) {
                if (PremiumTokens.Contains(code) && Convert.ToByte(code.ToString().Substring(7, 1)).Equals(Convert.ToByte(Math.Abs(DateTime.Today.Month - 10))))
                {
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**Premium activated!**",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255))),
                    }.WithCurrentTimestamp();
                    switch (Convert.ToByte(code.ToString().Substring(3, 1)))
                    {
                        case 0:
                            builder.Description = "Duration: `week`";
                            break;
                        case 1:
                            builder.Description = "Duration: `month`";
                            break;
                        case 2:
                            builder.Description = "Duration: `lifetime`";
                            break;
                    }
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{Context.User}")).Build());
                    if (!DoesExistSettings(Context.Guild)) { CreateSettings(Context.Guild); }
                    JObject rss = JObject.Parse(File.ReadAllText($@"Settings/settings_{Context.Guild.Id}.json"));
                    JObject welcome = (JObject)rss["Settings"];
                    try
                    {
                        welcome["Premium"] = code;
                    }
                    catch
                    {
                        welcome.TryAdd("Premium", code);
                    }
                    File.WriteAllText($@"Settings/settings_{Context.Guild.Id}.json", rss.ToString());
                    _vMem._run();
                    PremiumTokens.Remove(code);
                }
                else
                {
                    await ReplyAsync($"`Error!`");
                }
            }
            else
            {
                await ReplyAsync($"`Permission denied!`");
            }
        }

        private ulong _genNum(byte time)
        {
            switch (time)
            {
                case 0:
                    return Convert.ToUInt64(_ran.Next(10000000, 99999999).ToString().Remove(2, 2).Insert(2, "00").Remove(7, 1).Insert(7, $"{Math.Abs(DateTime.Today.Month - 10)}"));
                case 1:
                    return Convert.ToUInt64(_ran.Next(10000000, 99999999).ToString().Remove(2, 2).Insert(2, "01").Remove(7, 1).Insert(7, $"{Math.Abs(DateTime.Today.Month - 10)}"));
                case 2:
                    return Convert.ToUInt64(_ran.Next(10000000, 99999999).ToString().Remove(2, 2).Insert(2, "02").Remove(7, 1).Insert(7, $"{Math.Abs(DateTime.Today.Month - 10)}"));
                default:
                    return 0;
            }   
        }

        [Command("profile", RunMode = RunMode.Async), Summary("Changes bot picture")]
        public async Task Pfp(string URL)
        {
            if (SpecialPeople(Context.User.Id)) {
                await Context.Message.DeleteAsync();
                await Main.Daddy._client.CurrentUser.ModifyAsync(new Action<SelfUserProperties>((SelfUserProperties x) => x.Avatar = new Image(GetStreamFromUrl(URL))));
            }
            else {
                await ReplyAsync($"This command requires `BOT_OWNER` - you have `{GetHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
            }
        }

        [Command("hue", RunMode = RunMode.Async), Summary("Hueansohn"), Ext.Ratelimit(3, 1, Ext.Measure.Minutes)]
        public async Task ColoredMessage(float hue, float sat, float lit, string title, [Remainder]string text)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Hue)) {
                await Context.Message.DeleteAsync();
                var sw = Stopwatch.StartNew();
                var color = CustomColor.FromHSL(hue, sat, lit);
                var (r, g, b) = (color.R, color.G, color.B);
                var embedbuilder = new EmbedBuilder().WithColor(color).WithDescription(text).WithTitle(title);
                sw.Stop();
                await ReplyAsync("", embed: embedbuilder.WithFooter(x => x.WithText($"In {sw.ElapsedMilliseconds}ms")).Build());
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("echo", RunMode = RunMode.Async), Ext.Ratelimit(2, 1, Ext.Measure.Minutes)]
        public async Task Echo([Summary("Message you want to repeat back."), Remainder()]string arg)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Echo)) {
                var sw = Stopwatch.StartNew();
                await Context.Message.DeleteAsync();
                if (string.IsNullOrEmpty(arg)) {
                    return;
                }
                else {
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = "Echo command",
                        Description = arg,
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                }
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("type", RunMode = RunMode.Async), Alias("write"), Summary("typewritermode"), Ext.Ratelimit(1, 1, Ext.Measure.Minutes)]
        public async Task TypeWriter([Remainder]string text = null)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Write)) {
                await Context.Message.DeleteAsync();
                if (text.Split(' ').Length > 10) {
                    await ReplyAsync($"Max words: `10`");
                    return;
                }
                await ReplyAsync("…");
                await Task.Delay(1000);
                IMessage msg = (await Context.Channel.GetMessagesAsync(Context.Message, Direction.After, 1).Flatten()).Where(x => x.Author.Id == Main.Daddy._client.CurrentUser.Id).Take(1).ToList().First();
                string message = string.Empty;
                if (text == null) return;
                foreach (var c in RemoveAllMention(text).Split(' ')) {
                    if (c == null || string.IsNullOrEmpty(c)) {
                        continue;
                    }
                    await Task.Delay(1000);
                    await (msg as IUserMessage).ModifyAsync(x => {
                        message += $"{c} ";
                        x.Content = message;
                    });
                    if (msg.Content.Equals("…") || string.IsNullOrEmpty(msg.Content)) {
                        break;
                    }
                }
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("ev", RunMode = RunMode.Async), Summary("Evaluate code")]
        public async Task Eval([Summary("Evaluating string"), Remainder()]string arg)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Eval)) {
                if (SpecialPeople(Context.User.Id)) {
                    try {
                        await _Eval(arg, false);
                    }
                    catch(Exception e) {
                        await Main.Daddy.Log("_Eval", exception: e);
                    }
                }
                else {
                    await ReplyAsync($"This command requires `BOT_OWNER` - you have `{GetHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
                }
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("Mev", RunMode = RunMode.Async), Summary("Evaluate code")]
        public async Task JsonEval([Remainder] string code)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Eval)) {
                if (SpecialPeople(Context.User.Id)) {
                    await _Eval(code, true, true);
                }
                else {
                    await ReplyAsync($"This command requires `BOT_OWNER` - you have `{GetHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
                }
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("?!Mev", RunMode = RunMode.Async), Summary("Evaluate code")]
        public async Task SingleStaticEval(string methodName, [Remainder] string code)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Eval)) {
                if (SpecialPeople(Context.User.Id)) {
                    await _Eval(code, true, true, true, methodName);
                }
                else {
                    await ReplyAsync($"This command requires `BOT_OWNER` - you have `{GetHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
                }
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("?Mev", RunMode = RunMode.Async), Summary("Evaluate code")]
        public async Task SingleEval(string methodName, [Remainder] string code)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Eval)) {
                if (SpecialPeople(Context.User.Id)) {
                    await _Eval(code, true, true, methodName: methodName);
                }
                else {
                    await ReplyAsync($"This command requires `BOT_OWNER` - you have `{GetHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
                }
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }


        [Command("!Mev", RunMode = RunMode.Async), Summary("Evaluate code")]
        public async Task StaticEval([Remainder] string code)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Eval)) {
                if (SpecialPeople(Context.User.Id)) {
                    await _Eval(code, true, true, true);
                }
                else {
                    await ReplyAsync($"This command requires `BOT_OWNER` - you have `{GetHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
                }
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("Tev", RunMode = RunMode.Async), Summary("Evaluate code")]
        public async Task MEval([Remainder] string code)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Eval)) {
                if (SpecialPeople(Context.User.Id)) {
                    await _Eval(code, true);
                }
                else {
                    await ReplyAsync($"This command requires `BOT_OWNER` - you have `{GetHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
                }
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("?Tev", RunMode = RunMode.Async), Summary("Evaluate code")]
        public async Task FiltJEval(string name, [Remainder] string code)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Eval)) {
                if (SpecialPeople(Context.User.Id)) {
                    await _Eval(code, true, methodName: name);
                }
                else {
                    await ReplyAsync($"This command requires `BOT_OWNER` - you have `{GetHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
                }
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("?ev", RunMode = RunMode.Async), Summary("Evaluate code")]
        public async Task FiltEval(string name, [Remainder] string code)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Eval)) {
                if (SpecialPeople(Context.User.Id)) {
                    await _Eval(code, false, methodName: name);
                }
                else {
                    await ReplyAsync($"This command requires `BOT_OWNER` - you have `{GetHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
                }
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("inspect"), RequireOwner()]
        public async Task Inspect([Remainder()] string path)
        {
            path = path.Trim(' ', '\n', '`'); //Remove code block tags

            try {
                Ext.Ext ext = new Ext.Ext(0);
                object value = ext.GetProperty((Context as SocketCommandContext), null, null, path);
                var builder = new StringBuilder();
                if (value != null) {
                    var type = value.GetType().GetTypeInfo();
                    builder.AppendLine("```");
                    builder.AppendLine($"[{type.Namespace}.{type.Name}]");
                    builder.AppendLine($"{ext.InspectProperty(value)}");

                    if (value is System.Collections.IEnumerable enumerable) {
                        var items = enumerable.Cast<object>().ToArray();
                        if (items.Length > 0) {
                            builder.AppendLine();
                            foreach (var item in enumerable)
                                builder.AppendLine($"- {ext.InspectProperty(item)}");
                        }
                    }
                    else {
                        var groups = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                .Where(x => x.GetIndexParameters().Length == 0)
                                .GroupBy(x => x.Name)
                                .OrderBy(x => x.Key)
                                .ToArray();
                        if (groups.Length > 0) {
                            builder.AppendLine();
                            int pad = groups.Max(x => x.Key.Length) + 1;
                            foreach (var group in groups)
                                builder.AppendLine($"{group.Key.PadRight(pad, ' ')}{ext.InspectProperty(group.First().GetValue(value))}");
                        }
                    }
                    builder.AppendLine("```");
                }
                else
                    builder.AppendLine("`null`");
                await ReplyAsync(builder.ToString());
            }
            catch (Exception ex) {
                await ReplyAsync($"Error: {ex.Message}");
            }
        }

        [Command("json", RunMode = RunMode.Async), Alias("reload", "load")]
        public async Task JSON()
        {
            if (SpecialPeople(Context.User.Id)) {
                await Context.Message.DeleteAsync();
                CreateJson(Context.Guild);
                CreateSettings(Context.Guild);
            }
            else {
                await ReplyAsync($"This command requires `BOT_OWNER` -> you have `{GetHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
            }
        }

        [Command("update", RunMode = RunMode.Async)]
        public async Task Update()
        {
            if (SpecialPeople(Context.User.Id)) {
                await Context.Message.DeleteAsync();
                if (!DoesExistSettings(Context.Guild)) { CreateSettings(Context.Guild); }
                foreach (string x in Directory.EnumerateFiles(@"Settings/", "*", SearchOption.AllDirectories).Select(Path.GetFileName)/*Directory.GetFiles(@"Settings/"*/) {
                    JObject rss = JObject.Parse(File.ReadAllText($@"Settings/{x}"));
                    JObject welcome = (JObject)rss["Settings"];
                    welcome["JoinRole"] = Mention2role(Ext._vMem._vMemory[Mention2role(x)][Settings.JoinRole].ToString());
                    File.WriteAllText($@"Settings/{x}", rss.ToString());
                }
                _vMem._run();
            }
            else {
                await ReplyAsync($"This command requires `BOT_OWNER` -> you have `{GetHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
            }
        }

        [Command("test", RunMode = RunMode.Async)]
        public async Task Test()
        {
            if (SpecialPeople(Context.User.Id)) {
                EmbedBuilder builder = new EmbedBuilder()
                {
                    Title = "<a:typing:401791987579355137> test <a:Typing:398791357973528577>",
                    Description = "<a:Typing:401791987579355137> test <a:typing:398791357973528577>",
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                }.WithCurrentTimestamp();
                await ReplyAsync((string)Ext._vMem._vMemory[Context.Guild.Id][Settings.Welcome]);
                await ReplyAsync((string)Ext._vMem._vMemory[Context.Guild.Id][Settings.Leave]);
                await ReplyAsync($"{Ext._vMem._vMemory[Context.Guild.Id][Settings.DMmsg]}");
                await ReplyAsync($"{Convert.ToInt32(Ext._vMem._vMemory[Context.Guild.Id][Settings.DMmsg]).Equals(116)}");
                await ReplyAsync($"{jSon.CheckPermChn((Context.Guild as IGuild), Context.Channel.Id, Commands.Welcome)}");
                await ReplyAsync(string.Empty, embed: builder.Build());
                await ReplyAsync("<a:typing:401791987579355137> test <a:Typing:398791357973528577> - <a:Typing:401791987579355137> test <a:typing:398791357973528577>");
            }
            else {
                await ReplyAsync($"This command requires `BOT_OWNER` -> you have `{GetHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
            }
        }

        [Command("host", RunMode = RunMode.Async)]
        public async Task _lmao(string ayy)
        {
            if (SpecialPeople(Context.User.Id))
            {
                if (Context.Guild.Id == 215601994507878402)
                {
                    await Task.Delay(1250);
                    await ReplyAsync("System.BadRequestException: Object returned bad request: 403 Forbidden. at Daddy.Main.Daddy.<Host_Bot_VPS>d_43.MoveNext()");
                }
            }
            else
            {
                await ReplyAsync($"This command requires `BOT_OWNER` -> you have `{GetHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
            }
        }

        [Command("role", RunMode = RunMode.Async), Ext.Ratelimit(1, 1, Ext.Measure.Minutes), RequireUserPermissionAttribute(GuildPermission.Administrator)]
        public async Task SetRole([Summary("role")]IRole arg)
        {
            await Context.Message.DeleteAsync();
            if (IsVip(Context.User as SocketUser)) {
                if (!DoesExistSettings(Context.Guild)) { CreateSettings(Context.Guild); }
                JObject rss = JObject.Parse(File.ReadAllText($@"Settings/settings_{Context.Guild.Id}.json"));
                JObject welcome = (JObject)rss["Settings"];
                try {
                    welcome["JoinRole"] = arg.Id;
                }
                catch {
                    welcome.TryAdd("JoinRole", arg.Id);
                }
                File.WriteAllText($@"Settings/settings_{Context.Guild.Id}.json", rss.ToString());
                _vMem._run();
                await ReplyAsync($"New default join role is: `{arg.ToString()}`");
            }
        }

        [Command("log", RunMode = RunMode.Async), Ext.Ratelimit(1, 1, Ext.Measure.Minutes), RequireUserPermissionAttribute(GuildPermission.Administrator)]
        public async Task SetLog([Summary("channel")]IMessageChannel arg)
        {
            await Context.Message.DeleteAsync();
            if (IsVip(Context.User as SocketUser)) {
                if (!DoesExistSettings(Context.Guild)) { CreateSettings(Context.Guild); }
                JObject rss = JObject.Parse(File.ReadAllText($@"Settings/settings_{Context.Guild.Id}.json"));
                JObject welcome = (JObject)rss["Settings"];
                try {
                    welcome["LogCHN"] = arg.Id;
                }
                catch {
                    welcome.TryAdd("LogCHN", arg.Id);
                }
                File.WriteAllText($@"Settings/settings_{Context.Guild.Id}.json", rss.ToString());
                _vMem._run();
                await ReplyAsync($"New log channel is: `{arg.ToString()}`");
            }
        }

        [Command("wchannel", RunMode = RunMode.Async), Ext.Ratelimit(1, 1, Ext.Measure.Minutes), RequireUserPermissionAttribute(GuildPermission.Administrator)]
        public async Task SetWelcome([Summary("channel")]IMessageChannel arg)
        {
            await Context.Message.DeleteAsync();
            if (IsVip(Context.User as SocketUser))
            {
                if (!DoesExistSettings(Context.Guild)) { CreateSettings(Context.Guild); }
                JObject rss = JObject.Parse(File.ReadAllText($@"Settings/settings_{Context.Guild.Id}.json"));
                JObject welcome = (JObject)rss["Settings"];
                try
                {
                    welcome["WelcomeCHN"] = arg.Id;
                }
                catch
                {
                    welcome.TryAdd("WelcomeCHN", arg.Id);
                }
                File.WriteAllText($@"Settings/settings_{Context.Guild.Id}.json", rss.ToString());
                _vMem._run();
                await ReplyAsync($"New welcome channel is: `{arg.ToString()}`");
            }
        }

        public async Task SetWelcome_main(IMessageChannel arg)
        {
            if (!DoesExistSettings((arg as SocketGuildChannel).Guild)) { CreateSettings((arg as SocketGuildChannel).Guild); }
            JObject rss = JObject.Parse(File.ReadAllText($@"Settings/settings_{(arg as SocketGuildChannel).Guild.Id}.json"));
            JObject welcome = (JObject)rss["Settings"];
            try
            {
                welcome["WelcomeCHN"] = arg.Id;
            }
            catch
            {
                welcome.TryAdd("WelcomeCHN", arg.Id);
            }
            File.WriteAllText($@"Settings/settings_{(arg as SocketGuildChannel).Guild.Id}.json", rss.ToString());
            await Task.CompletedTask;
        }

        [Command("schar", RunMode = RunMode.Async)]
        public async Task AntiCharSpam([Summary("char spam"), Remainder()]int arg)
        {
            if (IsVip(Context.User as SocketUser)) {
                await Context.Message.DeleteAsync();
                if (!DoesExistSettings(Context.Guild)) { CreateSettings(Context.Guild); }
                JObject rss = JObject.Parse(File.ReadAllText($@"Settings/settings_{Context.Guild.Id}.json"));
                JObject welcome = (JObject)rss["Settings"];
                try {
                    welcome["CharSpam"] = arg;
                }
                catch {
                    welcome.TryAdd("CharSpam", arg);
                }
                File.WriteAllText($@"Settings/settings_{Context.Guild.Id}.json", rss.ToString());
                _vMem._run();
                await ReplyAsync($"New message char spam trigger is: `{arg}`");
            }
        }

        [Command("sword", RunMode = RunMode.Async)]
        public async Task AntiWordSpam([Summary("word spam"), Remainder()]int arg)
        {
            if (IsVip(Context.User as SocketUser)) {
                await Context.Message.DeleteAsync();
                if (!DoesExistSettings(Context.Guild)) { CreateSettings(Context.Guild); }
                JObject rss = JObject.Parse(File.ReadAllText($@"Settings/settings_{Context.Guild.Id}.json"));
                JObject welcome = (JObject)rss["Settings"];
                try {
                    welcome["WordSpam"] = arg;
                }
                catch {
                    welcome.TryAdd("WordSpam", arg);
                }
                File.WriteAllText($@"Settings/settings_{Context.Guild.Id}.json", rss.ToString());
                _vMem._run();
                await ReplyAsync($"New message word spam trigger is: `{arg}`");
            }
        }

        [Command("semoji", RunMode = RunMode.Async)]
        public async Task AntiEmojiSpam([Summary("word spam"), Remainder()]int arg)
        {
            if (IsVip(Context.User as SocketUser)) {
                await Context.Message.DeleteAsync();
                if (!DoesExistSettings(Context.Guild)) { CreateSettings(Context.Guild); }
                JObject rss = JObject.Parse(File.ReadAllText($@"Settings/settings_{Context.Guild.Id}.json"));
                JObject welcome = (JObject)rss["Settings"];
                try {
                    welcome["EmojiSpam"] = arg;
                }
                catch {
                    welcome.TryAdd("EmojiSpam", arg);
                }
                File.WriteAllText($@"Settings/settings_{Context.Guild.Id}.json", rss.ToString());
                _vMem._run();
                await ReplyAsync($"New message emoji spam trigger is: `{arg}`");
            }
        }

        [Command("welcome", RunMode = RunMode.Async)]
        public async Task Welcome([Summary("msg"), Remainder()]string arg)
        {
            if (IsVip(Context.User as SocketUser) && !string.IsNullOrEmpty(jSon.Settings(Context.Guild, Settings.Welcome))) {
                await Context.Message.DeleteAsync();
                if (!DoesExistSettings(Context.Guild)) { CreateSettings(Context.Guild); }
                JObject rss = JObject.Parse(File.ReadAllText($@"Settings/settings_{Context.Guild.Id}.json"));
                JObject welcome = (JObject)rss["Settings"];
                try {
                    welcome["Welcome"] = arg;
                }
                catch {
                    welcome.TryAdd("Welcome", arg);
                }
                File.WriteAllText($@"Settings/settings_{Context.Guild.Id}.json", rss.ToString());
                _vMem._run();
                await ReplyAsync($"New welcome messages is: `{arg}`");
            }
        }

        [Command("leave", RunMode = RunMode.Async)]
        public async Task Leave([Summary("msg"), Remainder()]string arg)
        {
            if (IsVip(Context.User as SocketUser) && !string.IsNullOrEmpty(jSon.Settings(Context.Guild, Settings.Leave))) {
                await Context.Message.DeleteAsync();
                if (!DoesExistSettings(Context.Guild)) { CreateSettings(Context.Guild); }
                JObject rss = JObject.Parse(File.ReadAllText($@"Settings/settings_{Context.Guild.Id}.json"));
                JObject welcome = (JObject)rss["Settings"];
                try {
                    welcome["Leave"] = arg;
                }
                catch {
                    welcome.TryAdd("Leave", arg);
                }
                File.WriteAllText($@"Settings/settings_{Context.Guild.Id}.json", rss.ToString());
                _vMem._run();
                await ReplyAsync($"New leave message is: `{arg}`");
            }
        }

        [Command("dm", RunMode = RunMode.Async), RequireBotPermission(GuildPermission.KickMembers)]
        public async Task DmSendcmd([Summary("t/f"), Remainder()]char arg)
        {
            if (IsVip(Context.User as SocketUser)) {
                await Context.Message.DeleteAsync();
                if (!arg.Equals('t') && !arg.Equals('f')) { await ReplyAsync("`t/f`"); return; }
                if (!DoesExistSettings(Context.Guild)) { CreateSettings(Context.Guild); }
                JObject rss = JObject.Parse(File.ReadAllText($@"Settings/settings_{Context.Guild.Id}.json"));
                JObject welcome = (JObject)rss["Settings"];
                try {
                    welcome["DMmsg"] = arg;
                }
                catch {
                    welcome.TryAdd("DMmsg", arg);
                }
                File.WriteAllText($@"Settings/settings_{Context.Guild.Id}.json", rss.ToString());
                _vMem._run();
                await ReplyAsync($"DMing set to: `{arg.ToString().Replace("t", "TRUE").Replace("f", "FALSE")}`");
            }
        }

        [Command("prefix", RunMode = RunMode.Async), RequireBotPermission(GuildPermission.Administrator)]
        public async Task Prefix([Summary("prefix"), Remainder()]char arg)
        {
            if (IsVip(Context.User as SocketUser)) {
                await Context.Message.DeleteAsync();
                if (Convert.ToChar(jSon.Settings(Context.Guild, Settings.Prefix)).Equals(arg)) {
                    await ReplyAsync($"`{arg}` is already server prefix!");
                    return;
                }
                if (!DoesExistSettings(Context.Guild)) { CreateSettings(Context.Guild); }
                JObject rss = JObject.Parse(File.ReadAllText($@"Settings/settings_{Context.Guild.Id}.json"));
                JObject welcome = (JObject)rss["Settings"];
                try {
                    welcome["Prefix"] = arg.ToString();
                }
                catch {
                    welcome.TryAdd("Prefix", arg.ToString());
                }
                File.WriteAllText($@"Settings/settings_{Context.Guild.Id}.json", rss.ToString());
                _vMem._run();
                await ReplyAsync($"New server prefix is: `{arg}`");
            }
        }

        [Command("cenable", RunMode = RunMode.Async), Alias("callow"), RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task _cEnable([Summary("command")]string arg)
        {
            if (IsVip(Context.User as SocketUser)) {
                await Context.Message.DeleteAsync();
                if (Enum.IsDefined(typeof(Commands), FirstCharToUpper(arg))) {
                    if (!DoesExistJson(Context.Guild)) { CreateJson(Context.Guild); }
                    JObject rss = JObject.Parse(File.ReadAllText($@"json/jayson_{Context.Guild.Id}.json"));
                    JObject channel = (JObject)rss[Context.Channel.Id];
                    channel[FirstCharToUpper(arg)] = true;
                    File.WriteAllText($@"json/jayson_{Context.Guild.Id}.json", rss.ToString());
                    await ReplyAsync($"{FirstCharToUpper(arg)} is enabled!");
                }
                else {
                    await ReplyAsync($"`{FirstCharToUpper(arg)}` doesn't exist!");
                }
            }
        }

        [Command("enable", RunMode = RunMode.Async), Alias("allow"), RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Enable([Summary("command")]string arg)
        {
            if (IsVip(Context.User as SocketUser)) {
                await Context.Message.DeleteAsync();
                if (Enum.IsDefined(typeof(Commands), FirstCharToUpper(arg))) {
                    if (!DoesExistJson(Context.Guild)) { CreateJson(Context.Guild); }
                    JObject rss = JObject.Parse(File.ReadAllText($@"json/jayson_{Context.Guild.Id}.json"));
                    foreach (var x in rss) {
                        x.Value[FirstCharToUpper(arg)] = true;
                    }
                    File.WriteAllText($@"json/jayson_{Context.Guild.Id}.json", rss.ToString());
                    await ReplyAsync($"{FirstCharToUpper(arg)} is enabled!");
                }
                else {
                    await ReplyAsync($"`{FirstCharToUpper(arg)}` doesn't exist!");
                }
            }
        }

        [Command("cdisable", RunMode = RunMode.Async), Alias("cforbid"), RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task _cDisable([Summary("command")]string arg)
        {
            if (IsVip(Context.User as SocketUser)) {
                await Context.Message.DeleteAsync();
                if (Enum.IsDefined(typeof(Commands), FirstCharToUpper(arg))) {
                    if (!DoesExistJson(Context.Guild)) { CreateJson(Context.Guild); }
                    JObject rss = JObject.Parse($"{File.ReadAllText($@"json/jayson_{Context.Guild.Id}.json")}");
                    JObject channel = (JObject)rss[Context.Channel.Id];
                    channel[FirstCharToUpper(arg)] = false;
                    File.WriteAllText($@"json/jayson_{Context.Guild.Id}.json", rss.ToString());
                    await ReplyAsync($"{FirstCharToUpper(arg)} is disabled!");
                }
                else {
                    await ReplyAsync($"`{FirstCharToUpper(arg)}` doesn't exist in json file");
                }
            }
        }

        [Command("disable", RunMode = RunMode.Async), Alias("forbid"), RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Disable([Summary("command")]string arg)
        {
            if (IsVip(Context.User as SocketUser)) {
                await Context.Message.DeleteAsync();
                if (Enum.IsDefined(typeof(Commands), FirstCharToUpper(arg))) {
                    if (!DoesExistJson(Context.Guild)) { CreateJson(Context.Guild); }
                    JObject rss = JObject.Parse($"{File.ReadAllText($@"json/jayson_{Context.Guild.Id}.json")}");
                    foreach (var x in rss) {
                        x.Value[FirstCharToUpper(arg)] = false;
                    }
                    File.WriteAllText($@"json/jayson_{Context.Guild.Id}.json", rss.ToString());
                    await ReplyAsync($"{FirstCharToUpper(arg)} is disabled!");
                }
                else {
                    await ReplyAsync($"`{FirstCharToUpper(arg)}` doesn't exist in json file");
                }
            }
        }

        [Command("gjson", RunMode = RunMode.Async)]
        public async Task Gjson([Summary("argument"), Remainder()]string arg)
        {
            if (SpecialPeople(Context.User.Id)) {
                string[] a = arg.Split(new string[] { "->" }, StringSplitOptions.None);
                JObject rss = null;
                try {
                    rss = JObject.Parse($"{File.ReadAllText($@"{a[0]}/{a[1]}_{Context.Guild.Id}.json")}");
                }
                catch {
                    await ReplyAsync($"Doesn't exist!\narg0: `{a[0]}`\narg1: `{a[1]}`\narg2: `{a[2]}`\n");
                    return;
                }
                if (a.Length.Equals(3)) {
                    await ReplyAsync($"```json\n{rss[a[2]]}```");
                    return;
                }
                else {
                    foreach (var x in rss) {
                        await ReplyAsync($"```json\n{x.Key}\n{x.Value}```");
                    }
                }
            }
            else {
                await ReplyAsync($"This command requires `BOT_OWNER` - you have `{GetHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
            }
        }

        [Command("eval", RunMode = RunMode.Async)]
        public async Task _fEval([Summary("argument"), Remainder()]string arg)
        {
            if (SpecialPeople(Context.User.Id)) {
                string[] a = arg.Split(new string[] { "->" }, StringSplitOptions.None);
                object[] args = a.Skip(3).Take(a.Length - 3).ToArray();
                if (a.Length <= 3) {
                    args = null;
                }
                if (a[0].Equals("d")) {
                    Main.Daddy _d = new Main.Daddy();
                    if (a[1].Equals("r")) {
                        await ReplyAsync($"{typeof(Main.Daddy).GetMethod(a[2]).Invoke(_d, args)}");
                    }
                    else if (a[1].Equals("e")) {
                        //typeof(Main.Daddy).GetMethod(a[2]).Invoke(_d, args);
                        //await ExtensionMethods.InvokeAsync(typeof(Main.Daddy).GetMethod(a[2]), _d, args);
                    }
                }
                else if (a[0].Equals("bc")) {
                    BaseCommands _bc = new BaseCommands();
                    if (a[1].Equals("r")) {
                        await ReplyAsync($"{typeof(BaseCommands).GetMethod(a[2]).Invoke(_bc, args)}");
                    }
                    else if (a[1].Equals("a")) {
                        switch (a[2]) {
                            case "Info":
                                await Task.Run(Info);
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
            else {
                await ReplyAsync($"This command requires `BOT_OWNER` - you have `{GetHighestPerm((Context.User as SocketGuildUser).GuildPermissions)}`.");
            }
        }

        public string GetText(object text)
        {
            return text.ToString();
        }

        [Command("info", RunMode = RunMode.Async), Summary("Gives some info about server"), Ext.Ratelimit(1, 1, Ext.Measure.Minutes)]
        public async Task Info()
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Info)) {
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
                    Value = $"{((SocketGuild)Context.Guild).Channels.Count} [{((SocketGuild)Context.Guild).VoiceChannels.Count} :microphone2:]"//new List<IGuildChannel>(await Context.Guild.GetChannelsAsync()).Count()
                };
                EmbedAuthorBuilder author = new EmbedAuthorBuilder()
                {
                    Name = "DaddyBot",
                    IconUrl = Main.Daddy._client.CurrentUser.GetAvatarUrl()
                };
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Author = author,
                    Title = $"{Context.Guild.Name}'s informations.",
                    Description = $"Bot made by: [Pat](http://daddybot.me/)\nBot made for: [M&D](https://discord.gg/D6qd4BE)\nBot created: {Main.Daddy._client.CurrentUser.CreatedAt}",
                    Fields = new List<EmbedFieldBuilder>() { field1, field2, field3, field4, field5, field6 },
                    ThumbnailUrl = Context.Guild.IconUrl,
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                }.WithCurrentTimestamp();
                sw.Stop();
                await ReplyAsync(string.Empty, embed: embed.WithFooter(x => x.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("uinfo", RunMode = RunMode.Async), Alias("dox", "whois", "stats"), Summary("Gives some info about user"), Ext.Ratelimit(1, 1, Ext.Measure.Minutes)]
        public async Task UserInfo([Remainder()]IGuildUser user = null)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Info)) {
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
                    Fields = new List<EmbedFieldBuilder>() { field1, field2, field3, field4, field5, field6 },
                    ThumbnailUrl = user.GetAvatarUrl(ImageFormat.Auto, (ushort)256),
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                }.WithThumbnailUrl(user.GetAvatarUrl(ImageFormat.Auto, (ushort)256)).WithCurrentTimestamp();
                sw.Stop();
                await ReplyAsync(string.Empty, embed: embed.WithFooter(x => x.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("help", RunMode = RunMode.Async), Summary("A list of cmds."), Ext.Ratelimit(1, 1, Ext.Measure.Minutes)]
        public async Task Help()
        {
            await ReplyAsync($"{Context.User.Mention} Please check your DM for a list of commands!");
            IDMChannel x = await (Context.User as SocketUser).GetOrCreateDMChannelAsync();
            await x.SendMessageAsync(Database.JSON.GetHelp1(), false);
            await Task.Delay(250);
            IDMChannel y = await (Context.User as SocketUser).GetOrCreateDMChannelAsync();
            await y.SendMessageAsync(Database.JSON.GetHelp2(), false);
            await Context.Message.DeleteAsync();
        }

        [Command("invite", RunMode = RunMode.Async), Summary("A list of cmds."), Ext.Ratelimit(1, 1, Ext.Measure.Minutes)]
        public async Task Invite()
        {
            await ReplyAsync($"{Context.User.Mention} Please check your DM for an invite link!");
            IDMChannel x = await (Context.User as SocketUser).GetOrCreateDMChannelAsync();
            await x.SendMessageAsync("https://discordapp.com/oauth2/authorize?client_id=215626696202780672&scope=bot&permissions=2146958591", false);
            await Context.Message.DeleteAsync();
        }

        [Command("avatar", RunMode = RunMode.Async), Alias("pfp"), Summary("Gets avatar."), Ext.Ratelimit(1, 1, Ext.Measure.Minutes)]
        public async Task Avatar([Summary("User")]IUser arg = null)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Avatar)) {
                var sw = Stopwatch.StartNew();
                EmbedBuilder builder = new EmbedBuilder();
                if (arg != null) {
                    builder.Title = arg.Username;
                    builder.WithImageUrl(arg.GetAvatarUrl(ImageFormat.Auto, (ushort)256));//256
                }
                else {
                    builder.Title = Context.User.Username;
                    builder.WithImageUrl(Context.User.GetAvatarUrl(ImageFormat.Auto, (ushort)256));
                }
                builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                builder.WithCurrentTimestamp();
                sw.Stop();
                await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                await Context.Message.DeleteAsync();
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("channels", RunMode = RunMode.Async), Alias("channel"), Summary("Gets channel IDs."), Ext.Ratelimit(1, 1, Ext.Measure.Minutes)]
        public async Task Channels([Summary("t/v")]string type = null)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Channels)) {
                var sw = Stopwatch.StartNew();
                StringBuilder str0 = new StringBuilder();
                StringBuilder str1 = new StringBuilder();
                IEnumerable<IGuildChannel> channels = null;
                if (string.IsNullOrEmpty(type) || type.ToLower().Equals("t")) {
                    channels = (await Context.Guild.GetChannelsAsync()).Where(x => x is SocketTextChannel).ToList().OrderBy(x => x.Position);
                }
                else if (type.ToLower().Equals("v")) {
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
                EmbedBuilder builder = new EmbedBuilder()
                {
                    Fields = new List<EmbedFieldBuilder>() { field0, field1 },
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                }.WithCurrentTimestamp();
                sw.Stop();
                await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("roles", RunMode = RunMode.Async), Alias("role"), Summary("Gets role IDs."), Ext.Ratelimit(1, 1, Ext.Measure.Minutes)]
        public async Task Roles()
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Role)) {
                var sw = Stopwatch.StartNew();
                StringBuilder str0 = new StringBuilder();
                StringBuilder str1 = new StringBuilder();
                IEnumerable<IRole> roles = (Context.Guild as SocketGuild).Roles.Where(x => !x.IsEveryone).OrderByDescending(y => y.Position);
                roles.ToList().ForEach(x => str0.AppendLine($"{x.Name} [{(x as SocketRole).Members.Count()}]"));
                roles.ToList().ForEach(x => str1.AppendLine($"{x.Id}"));
                EmbedFieldBuilder field0 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "Role [Count]",
                    Value = str0.ToString()
                };
                EmbedFieldBuilder field1 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = "ID",
                    Value = str1.ToString()
                };
                EmbedBuilder builder = new EmbedBuilder()
                {
                    Fields = new List<EmbedFieldBuilder>() { field0, field1},
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                }.WithCurrentTimestamp();
                sw.Stop();
                await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("perms", RunMode = RunMode.Async), Alias("permissions"), Summary("Gets perms [API]"), Ext.Ratelimit(1, 1, Ext.Measure.Minutes)]
        public async Task Perms()
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Permissions)) {
                await Context.Message.DeleteAsync();
                var sw = Stopwatch.StartNew();
                StringBuilder str0 = new StringBuilder();
                GuildPermissions perms = (Context.User as SocketGuildUser).GuildPermissions;
                perms.ToList().ForEach(x => str0.AppendLine($"{x}"));
                EmbedFieldBuilder field0 = new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = $"Permissions - {Context.User.Username}",
                    Value = str0.ToString()
                };
                EmbedBuilder builder = new EmbedBuilder()
                {
                    Fields = new List<EmbedFieldBuilder>() { field0 },
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                }.WithCurrentTimestamp();
                sw.Stop();
                await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("random", RunMode = RunMode.Async), Summary("Random user"), Ext.Ratelimit(3, 0.5, Ext.Measure.Minutes)]
        public async Task RandomUser()
        {
            await Context.Message.DeleteAsync();
            if (IsVip(Context.User as SocketGuildUser) && jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Random)) {
                IGuildUser user = (await Context.Guild.GetUsersAsync()).Where(x => x is SocketGuildUser).ToList()[_ran.Next(((SocketGuild)Context.Guild).MemberCount)];
                await ReplyAsync($"{user.Mention}");
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("disc", RunMode = RunMode.Async), Summary("Discriminator search"), Ext.Ratelimit(1, 0.1, Ext.Measure.Minutes)]
        public async Task DiscriminatorSearch()
        {
            await Context.Message.DeleteAsync();
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Discriminator)) {
                bool y = true;
                StringBuilder sb = new StringBuilder();
                SocketGuildUser me = (Context.User as SocketGuildUser);
                (await Context.Client.GetGuildsAsync()).ToList().ForEach(async g => {
                    (await g.GetUsersAsync()).Where(x => x is SocketGuildUser).ToList().ForEach(z => {
                        if (z.DiscriminatorValue.Equals(me.DiscriminatorValue) && !z.Username.Equals(me.Username)) {
                            y = false;
                            if (!sb.ToString().Contains(z.Username)) {
                                sb.AppendLine($"{z.Username} - {z.DiscriminatorValue}");
                            }
                        }
                    });
                });
                if (!y) {
                    await ReplyAsync(sb.ToString());
                }
                else {
                    await ReplyAsync($"`No users found!`");
                }
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        [Command("top", RunMode = RunMode.Async), Alias("users"), Summary("Gets first users"), Ext.Ratelimit(1, 5, Ext.Measure.Minutes)]
        public async Task TopUsers([Summary("Number")]int users2find = 10)
        {
            if (jSon.CheckPermChn(Context.Guild, Context.Channel.Id, Commands.Top)) {
                await Context.Message.DeleteAsync();
                if (users2find > 25 || users2find > (Context.Guild as SocketGuild).MemberCount || users2find <= 0) {
                    await ReplyAsync("Whoa, slow down there buddy!");
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
                EmbedBuilder builder = new EmbedBuilder()
                {
                    Fields = new List<EmbedFieldBuilder>() { field0, field1 },
                    Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                }.WithCurrentTimestamp();
                sw.Stop();
                await ReplyAsync(string.Empty, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
            else {
                await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
            }
        }

        public void Fuckoff() => Console.WriteLine("fuck off");

        [Command("gamble", RunMode = RunMode.Async), Alias("roll"), Ext.Ratelimit(3, 0.08, Ext.Measure.Minutes)]
        public async Task _gamble(int amount)
        {
            await Context.Message.DeleteAsync();
            if (amount > 0 && amount < 100) {
                int random = _ran.Next(101);
                if (!token.Equals(0)) {
                    if (Context.User.Id.Equals(token)) {
                        await ReplyAsync($"{Context.User.Mention} You won! {amount}%");
                        token = 0;
                        return;
                    }
                }
                if (random <= amount) {
                    await ReplyAsync($"{Context.User.Mention} You won! {amount}%");
                }
                else {
                    await ReplyAsync($"{Context.User.Mention} You lost! {amount}%");
                }
            }
            else {
                await ReplyAsync($"{Context.User.Mention}\n```cpp\n<int> 0-100\n```");
            }
        }

        [Command("gambIe", RunMode = RunMode.Async), Alias("roII"), Ext.Ratelimit(3, 0.08, Ext.Measure.Minutes)]
        public async Task _gamble_fake(int amount)
        {
            await Context.Message.DeleteAsync();
            if (amount > 0 && amount < 100)
            {
                int random = _ran.Next(101);
                await ReplyAsync($"{Context.User.Mention} You won! {amount}%");
            }
            else
            {
                await ReplyAsync($"{Context.User.Mention}\n```cpp\n<int> 0-100\n```");
            }
        }

        private async Task _Eval(string code, bool withType = false, bool methods = false, bool statics = false, string methodName = null)
        {
            await Context.Message.DeleteAsync();
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
            try {
                var ctask = CSharpScript.EvaluateAsync(code, globals: globals, cancellationToken: cts.Token, options: scriptOptions);
                ctask.Wait(cts.Token);
                result = await ctask;
                if (result != null) {
                    //prog.ObjectMemory.Push(result);
                }
                type = Format.Sanitize(Ext.TypeInspector.VisualizeType(result));
                if (methods) {
                    vizual.Append(Ext.TypeInspector.VisualizeMethods(result, !statics, methodName));
                }
                else {
                    vizual.Append(Ext.TypeInspector.VisualizeValue(result, withType, findMember: methodName));
                }
                await Main.Daddy.Log(vizual.ToString());
                color = Success;
                success = true;
                //await Context.Message.DeleteAsync();
            }
            catch (CompilationErrorException e) {
                await Main.Daddy.Log("Error", exception: e);

                foreach (var item in e.Diagnostics) {
                    if (withType) {
                        vizual.AppendLine(Format.Bold("Error"));
                        vizual.AppendLine(Format.Code(Ext.Extensions.StripBlock(item.ToString())));
                    }
                    source.OnNext(new Ext.FieldValue { Content = item.ToString(), Code = "", Title = "Error" });
                }
                if (withType) {
                    await ReplyAsync(vizual.ToString());
                    return;
                }
                color = Error;
            }
            catch (OperationCanceledException e) {
                await Main.Daddy.Log("Error", exception: e);
                source.OnNext(new Ext.FieldValue { Content = e.Message, Code = "", Title = "Timeout" });
                color = Warning;
            }
            catch (JsonSerializationException e) {
                await Main.Daddy.Log("Error", exception: e);
                source.OnNext(new Ext.FieldValue { Content = e.Message, Code = "", Title = "Error" });
                color = Error;
            }
            catch (TargetInvocationException e) {
                await Main.Daddy.Log("Error", exception: e);
                source.OnNext(new Ext.FieldValue { Content = e.InnerException.Message, Code = "", Title = "Error" });
                color = Error;
            }
            catch (Exception e) {
                await Main.Daddy.Log("Error", exception: e);
                source.OnNext(new Ext.FieldValue { Content = e.Message, Code = "", Title = "Error" });
                color = Error;
            }

            try {
                var lastpart = "";
                if (withType) {
                    var s = new StringBuilder();
                    s.AppendLine(Format.Bold("Input"));
                    s.AppendLine(Format.Code(Ext.Extensions.StripBlock(code), "cs"));
                    s.AppendLine(Format.Bold($"Result<{type}>"));
                    await ReplyAsync(s.ToString());
                }

                // Build message
                if (success) {
                    if (vizual.Length > (withType ? 1800 : 1000)) {
                        var vizString = vizual.ToString();
                        var header = vizString.TrimStart('\r', '\n', ' ', '\t').Split(new char[] { '\n' }, 2)[0];
                        vizString = vizString.Substring(header.Length + 1);
                        var continuation = false;
                        while (true) {
                            var end = vizString.IndexOf('\n', Math.Min(withType ? 1800 : 800, vizString.Length));
                            if (end < 0) {
                                end = vizString.Length;
                            }
                            var part = vizString.Substring(0, end);
                            vizString = vizString.Substring(Math.Min(end + 1, vizString.Length));
                            if (continuation) {
                                source.OnNext(new Ext.FieldValue { Content = $"{header}\n{part}", Code = "", Title = $"Continuation<{type}>" });
                            }
                            else {
                                source.OnNext(new Ext.FieldValue { Content = $"{header}\n{part}", Code = "", Title = $"Result<{type}>" });
                            }
                            if (vizString.Length <= 0) {
                                lastpart = $"{header}\n{part}";
                                break;
                            }
                            // Send

                            source.OnCompleted();
                            var res = (withType) ? await ReplyAsync(Format.Code(Ext.Extensions.StripBlock($"{header}\n{part}"))) : await ReplyAsync("", false, (await embedbuilder).WithColor(color));
                            await Main.Daddy.Log(res.Content);
                            source = new Subject<Ext.FieldValue>();
                            embedbuilder = Ext.Extensions.BuildBotAnswer(source);
                            continuation = true;
                        }
                    }
                    else {
                        lastpart = vizual.ToString();
                        source.OnNext(new Ext.FieldValue { Content = lastpart, Code = "", Title = $"Result<{type}>" });
                    }
                }
                else {
                    if (withType) {
                        source.OnCompleted();
                        await ReplyAsync("", embed: (await embedbuilder).WithColor(color));
                        return;
                    }
                }
                if (type.Equals("String")) {
                    string str = result.ToString();
                    source.OnNext(new Ext.FieldValue { Content = str.Substring(0, Math.Min(str.Length, 500)), Code = "", Title = "Value" });
                }

                source.OnCompleted();
                sw.Stop();
                var reply = (withType) ? await ReplyAsync(Format.Code(lastpart)) : await ReplyAsync("", false, (await embedbuilder).WithColor(color).WithFooter(x => x.WithText($"In {sw.ElapsedMilliseconds}ms")));
                await Main.Daddy.Log(reply.Content);
            }
            catch (Exception e) {
                await Main.Daddy.Log("Error", exception: e);
                source = new Subject<Ext.FieldValue>();
                embedbuilder = Ext.Extensions.BuildBotAnswer(source);
                source.OnNext(new Ext.FieldValue { Content = e.Message, Code = "", Title = "Error" });
                source.OnCompleted();
                color = Error;
                await ReplyAsync("", false, (await embedbuilder).WithColor(color).WithFooter(x => x.WithText($"In {sw.ElapsedMilliseconds}ms")));
            }
        }

        public static async Task SendReasonEmbedToUserDM(SocketUser user, IGuild guild, string reason, Commands r)
        {
            JSON jSon = new JSON();
            if ((Convert.ToInt32(Ext._vMem._vMemory[guild.Id][Settings.DMmsg]).Equals(116))) {
                EmbedBuilder builder = new EmbedBuilder();
                var sw = Stopwatch.StartNew();
                switch (r) {
                    case Commands.Mute:
                        builder.Description = $"**You are Muted from {guild.Name}!**{(string.IsNullOrEmpty(reason) ? string.Empty : $"\n**Reason: {reason}**")}";
                        break;
                    case Commands.Kick:
                        builder.Description = $"**You are Kicked from {guild.Name}!**{(string.IsNullOrEmpty(reason) ? string.Empty : $"\n**Reason: {reason}**")}";
                        break;
                    case Commands.Softban:
                        builder.Description = $"**You are Softbanned from {guild.Name}!**{(string.IsNullOrEmpty(reason) ? string.Empty : $"\n**Reason: {reason}**")}";
                        break;
                    case Commands.Ban:
                        builder.Description = $"**You are Banned from {guild.Name}!**{(string.IsNullOrEmpty(reason) ? string.Empty : $"\n**Reason: {reason}**")}";
                        break;
                    case Commands.Normie:
                        builder.Description = $"**You are Normied in {guild.Name}!**{(string.IsNullOrEmpty(reason) ? string.Empty : $"\n**Reason: {reason}**")}";
                        break;
                    default:
                        builder.Description = $"**null**";
                        break;
                }
                builder.WithAuthor(new EmbedAuthorBuilder() { IconUrl = user.GetAvatarUrl() });
                builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
                IDMChannel x = await user.GetOrCreateDMChannelAsync();
                builder.WithCurrentTimestamp();
                sw.Stop();
                await x.SendMessageAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
            }
        }

        public static string FirstCharToUpper(string arg) => (String.IsNullOrEmpty(arg) ? throw new ArgumentException("ARGH!") : arg.First().ToString().ToUpper() + arg.Substring(1));

        public static Stream GetStreamFromUrl(string url)
        {
            WebRequest req = WebRequest.Create(url);
            Task<WebResponse> res = Task.Factory.FromAsync(req.BeginGetResponse, x => req.EndGetResponse(x), (object)null);
            return res.ContinueWith(x => x.Result.GetResponseStream()).Result;
        }

        public string GetHighestPerm(GuildPermissions perms)
        {
            if (perms.Administrator) {
                return "ADMIN";
            }
            else if (perms.ManageRoles) {
                return "MOD";
            }
            else {
                return "USER";
            }
        }

        public static bool IsVip(SocketUser user) => ((user as SocketGuildUser).Roles.Where(x => x.Position > x.Guild.CurrentUser.Hierarchy).Count() > 0);

        public static bool DoesExistJson(IGuild arg) => File.Exists($@"json/jayson_{arg.Id}.json");

        public static async void CreateJson(IGuild arg)
        {
            BaseCommands _bc = new BaseCommands();
            IEnumerable<IGuildChannel> channels = (await arg.GetChannelsAsync()).Where(y => y is SocketTextChannel).ToList().OrderBy(y => y.Position);
            Dictionary<ulong, Dictionary<Commands, bool>> perm = new Dictionary<ulong, Dictionary<Commands, bool>>();
            channels.ToList().ForEach(z => perm.Add(z.Id, _bc.inPerm));
            File.WriteAllText($@"json/jayson_{arg.Id}.json", JsonConvert.SerializeObject(perm, Formatting.Indented));
        }

        public static bool DoesExistSettings(IGuild arg) => File.Exists($@"Settings/settings_{arg.Id}.json");

        public static void CreateSettings(IGuild arg)
        {
            BaseCommands _bc = new BaseCommands();
            File.WriteAllText($@"Settings/settings_{arg.Id}.json", JsonConvert.SerializeObject(new Dictionary<string, Dictionary<Settings, object>>
            {
                { "Settings", _bc.inWelcome }
            }, Formatting.Indented));
        }

        public static bool SpecialPeople(ulong ID)
        {
            ulong[] IDs = new ulong[] { 170695871510347778 };
            if (IDs.ToList().Where(x => x.Equals(ID)).Count() != 0) {
                return true;
            }
            else {
                return false;
            }
        }

        public static ulong Mention2role(string arg) => Convert.ToUInt64(Regex.Replace(arg, "[^0-9]", string.Empty));

        public static string RemoveMention(string str) => Regex.Replace(str, @"\<.*\>+", string.Empty);

        public static string RemoveAllMention(string str)
        {
            StringBuilder sb = new StringBuilder(Regex.Replace(str, @"\<.*?\>+", string.Empty));
            return sb.Replace("@here", string.Empty)
                        .Replace("@everyone", string.Empty)
                        .ToString();
        }

        public static string EditMessage(string str, IGuildUser user, IChannel channel, IGuild guild)
        {
            StringBuilder sb = new StringBuilder(str);
            return sb.Replace("{server}", guild.Name)
                        .Replace("{user}", user.Username)
                        .Replace("{user.mention}", user.Mention)
                        .Replace("{user.disc}", user.ToString())
                        .Replace("{user.id}", $"{user.Id}")
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
            Discriminator = 35,
            Role = 36,
            Gamble = 37
        }

        public enum Settings
        {
            Welcome = 0,
            Leave = 1,
            JoinRole = 2,
            CharSpam = 3,
            WordSpam = 4,
            EmojiSpam = 5,
            Prefix = 6,
            DMmsg = 7,
            LogCHN = 8,
            Premium = 9,
            WelcomeCHN = 10
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
