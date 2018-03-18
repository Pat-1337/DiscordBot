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
                 _root
            };
            return JsonConvert.SerializeObject(collectionWrapper, Formatting.Indented);
        }

        public static string GetToken()
        {
            Dictionary<string, string> obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText($@"_bot/token.json"));
            return obj["Token"];
        }

        public static string GetHelp1()
        {
            Dictionary<string, string> obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText($@"_bot/token.json"));
            return obj["Help1"];
        }

        public static string GetHelp2()
        {
            Dictionary<string, string> obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText($@"_bot/token.json"));
            return obj["Help2"];
        }

        public static string GetApiKeyG()
        {
            Dictionary<string, string> obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText($@"_bot/token.json"));
            return obj["api_key_giphy"];
        }

        public static string GetApiKeydb()
        {
            Dictionary<string, string> obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText($@"_bot/token.json"));
            return obj["api_key_danbooru"];
        }

        public static string GetApiNamedb()
        {
            Dictionary<string, string> obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText($@"_bot/token.json"));
            return obj["api_name_danbooru"];
        }

        public bool CheckPermChn(IGuild guild, ulong id, Modules.BaseCommands.Commands cmd)
        {
            Modules.BaseCommands _bc = new Modules.BaseCommands();
            Main.Daddy._client.Guilds.ToList().ForEach(async x => {
                if (!File.Exists($@"json/jayson_{x.Id}.json") && x.Id.Equals(guild.Id)) {
                    IEnumerable<IGuildChannel> channels = (await guild.GetChannelsAsync()).Where(y => y is SocketTextChannel).ToList().OrderBy(y => y.Position);
                    Dictionary<ulong, Dictionary<Modules.BaseCommands.Commands, bool>> perm = new Dictionary<ulong, Dictionary<Modules.BaseCommands.Commands, bool>>();
                    channels.ToList().ForEach(z => perm.Add(z.Id, _bc.inPerm));
                    File.WriteAllText($@"json/jayson_{guild.Id}.json", JsonConvert.SerializeObject(perm, Formatting.Indented));
                }
            });
            if (JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<Modules.BaseCommands.Commands, bool>>>(File.ReadAllText($@"json/jayson_{guild.Id}.json")).TryGetValue(id, out Dictionary<Modules.BaseCommands.Commands, bool> dispatch)) {
                return dispatch[cmd];
            }
            else {
                Dictionary<ulong, Dictionary<Modules.BaseCommands.Commands, bool>> _parm = JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<Modules.BaseCommands.Commands, bool>>>(File.ReadAllText($@"json/jayson_{guild.Id}.json"));
                _parm.TryAdd(id, _bc.inPerm);
                File.WriteAllText($@"json/jayson_{guild.Id}.json", JsonConvert.SerializeObject(_parm, Formatting.Indented));
                if (_parm[id].TryGetValue(cmd, out bool tf)) {
                    return tf;
                }
                else {
                    _parm[id].TryAdd(cmd, _bc.inPerm.Where(x => x.Key.Equals(cmd)).Select(x => x.Value).Single());
                    File.WriteAllText($@"json/jayson_{guild.Id}.json", JsonConvert.SerializeObject(_parm, Formatting.Indented));
                    return _bc.inPerm.Where(x => x.Key.Equals(cmd)).Select(x => x.Value).Single();
                }
            }
        }

        public string Settings(IGuild guild, Modules.BaseCommands.Settings stg)
        {
            Modules.BaseCommands _bc = new Modules.BaseCommands();
            try {
                Main.Daddy._client.Guilds.ToList().ForEach(x => {
                    if (!File.Exists($@"Settings/settings_{x.Id}.json") && x.Id.Equals(guild.Id)) {
                        Dictionary<string, Dictionary<Modules.BaseCommands.Settings, object>> welcome = new Dictionary<string, Dictionary<Modules.BaseCommands.Settings, object>>
                        {
                            { "Settings", _bc.inWelcome }
                        };
                        File.WriteAllText($@"Settings/settings_{guild.Id}.json", JsonConvert.SerializeObject(welcome, Formatting.Indented));
                    }
                });
                Dictionary<string, Dictionary<Modules.BaseCommands.Settings, object>> _json = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<Modules.BaseCommands.Settings, object>>>(File.ReadAllText($@"Settings/settings_{guild.Id}.json"));
                return (string)_json["Settings"][stg];
            }
            catch {
                Dictionary<string, Dictionary<Modules.BaseCommands.Settings, object>> _json0 = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<Modules.BaseCommands.Settings, object>>>(File.ReadAllText($@"Settings/settings_{guild.Id}.json"));
                _json0["Settings"].TryAdd(stg, _bc.inWelcome.Where(x => x.Key.Equals(stg)).Select(x => x.Value).Single());
                File.WriteAllText($@"Settings/settings_{guild.Id}.json", JsonConvert.SerializeObject(_json0, Formatting.Indented));
                return _bc.inWelcome.Where(x => x.Key.Equals(stg)).Select(x => x.Value).Single().ToString();
            }
        }

        public static void DeleteJSON(IGuild guild)
        {
            Ext._vMem _vMem = new Ext._vMem();
            if (File.Exists($@"Settings/settings_{guild.Id}.json")) {
                File.Delete($@"Settings/settings_{guild.Id}.json");
            }
            if (File.Exists($@"json/jayson_{guild.Id}.json")) {
                File.Delete($@"json/jayson_{guild.Id}.json");
            }
            _vMem._run();
        }

        public static void DeleteJSON(IGuild guild, ulong id)
        {
            if (File.Exists($@"json/jayson_{guild.Id}.json")) {
                try {
                    Dictionary<ulong, Dictionary<Modules.BaseCommands.Commands, bool>> perm = JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<Modules.BaseCommands.Commands, bool>>>(File.ReadAllText($@"json/jayson_{guild.Id}.json"));
                    perm.Remove(id);
                    File.WriteAllText($@"json/jayson_{guild.Id}.json", JsonConvert.SerializeObject(perm, Formatting.Indented));
                }
                catch {
                    throw new Exception("Can not get Dictionary from JSON file!");
                }
            }
        }

        public void SetTimeJSON(IGuild guild, IUser user, DateTime time)
        {
            Timer timer = new Timer
            {
                ID = user.Id,
                Release = time,
                Roles = ((user as SocketGuildUser).Roles).Where(x => x.Id != guild.EveryoneRole.Id).Select(y => y.Id).ToArray()
            };
            File.WriteAllText($@"Time/time_{guild.Id}.json", AddTimeBan(timer));
        }

        public bool GetTime(IGuild guild, ulong id)
        {
            Main.Daddy._client.Guilds.ToList().ForEach(x => {
                if (!File.Exists($@"Time/time_{x.Id}.json") && x.Id.Equals(guild.Id)) {
                    File.Create($@"Time/time_{x.Id}.json").Dispose();
                }
            });
            JObject rss = JObject.Parse(File.ReadAllText($@"Time/time_{guild.Id}.json"));
            JArray dataTable = (JArray)rss["_root"];
            for (int y = 0; y < dataTable.Count; y++) {
                if (Convert.ToUInt64((string)dataTable[y]["ID"]).Equals(id)) {
                    if (DateTime.ParseExact((string)dataTable[y]["Release"], "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) <= DateTime.Now) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }
            return false;
        }

        private void RemoveFields(JToken token, string[] fields)
        {
            JContainer container = token as JContainer;
            if (container == null) return;

            List<JToken> removeList = new List<JToken>();
            foreach (JToken el in container.Children()) {
                if (el is JProperty p && fields.Contains(p.Name)) {
                    removeList.Add(el);
                }
                RemoveFields(el, fields);
            }
            foreach (JToken el in removeList) {
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
            foreach (JToken el in container.Children()) {
                if (el is JProperty p && fields.Contains(p.Name)) {
                    removeList.Add(el);
                }
                el.RemoveFields(fields);
            }

            foreach (JToken el in removeList) {
                el.Remove();
            }

            return token;
        }
    }
}
