using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using Daddy.Modules;
using Daddy.Database;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using static Daddy.Modules.BaseCommands;

namespace Daddy.Modules
{
    public class Currency : ModuleBase
    {
        public static Random _ran = new Random();
        JSON jSon = new JSON();
        Ext._vMem _vMem = new Ext._vMem();
        public static Emote shekel = null;

        [Command("shekels", RunMode = RunMode.Async), Summary("Checks how much shekels you have")]
        public async Task _shekels(SocketGuildUser user = null)
        {
            await ReplyAsync(string.Empty, false, embed: _builder($"**{(user ?? Context.User).Username}**", $"User ー {Amount((user ?? Context.User).Id)}{shekel}\nBank ー {Amount_bank((user ?? Context.User).Id)}{shekel}")
                .WithFooter(y => y.WithText(Context.User.ToString())).Build());
        }

        [Command("vote", RunMode = RunMode.Async), Alias("daily"), Summary("Sends link to vote"), Ext.Ratelimit(1, 5, Ext.Measure.Minutes)]
        public async Task _vote(SocketGuildUser user = null) {
            await ReplyAsync("https://discordbots.org/bot/215626696202780672/vote");
        }

        [Command("give", RunMode = RunMode.Async), Priority(1), Summary("Gives shekels")]
        public async Task _give_shekels_(SocketGuildUser user, int arg) => await _give_shekels(arg, user);

        [Command("give", RunMode = RunMode.Async), Priority(0), Summary("Gives shekels"), Ext.Ratelimit(1, 0.5, Ext.Measure.Minutes)]
        public async Task _give_shekels(int arg, SocketGuildUser user)
        {
            if (!(user.Id).Equals(Context.User.Id) && !user.IsBot) {
                Give(Context.User as SocketGuildUser, user, arg, out int[] vals);
                if (!vals.Any()) {
                    await ReplyAsync("`Error!`");
                    return;
                }
                await ReplyAsync(string.Empty, false, embed: _builder(string.Empty, $"{Context.User.Username} ー {vals[0]}{shekel}\n{user.Username} ー {vals[1]}{shekel}")
                    .WithFooter(y => y.WithText(Context.User.ToString())).Build());
            }
        }

        [Command("deposit", RunMode = RunMode.Async), Summary("Deposit"), Ext.Ratelimit(1, 5, Ext.Measure.Minutes)]
        public async Task _deposit_shekels(uint arg)
        {
            Deposit(Context.User as SocketGuildUser, arg, out int[] vals);
            if (!vals.Any()) {
                await ReplyAsync($"`Error! You can deposit up to {5 * Get_Storage(Context.User.Id)}k` {shekel}");
                return;
            }
            await ReplyAsync(string.Empty, false, embed: _builder($"{Context.User.Username} deposited {vals[0]} to the bank!", $"{Context.User.Username} ー {vals[1]}{shekel}\nBank ー {vals[2]}{shekel}")
                .WithFooter(y => y.WithText(Context.User.ToString())).Build());
        }

        [Command("withdraw", RunMode = RunMode.Async), Summary("Deposit"), Ext.Ratelimit(1, 5, Ext.Measure.Minutes)]
        public async Task _withdraw_shekels(uint arg)
        {
            Withdraw(Context.User as SocketGuildUser, arg, out int[] vals);
            if (!vals.Any()) {
                await ReplyAsync($"`Error!`");
                return;
            }
            await ReplyAsync(string.Empty, false, embed: _builder($"{Context.User.Username} withdrew {vals[0]} from the bank!", $"{Context.User.Username} ー {vals[1]}{shekel}\nBank ー {vals[2]}{shekel}")
                .WithFooter(y => y.WithText(Context.User.ToString())).Build());
        }

