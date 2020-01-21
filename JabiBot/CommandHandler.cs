using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace JabiBot
{
    class CommandHandler
    {
        public DiscordSocketClient _client;
        private CommandService _service;
        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;

            _service = new CommandService();

            _service.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            //_client.MessageReceived += _client_MessageReceived;

            _client.MessageReceived += HandleCommand;

            _client.MessageReceived += IsSwissDiscord;

            _client.Ready += ai;
        }

        SocketMessage r;
        private async Task ai()
        {
            Thread t = new Thread(aiTrd);
            t.Start();
        }
        private async void aiTrd()
        {
            while (true)
            {
                await Task.Delay(5000);
                if (r != null)
                {
                    try
                    {
                        await _client_MessageReceived(r);
                    }
                    catch(Exception ex) { Console.WriteLine(ex); }
                }
            }
        }

        private async Task IsSwissDiscord(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            if (msg.Channel.Id == 669305903710863440)
            {
                r = arg;
            }
        }

        private async Task _client_MessageReceived(SocketMessage arg)
        {
            string umsg = arg.Content;
            string[] aiWordList = File.ReadAllLines(Global.AiFilePath);
            for (int i = 0; i != aiWordList.Length; i++)
                aiWordList[i] = aiWordList[i].ToLower();
            int[] indx = aiWordList.Select((b, i) => b == umsg ? i : -1).Where(i => i != -1).ToArray();
            if(indx.Length != 0)
            {
                Random r = new Random();
                int inx = r.Next(0, indx.Length - 1);
                string msg = aiWordList[inx + 1];
                if (inx == 0)
                    msg = sant(aiWordList[r.Next(0, aiWordList.Length - 1)]).Result;
                await arg.Channel.SendMessageAsync(sant(msg).Result);
            }
            else
            {
                string[] spl = umsg.Split(' ');
                var query = from state in aiWordList.AsParallel()
                            let StateWords = state.Split(' ')
                            select (Word: state, Count: spl.Intersect(StateWords).Count());
                var sortedDict = from entry in query orderby entry.Count descending select entry;
                string rMsg = sortedDict.First().Word;
                var reslt = aiWordList.Select((b, i) => b == rMsg ? i : -1).Where(i => i != -1).ToArray();
                if (reslt.Length != 0)
                {
                    Random ran = new Random();
                    var ind = (reslt[ran.Next(0, reslt.Length)]);
                    string msg = aiWordList[ind + 1];
                    if (msg == "16 gb ram ")
                    {
                        Random r = new Random();
                        await arg.Channel.SendMessageAsync(sant(aiWordList[r.Next(0, aiWordList.Length - 1)]).Result);
                        return;
                    }
                    await arg.Channel.SendMessageAsync(sant(msg).Result);
                       
                }
                else { await arg.Channel.SendMessageAsync(sant(rMsg).Result); }
            }
        }
        public async Task<string> sant(string inp)
        {
            string msg = inp;
            Regex rg2 = new Regex(".*(\\d{18})>.*");

            if (rg2.IsMatch(msg))
            {
                var rm = rg2.Match(msg);
                var user = _client.GetGuild(592458779006730264).GetUser(Convert.ToUInt64(rm.Groups[1].Value));
                if (user != null)
                {
                    msg = msg.Replace(rm.Groups[0].Value, $"**(non-ping: {user.Username}#{user.Discriminator})**");
                }
                else
                {
                    try
                    {
                        var em = await _client.GetGuild(592458779006730264).GetEmoteAsync(Convert.ToUInt64(rm.Groups[1].Value));
                        if (em == null)
                        {
                            msg = msg.Replace(rm.Groups[0].Value, $"**(non-ping: {rm.Value})**");
                        }

                    }
                    catch (Exception ex) 
                    {
                        if (msg.Contains("@everyone")) { msg = msg.Replace("@everyone", "***(Non-ping @every0ne)***"); }
                        if (msg.Contains("@here")) { msg = msg.Replace("@here", "***(Non-ping @h3re)***"); }
                      
                    }
                }
                //return msg;
            }
            if (msg.Contains("@everyone")) { msg = msg.Replace("@everyone", "***(Non-ping @every0ne)***"); }
            if (msg.Contains("@here")) { msg = msg.Replace("@here", "***(Non-ping @h3re)***"); }
            return msg;
        }
        private async Task HandleCommand(SocketMessage s)
        {
            SocketUserMessage msg = s as SocketUserMessage;
            if(msg == null) { return; }
            var context = new SocketCommandContext(_client, msg);
            int argPos = 0;
            if (msg.HasCharPrefix(Global.Prefix, ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos, null, MultiMatchHandling.Best);

                if(result.IsSuccess == false)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }
    }
}
