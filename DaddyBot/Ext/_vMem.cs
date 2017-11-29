using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using System.Threading.Tasks;
using Daddy.Database;
using Newtonsoft.Json;
using System.Linq;
using System.IO;
using System.Threading;

namespace Daddy.Ext
{
    public class _vMem
    {
        public static Dictionary<ulong, Dictionary<object, object>> _vMemory = new Dictionary<ulong, Dictionary<object, object>>();

        public void _run()
        {
            new Thread(new ThreadStart(_run_in)).Start();
        }

        public void _run_in()
        {
            _vMemory.Clear();
            Main.Daddy._client.Guilds.ToList().ForEach(x => {
                Dictionary<object, object> _cache = new Dictionary<object, object>();
                Dictionary<string, Dictionary<Modules.BaseCommands.Settings, string>> _json = new Dictionary<string, Dictionary<Modules.BaseCommands.Settings, string>>();
                try {
                    _json = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<Modules.BaseCommands.Settings, string>>>(File.ReadAllText($@"Settings/settings_{x.Id}.json"));
                }
                catch {
                    if (!Modules.BaseCommands.doesExistSettings(Main.Daddy._client.GetGuild(x.Id) as IGuild)) {
                        Modules.BaseCommands.createSettings(Main.Daddy._client.GetGuild(x.Id) as IGuild);
                    }
                    _json = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<Modules.BaseCommands.Settings, string>>>(File.ReadAllText($@"Settings/settings_{x.Id}.json"));
                }
                foreach (Modules.BaseCommands.Settings setting in Enum.GetValues(typeof(Modules.BaseCommands.Settings))) {
                    try {
                        _cache.Add(setting, _json["Settings"][setting]);
                    }
                    catch {
                        //lol idk fuck off
                    }
                }
                _vMemory.Add(x.Id, _cache);
            });
        }
    }
}
