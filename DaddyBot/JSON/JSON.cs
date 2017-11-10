using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft;
using System.IO;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using Discord;
using System.Linq;
using System.Globalization;

namespace Daddy.Database
{
    public class JSON
    {
        public class Timer
        {
            public ulong ID { get; set; }
            public DateTime Release { get; set; }
            public ulong[] Roles { get; set; }
        }

        public static List<Timer> _root = new List<Timer>();

        public static string AddTimeBan(Timer myObject)
        {
            _root.Add(myObject);
            dynamic collectionWrapper = new
            {
                _root = _root
            };
            return JsonConvert.SerializeObject(collectionWrapper, Formatting.Indented);
        }

        public static string getToken()
        {
            Dictionary<string, string> obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText($@"_bot/token.json"));
            return obj["Token"];
        }

        public static string getHelp()
        {
            Dictionary<string, string> obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText($@"_bot/token.json"));
            return obj["Help"];
        }

        public bool checkPermChn(IGuild guild, ulong id, Modules.BaseCommands.Commands cmd)
        {
            Modules.BaseCommands _bc = new Modules.BaseCommands();
            try
            {
                Main.Daddy._client.Guilds.ToList().ForEach(async x =>
                {
                    if (!File.Exists($@"json/jayson_{x.Id}.json") && x.Id.Equals(guild.Id))
                    {
                        IEnumerable<IGuildChannel> channels = (await guild.GetChannelsAsync()).Where(y => y is SocketTextChannel).ToList().OrderBy(y => y.Position);
                        _bc.perm.Clear();
                        channels.ToList().ForEach(z => _bc.perm.Add(z.Id, _bc.inPerm));
                        File.WriteAllText($@"json/jayson_{guild.Id}.json", JsonConvert.SerializeObject(_bc.perm, Formatting.Indented));
                    }
                });
                Dictionary<ulong, Dictionary<Modules.BaseCommands.Commands, bool>> _parm = JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<Modules.BaseCommands.Commands, bool>>>(File.ReadAllText($@"json/jayson_{guild.Id}.json"));
                return _parm[id][cmd];
            }
            catch
            {
                Dictionary<ulong, Dictionary<Modules.BaseCommands.Commands, bool>> _parm0 = JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<Modules.BaseCommands.Commands, bool>>>(File.ReadAllText($@"json/jayson_{guild.Id}.json"));
                Dictionary<Modules.BaseCommands.Commands, bool> _parm1 = _parm0[id];
                _parm1.Add(cmd, _bc.inPerm.Where(x => x.Key.Equals(cmd)).Select(x => x.Value).Single());
                _parm0[id] = _parm1;
                File.WriteAllText($@"json/jayson_{guild.Id}.json", JsonConvert.SerializeObject(_parm0, Formatting.Indented));
                Dictionary<ulong, Dictionary<Modules.BaseCommands.Commands, bool>> _parm2 = JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<Modules.BaseCommands.Commands, bool>>>(File.ReadAllText($@"json/jayson_{guild.Id}.json"));
                return _parm2[id][cmd];
                //return _bc.inPerm.Where(x => x.Key.Equals(cmd)).Select(x => x.Value).Single();
            }
        }

        public string Settings(IGuild guild, Modules.BaseCommands.Settings stg)
        {
            Modules.BaseCommands _bc = new Modules.BaseCommands();
            try
            {
                Main.Daddy._client.Guilds.ToList().ForEach(x =>
                {
                    if (!File.Exists($@"Settings/settings_{x.Id}.json") && x.Id.Equals(guild.Id))
                    {
                        _bc.welcome.Clear();
                        _bc.welcome.Add("Settings", _bc.inWelcome);
                        File.WriteAllText($@"Settings/settings_{guild.Id}.json", JsonConvert.SerializeObject(_bc.welcome, Formatting.Indented));
                    }
                });
                Dictionary<string, Dictionary<Modules.BaseCommands.Settings, string>> _json = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<Modules.BaseCommands.Settings, string>>>(File.ReadAllText($@"Settings/settings_{guild.Id}.json"));
                return _json["Settings"][stg];
            }
            catch
            {
                Dictionary<string, Dictionary<Modules.BaseCommands.Settings, string>> _json0 = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<Modules.BaseCommands.Settings, string>>>(File.ReadAllText($@"Settings/settings_{guild.Id}.json"));
                Dictionary<Modules.BaseCommands.Settings, string> _json1 = _json0["Settings"];
                _json1.Add(stg, _bc.inWelcome.Where(x => x.Key.Equals(stg)).Select(x => x.Value).Single());
                _json0["Settings"] = _json1;
                File.WriteAllText($@"Settings/settings_{guild.Id}.json", JsonConvert.SerializeObject(_json0, Formatting.Indented));
                Dictionary<string, Dictionary<Modules.BaseCommands.Settings, string>> _json2 = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<Modules.BaseCommands.Settings, string>>>(File.ReadAllText($@"Settings/settings_{guild.Id}.json"));
                return _json2["Settings"][stg];
                //return _bc.inWelcome.Where(x => x.Key.Equals(stg)).Select(x => x.Value).Single()
            }
        }

        public void setTimeJSON(IGuild guild, IUser user, DateTime time)
        {
            /*Main.Daddy._client.Guilds.ToList().ForEach(x =>
            {
                if (!File.Exists($@"Time\time_{x.Id}.json") && x.Id.Equals(guild.Id))
                {
                    File.Create($@"Time\time_{x.Id}.json").Dispose();
                }
            });*/
            Timer timer = new Timer
            {
                ID = user.Id,
                Release = time,
                Roles = ((user as SocketGuildUser).Roles).Where(x => x.Id != guild.EveryoneRole.Id).Select(y => y.Id).ToArray()
            };
            File.WriteAllText($@"Time/time_{guild.Id}.json", AddTimeBan(timer));
        }

        public bool getTime(IGuild guild, ulong id)
        {
            Main.Daddy._client.Guilds.ToList().ForEach(x =>
            {
                if (!File.Exists($@"Time/time_{x.Id}.json") && x.Id.Equals(guild.Id))
                {
                    File.Create($@"Time/time_{x.Id}.json").Dispose();
                }
            });
            JObject rss = JObject.Parse(File.ReadAllText($@"Time/time_{guild.Id}.json"));
            JArray dataTable = (JArray)rss["_root"];
            for (int y = 0; y < dataTable.Count; y++)
            {
                if (Convert.ToUInt64((string)dataTable[y]["ID"]).Equals(id))
                {
                    if (DateTime.ParseExact((string)dataTable[y]["Release"], "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) <= DateTime.Now)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        private void removeFields(JToken token, string[] fields)
        {
            JContainer container = token as JContainer;
            if (container == null) return;

            List<JToken> removeList = new List<JToken>();
            foreach (JToken el in container.Children())
            {
                JProperty p = el as JProperty;
                if (p != null && fields.Contains(p.Name))
                {
                    removeList.Add(el);
                }
                removeFields(el, fields);
            }
            foreach (JToken el in removeList)
            {
                el.Remove();
            }
        }
    }


    public static class Extensions
    {
        public static JToken RemoveFields(this JToken token, string[] fields)
        {
            JContainer container = token as JContainer;
            if (container == null) return token;

            List<JToken> removeList = new List<JToken>();
            foreach (JToken el in container.Children())
            {
                JProperty p = el as JProperty;
                if (p != null && fields.Contains(p.Name))
                {
                    removeList.Add(el);
                }
                el.RemoveFields(fields);
            }

            foreach (JToken el in removeList)
            {
                el.Remove();
            }

            return token;
        }
    }
}
