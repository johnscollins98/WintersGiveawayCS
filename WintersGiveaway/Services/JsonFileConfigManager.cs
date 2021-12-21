using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WintersGiveaway.Interfaces;
using WintersGiveaway.Models;

namespace WintersGiveaway.Services
{
    public class JsonFileConfigManager : IConfigManager
    {
        private readonly IFile jsonFile;
        private Config? cache;

        public JsonFileConfigManager(IFile jsonFile)
        {
            this.jsonFile = jsonFile;
        }

        public Config GetConfg()
        {
            if (cache == null)
            {
                cache = JsonConvert.DeserializeObject<Config>(jsonFile.GetText());
            }

            return cache;
        }
    }
}
