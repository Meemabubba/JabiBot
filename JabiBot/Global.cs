using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace JabiBot
{
    class Global
    {
        public static string Token { get; set; }
        public static char Prefix { get; set; }

        public static string ConfigFilePath = Environment.CurrentDirectory + "\\Data\\Config.json";

        public static string InsultFilePath = Environment.CurrentDirectory + "\\Data\\insults.txt";

        internal static string ComplimentFilePath = Environment.CurrentDirectory + "\\Data\\compliments.txt";

        public static ulong BotAiChan { get; set; }

        public static string AiFilePath = Environment.CurrentDirectory + "\\Data\\Responses.AI";

        public static void ReadConfig()
        {
            if (File.Exists(ComplimentFilePath) == false)
                File.Create(ComplimentFilePath).Close();
            Dictionary<string, string> conf = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(ConfigFilePath));
            Token = conf["Token"];
            Prefix = conf["Prefix"].ToCharArray()[0];
            BotAiChan = Convert.ToUInt64(conf["BotAiChan"]);
        }
    }
}