        [Command("steal", RunMode = RunMode.Async), Summary("Gives shekels"), Ext.Ratelimit(1, 0.25, Ext.Measure.Minutes)]
        public async Task _steal_shekels(SocketGuildUser user)
        {
            if (!(user.Id).Equals(Context.User.Id) && !user.IsBot)
            {
                if (Amount(Context.User.Id) >= 10) {
                    Steal(Context.User as SocketGuildUser, user, out object[] vals);
                    await ReplyAsync(string.Empty, false, embed: _builder($@"**{((bool)vals[0] ? $"{Context.User.Username} stole {vals[1]}{shekel} from {user.Username}"
                        : $"{Context.User.Username} lost {vals[1]}{shekel} to {user.Username}")}**",
                        $"\n{Context.User.Username} ー {vals[2]}{shekel}\n{user.Username} ー {vals[3]}{shekel}")
                        .WithFooter(y => y.WithText(Context.User.ToString())).Build());
                }
                else {
                    await ReplyAsync($"`You need at least 10`{shekel}");
                }
            }
        }

        [Command("sets", RunMode = RunMode.Async), Summary("Sets how much shekels you have"), RequireOwner()]
        public async Task _set_shekels(int arg, SocketGuildUser user = null)
        {
            jSon.Set_User_Data((user ?? Context.User).Id, User.Shekels, arg);
            await ReplyAsync(string.Empty, false, embed: _builder($"**{(user ?? Context.User).Username} ー {arg}{shekel}**")
                .WithFooter(y => y.WithText(Context.User.ToString())).Build());
        }

        [Command("setb", RunMode = RunMode.Async), Summary("Sets how much shekels you have in bank"), RequireOwner()]
        public async Task _set_bank(int arg, SocketGuildUser user = null)
        {
            jSon.Set_User_Data((user ?? Context.User).Id, User.Bank, arg);
            await ReplyAsync(string.Empty, false, embed: _builder($"**{(user ?? Context.User).Username} ー {arg}{shekel}**")
                .WithFooter(y => y.WithText(Context.User.ToString())).Build());
        }

        [Command("adds", RunMode = RunMode.Async), Summary("Adds sgekels"), RequireOwner()]
        public async Task _add_shekels(int arg, SocketGuildUser user = null)
        {
            Add((user ?? Context.User).Id, arg, out int val);
            await ReplyAsync(string.Empty, false, embed: _builder($"**{(user ?? Context.User).Username} ー {val}{shekel}**")
                .WithFooter(y => y.WithText(Context.User.ToString())).Build());
        }

        [Command("subs", RunMode = RunMode.Async), Summary("Subtracts shekels"), RequireOwner()]
        public async Task _sub_shekels(int arg, SocketGuildUser user = null)
        {
            Subtract((user ?? Context.User).Id, arg, out int val);
            await ReplyAsync(string.Empty, false, embed: _builder($"**{(user ?? Context.User).Username} ー {val}{shekel}**")
                .WithFooter(y => y.WithText(Context.User.ToString())).Build());
        }

        [Command("store", RunMode = RunMode.Async), Summary("Opens the store")]
        public async Task _store_open()
        {
            IUserMessage msg = await ReplyAsync(string.Empty, false, embed: _store().Build());
            for (int x = 1; x < Enum.GetValues(typeof(Store)).Length + 1; x++) {
                await msg.AddReactionAsync(new Emoji($"{x}\u20e3"));
            }
        }

        public void Add_storage(ulong id)
        {
            jSon.Set_User_Data(id, User.Storage, Get_Storage(id) + 1);
        }

        public void Add(ulong id, int arg, out int val)
        {
            int amount = Amount(id);
            jSon.Set_User_Data(id, User.Shekels, amount + arg);
            val = amount + arg;
        }

        public void Add_bank(ulong id, int arg, out int val)
        {
            int amount = Amount_bank(id);
            jSon.Set_User_Data(id, User.Bank, amount + arg);
            val = amount + arg;
        }

        public void Subtract(ulong id, int arg, out int val)
        {
            int amount = Amount(id);
            if (arg >= amount) {
                jSon.Set_User_Data(id, User.Shekels, 0);
                val = 0;
                return;
            }
            jSon.Set_User_Data(id, User.Shekels, amount - arg);
            val = amount - arg;
        }

        public void Subtract_bank(ulong id, int arg, out int val)
        {
            int amount = Amount_bank(id);
            if (arg >= amount)
            {
                jSon.Set_User_Data(id, User.Bank, 0);
                val = 0;
                return;
            }
            jSon.Set_User_Data(id, User.Bank, amount - arg);
            val = amount - arg;
        }

        /// <summary> Calls DB to check storage </summary>
        public int Get_Storage(ulong id) => Convert.ToInt32(jSon.User_data(id, User.Storage));

        /// <summary> Calls DB to check amount of shekels </summary>
        public int Amount(ulong id) => Convert.ToInt32(jSon.User_data(id, User.Shekels));

        /// <summary> Calls DB to check amount of shekels in bank </summary>
        public int Amount_bank(ulong id) => Convert.ToInt32(jSon.User_data(id, User.Bank));

        private void Give(SocketGuildUser user1, SocketGuildUser user2, int amount, out int[] vals)
        {
            if (amount > 0)
            {
                int client = Amount(user1.Id);
                if (amount > client){
                    amount = client;
                }
                Subtract(user1.Id, amount, out int val1);
                Add(user2.Id, amount, out int val2);
                vals = new int[] { val1, val2 };
            }
            else {
                vals = new int[0];
            }
        }

        private void Deposit(SocketGuildUser user, uint amount, out int[] vals)
        {
            int bank = Amount_bank(user.Id);
            int storage = Get_Storage(user.Id);
            if (amount > 0 && bank < 5000 * storage)
            {
                uint client = (uint)Amount(user.Id);
                if (amount > client) {
                    amount = client;
                }
                if ((bank + amount) <= 5000 * storage)
                {
                    Subtract(user.Id, (int)amount, out int val1);
                    Add_bank(user.Id, (int)amount, out int val2);
                    vals = new int[] { (int)amount, val1, val2 };
                }
                else {
                    Subtract(user.Id, 5000 * storage - bank, out int val1);
                    Add_bank(user.Id, 5000 * storage - bank, out int val2);
                    vals = new int[] { (int)amount, val1, val2 };
                }
            }
            else {
                vals = new int[0];
            }
        }

        private void Withdraw(SocketGuildUser user, uint amount, out int[] vals)
        {
            int bank = Amount_bank(user.Id);
            if (amount > 0 && bank > 0)
            {
                if (amount > bank) {
                    amount = (uint)bank;
                }
                Add(user.Id, (int)amount, out int val2);
                Subtract_bank(user.Id, (int)amount, out int val1);
                vals = new int[] { (int)amount, val2, val1 };
            }
            else {
                vals = new int[0];
            }
        }

        private void Steal(SocketGuildUser user1, SocketGuildUser user2, out object[] vals)
        {
            int random = _ran.Next(101);
            int amount1 = Amount(user1.Id);
            int amount2 = Amount(user2.Id);
            if (random <= 15)
            {
                int stolen = _ran.Next(amount2 / 4, amount2 / 3);
                Subtract(user2.Id, stolen, out int val1);
                Add(user1.Id, stolen, out int val2);
                vals = new object[] { true, stolen, val2, val1};
            }
            else
            {
                int lost = _ran.Next(amount1 / 4, amount1 / 3);
                Subtract(user1.Id, lost, out int val1);
                Add(user2.Id, lost, out int val2);
                vals = new object[] { false, lost, val1, val2 };
            }
        }

        private EmbedBuilder _builder(string text, string desc = "")
        {
            return new EmbedBuilder()
            {
                Title = text,
                Description = desc,
                Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
            }.WithCurrentTimestamp();
        }

        private EmbedBuilder _store()
        {
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            foreach (Store store in Enum.GetValues(typeof(Store))) {
                sb1.AppendLine($"[{Array.IndexOf(Enum.GetValues(typeof(Store)), store) + 1}] {store.ToString()}");
                sb2.AppendLine($"{(int)store}");
            }
            EmbedFieldBuilder field1 = new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = "Item",
                Value = sb1.ToString()
            };
            EmbedFieldBuilder field2 = new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = $"Price [{shekel}]",
                Value = sb2.ToString()
            };
            return new EmbedBuilder()
            {
                Title = "Shekel store",
                Fields = new List<EmbedFieldBuilder>() { field1, field2 },
                Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
            }.WithCurrentTimestamp();
        }

        public static void SetEmoji()
        {
            if (Emote.TryParse("<:shekel:436609077766914048>", out Emote emote))
            {
                shekel = emote;
            }
        }
    }
}
