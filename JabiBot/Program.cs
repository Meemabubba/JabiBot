using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace JabiBot
{
    class Program
    {
        private DiscordSocketClient _client;

        static void Main(string[] args)
        {
            new Program().start().GetAwaiter().GetResult();
        }
        public async Task start()
        {
            Global.ReadConfig();
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Debug
            });

            _client.Log += _client_Log;

            await _client.LoginAsync(TokenType.Bot, Global.Token);

            await _client.StartAsync();

            CommandHandler c = new CommandHandler(_client);

            await Task.Delay(-1);
        }

        private async Task _client_Log(LogMessage msg)
        {
            if (!msg.Message.StartsWith("Received Dispatch"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[" + DateTime.Now.TimeOfDay + "] - " + msg.Message);
            }
        }
    }
}
