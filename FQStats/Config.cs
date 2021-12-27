using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FQStats
{
    public class Config : IRocketPluginConfiguration
    {
        public string DatabaseAddress;
        public int DatabasePort;
        public string DatabaseName;
        public string DatabaseTableName;
        public string DatabaseUsername;
        public string DatabasePassword;
        public void LoadDefaults()
        {
            DatabaseAddress = "localhost";
            DatabaseUsername = "unturned";
            DatabasePassword = "password";
            DatabaseName = "unturned";
            DatabaseTableName = "ranks";
            DatabasePort = 3306;
        }
    }
}
