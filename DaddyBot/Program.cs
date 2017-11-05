using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Daddy.Database;
using System.Collections.Generic;

namespace Daddy.Main
{
    public class Daddy
    {
        static void Main(string[] args) => new Daddy().Start(args).GetAwaiter().GetResult();

        public static CommandService _commands;
        public static DiscordSocketClient _client;
        private CancellationTokenSource _cts;
        public static Random _ran = new Random();
        public static Daddy _instance = new Daddy();
        JSON jSon = new JSON();
        public static readonly string token = JSON.getToken();
        public Ext.Ext ObjectMemory { get; private set; }

        DateTime startTime = DateTime.Now;

        public async Task Start(string[] args)
        {
            await DoBotStuff();

            await Task.Delay(-1);
        }

        public void Stop()
        {
            _cts.Cancel();
        }

        public async Task DoBotStuff()
        {
            _instance = this;
            _cts = new CancellationTokenSource();
            _commands = new CommandService();
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 50
                //TotalShards = 2
            });

            _client.Log += Log;

            await InstallCommands();
            ConfigureEventHandlers();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            Task timerTask = RunPeriodically(_game, TimeSpan.FromSeconds(60), tokenSource.Token);

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }

        private async Task InstallCommands()
        {
            _client.MessageReceived += HandleCommand;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommand(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null || arg.Author.IsBot)
            {
                return;
            }
            int argPos = 0;
            if (message.Content.Contains("https://discord.gg"))
            {
                await message.DeleteAsync();
                await message.Channel.SendMessageAsync($"{message.Author.Mention} Please don't send invite links!");
            }
            else if (message.Content.ToLower().Contains("valkyrie"))
            {
                await message.DeleteAsync();
                await message.Channel.SendMessageAsync($"{message.Author.Mention} Die nigger");
            }
            else if (message.Content.ToLower().Contains("kosovo je srbija"))
            {
                await message.DeleteAsync();
                await message.Channel.SendMessageAsync($"{message.Author.Mention} Screw Albania");
            }
            /*else if (HasConsecutiveChars(message.Content, Convert.ToInt32(jSon.Settings((message.Channel as SocketGuildChannel).Guild, Modules.BaseCommands.Settings.CharSpam))) && !Modules.BaseCommands.isVip(message.Author))
            {
                await message.DeleteAsync();
                await message.Channel.SendMessageAsync($"{message.Author.Mention} Don't type nonsense!");
            }
            else if (HasConsecutiveWords(message.Content, Convert.ToInt32(jSon.Settings((message.Channel as SocketGuildChannel).Guild, Modules.BaseCommands.Settings.WordSpam))) && !Modules.BaseCommands.isVip(message.Author))
            {
                await message.DeleteAsync();
                await message.Channel.SendMessageAsync($"{message.Author.Mention} Don't type nonsense!");
            }
            else if (HasConsecutiveEmojis(message.Content, Convert.ToInt32(jSon.Settings((message.Channel as SocketGuildChannel).Guild, Modules.BaseCommands.Settings.EmojiSpam))) && !Modules.BaseCommands.isVip(message.Author))
            {
                await message.DeleteAsync();
                await message.Channel.SendMessageAsync($"{message.Author.Mention} Don't type nonsense!");
            }*/
            else if (!(message.HasCharPrefix(Convert.ToChar(jSon.Settings((message.Channel as SocketGuildChannel).Guild, Modules.BaseCommands.Settings.Prefix)), ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
            {
                return;
            }
            var context = new CommandContext(_client, message);
            var result = await _commands.ExecuteAsync(context, argPos);
            if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

        public void ConfigureEventHandlers()
        {
            _client.JoinedGuild += Client_JoinedGuild;
            _client.LeftGuild += Client_LeftGuild;
            _client.UserJoined += Client_UserJoined;
            _client.UserLeft += Client_UserLeft;
        }

        private async Task Client_UserJoined(SocketGuildUser arg)
        {
            await sendJoinMSG(arg, null, JoinSeverity.UserJoined);
        }

        private async Task Client_UserLeft(SocketGuildUser arg)
        {
            await sendJoinMSG(arg, null, JoinSeverity.UserLeft);
        }

        private async Task Client_JoinedGuild(SocketGuild arg)
        {
            await sendJoinMSG(null, arg, JoinSeverity.BotJoined);
        }

        private async Task Client_LeftGuild(SocketGuild arg)
        {
            await sendJoinMSG(null, arg, JoinSeverity.BotLeft);
        }

        public async Task sendJoinMSG(SocketGuildUser user = null, SocketGuild guild = null, JoinSeverity j = JoinSeverity.Null)
        {
            EmbedBuilder builder = new EmbedBuilder();
            EmbedAuthorBuilder author = new EmbedAuthorBuilder();
            Database.JSON jSon = new Database.JSON();
            switch (j)
            {
                case JoinSeverity.UserJoined:
                    if (jSon.checkPermChn(guild as IGuild, guild.DefaultChannel.Id, Modules.BaseCommands.Commands.Welcome))
                    {
                        author.Name = $"{user.Username} joined the {user.Guild.Name} family!";
                        builder.Description = $"Thanks for Joining!\nEveryone welcome {user.Username}!\n\n{jSon.Settings(user.Guild, Modules.BaseCommands.Settings.Welcome)}";
                        //builder.WithImageUrl("http://i.imgur.com/hwaYXat.png");
                        if (!jSon.Settings(user.Guild, Modules.BaseCommands.Settings.JoinRole).Equals("N/A"))
                        {
                            await user.AddRoleAsync(user.Guild.GetRole(Modules.BaseCommands.mention2role(jSon.Settings(user.Guild, Modules.BaseCommands.Settings.JoinRole))));
                        }
                    }
                    else
                    {
                        return;
                    }
                    break;
                case JoinSeverity.UserLeft:
                    if (!Modules.BaseCommands.noSend.Contains(user.Id))
                    {
                        if (jSon.checkPermChn(guild as IGuild, guild.DefaultChannel.Id, Modules.BaseCommands.Commands.Leave))
                        {
                            author.Name = $"{user.Username} left {user.Guild.Name} family!";
                            builder.Description = $"{user.Username} has left {user.Guild.Name}!\n{jSon.Settings(user.Guild, Modules.BaseCommands.Settings.Leave)}";
                        }
                        else
                        {
                            return;
                        }
                        break;
                    }
                    else
                    {
                        return;
                    }
                case JoinSeverity.BotJoined:
                    await Task.Delay(2250);
                    await sendWelcomeMsg(guild);
                    return;
                case JoinSeverity.BotLeft:
                    builder.Description = $"";
                    break;
                case JoinSeverity.Null:
                    builder.Description = $"**NULL**";
                    break;
                default:
                    builder.Description = $"**?NULL**";
                    break;
            }
            author.IconUrl = user.GetAvatarUrl();
            builder.WithThumbnailUrl(user.GetAvatarUrl());
            builder.WithAuthor(author);
            builder.Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
            await user.Guild.DefaultChannel.SendMessageAsync(string.Empty, false, embed: builder);
        }

        public static async Task log(string message, LogSeverity severity = LogSeverity.Info, [System.Runtime.CompilerServices.CallerMemberName] string source = "", Exception exception = null)
        {
            await Console.Error.WriteLineAsync($"[{severity}][{source}] {message}");
            if (exception != null)
            {
                await Console.Error.WriteLineAsync(exception.ToString());
                await Console.Error.WriteLineAsync();
            }
        }

        public static bool HasConsecutiveChars(string source, int sequenceLength)
        {
            return Regex.IsMatch(source, "([a-zA-Z])\\1{" + (sequenceLength - 1) + "}");//WORKS
        }

        public static bool HasConsecutiveWords(string source, int sequenceLength)
        {
            return Regex.IsMatch(source, @"(\b(\w+)\s+){" + (sequenceLength - 1) + @"}\1\b");//NEEDS FIX
        }

        public static bool HasConsecutiveEmojis(string source, int sequenceLength)
        {
            return Regex.IsMatch(source, @"((\:.*?\:+)\s+){" + (sequenceLength - 1) + @"\1");//DOESN'T WORK
        }

        async Task RunPeriodically(Action action, TimeSpan interval, CancellationToken token)
        {
            while (true)
            {
                action();
                await Task.Delay(interval, token);
            }
        }

        public async void _game()//games[_ran.Next(games.Length)]
        {
            int guilds = _client.Guilds.Count;
            await Task.Delay(1750);
            string[] games = new string[] {
                $"Uptime: {(int)(DateTime.Now - Process.GetCurrentProcess().StartTime).TotalMinutes} minutes",
                $"Shard: {_client.ShardId} / 1",
                $"in {guilds} guilds #{_client.ShardId}",
                $"Latency: {_client.Latency}ms",
                $"{GC.GetTotalMemory(true)} bytes used" };
            await _client.SetGameAsync(games[_ran.Next(games.Length)]);
        }

        public async Task sendWelcomeMsg(SocketGuild arg)
        {
            EmbedBuilder builder = new EmbedBuilder();
            EmbedAuthorBuilder author = new EmbedAuthorBuilder();
            author.Name = $"{_client.CurrentUser.Username} joined {arg.Name} family!";
            builder.Description = $"Bot made by: [Pat](https://steamcommunity.com/id/0xPat/)\nBot made for: [M&D](https://discord.gg/dFKerFY)\nBot created: {_client.CurrentUser.CreatedAt}\nType .help for command info";
            //builder.WithImageUrl("http://i.imgur.com/hwaYXat.png");
            author.IconUrl = _client.CurrentUser.GetAvatarUrl();
            builder.WithThumbnailUrl(_client.CurrentUser.GetAvatarUrl());
            builder.WithAuthor(author);
            builder.Color = new Discord.Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)));
            await arg.DefaultChannel.SendMessageAsync(string.Empty, false, embed: builder);
            Modules.BaseCommands _bc = new Modules.BaseCommands();
            if (!Modules.BaseCommands.doesExistJson(arg as IGuild))
            {
                Modules.BaseCommands.createJson(arg as IGuild);
            }
        }

        public enum JoinSeverity
        {
            Null = -1,
            UserJoined = 0, 
            UserLeft = 1,
            BotJoined = 2,
            BotLeft = 3
        }

        private static Task Log(LogMessage arg)
        {
            var cc = Console.ForegroundColor;
            switch (arg.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{arg.Severity,8}] {arg.Source}: {arg.Message}");
            Console.ForegroundColor = cc;
            return Task.CompletedTask;
        }
    }
}
