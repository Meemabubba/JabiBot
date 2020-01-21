using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Discord;
using System.Timers;

namespace JabiBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        List<Timer> timerList = new List<Timer>();

        [Command("hello")]
        public async Task hello()
        {
            await Context.Channel.SendMessageAsync("Hi there, " + Context.Message.Author.Mention);
        }
        [Command("fact")]
        public async Task fact()
        {
            //https://uselessfacts.jsph.pl/random
            HttpClient c = new HttpClient();
            var req = await c.GetAsync("https://uselessfacts.jsph.pl/random.txt");
            string messag = await req.Content.ReadAsStringAsync();
            await Context.Channel.SendMessageAsync("Here is a random fact, \n ```" + messag + "```");
        }
        [Command("insult")]
        public async Task insult(params string[] args)
        {
            if (args.Length == 0)
            {
                string[] insults = File.ReadAllLines(Global.InsultFilePath);
                Random r = new Random();
                string ins = insults[r.Next(0, insults.Length - 1)];
                await Context.Channel.SendMessageAsync("Hey " + Context.Message.Author.Mention + "! " + ins);
            }
            else
            {
                if (args[0] == "add")
                {
                    string tot = string.Join(" ", args);
                    tot = tot.Replace("add ", "");
                    string current = File.ReadAllText(Global.InsultFilePath);
                    string final = current + "\n" + tot;
                    File.WriteAllText(Global.InsultFilePath, final);
                    await Context.Channel.SendMessageAsync("Saved the insult: " + tot);
                }
                if (args[0] == "remove")
                {
                    string tot = string.Join(" ", args);
                    tot = tot.Replace("remove ", "");
                    string[] insults = File.ReadAllLines(Global.InsultFilePath);
                    if (insults.Contains(tot))
                    {
                        List<string> nList = insults.ToList();
                        nList.Remove(tot);
                        File.WriteAllLines(Global.InsultFilePath, nList.ToArray());
                        await Context.Channel.SendMessageAsync("Removed the insult: " + tot);
                    }
                    await Context.Channel.SendMessageAsync("Hey that insult doesnt exist!");

                }
                if (args[0] == "list")
                {
                    string ins = File.ReadAllText(Global.InsultFilePath);
                    EmbedBuilder b = new EmbedBuilder()
                    {
                        Color = Color.Purple,
                        Title = "Insult List oWo",
                        Description = "Heres the insults\n```" + ins + "```"
                    };
                    await Context.Channel.SendMessageAsync("", false, b.Build());
                }
            }
        }
        [Command("compliment")]
        public async Task compliment(params string[] args)
        {
            if (args.Length == 0)
            {
                string[] compliments = File.ReadAllLines(Global.ComplimentFilePath);
                Random r = new Random();
                string comp = compliments[r.Next(0, compliments.Length - 1)];
                await Context.Channel.SendMessageAsync(comp + " " + Context.Message.Author.Mention + "!");
            }
            else
            {
                if (args[0] == "add")
                {
                    string tot = string.Join(" ", args);
                    tot = tot.Replace("add ", "");
                    string current = File.ReadAllText(Global.ComplimentFilePath);
                    string final = current + "\n" + tot;
                    File.WriteAllText(Global.ComplimentFilePath, final);
                    await Context.Channel.SendMessageAsync("Saved the compliment: " + tot);
                }
                if (args[0] == "remove")
                {
                    string tot = string.Join(" ", args);
                    tot = tot.Replace("remove ", "");
                    string[] compliments = File.ReadAllLines(Global.ComplimentFilePath);
                    if (compliments.Contains(tot))
                    {
                        List<string> nList = compliments.ToList();
                        nList.Remove(tot);
                        File.WriteAllLines(Global.ComplimentFilePath, nList.ToArray());
                        await Context.Channel.SendMessageAsync("Removed the compliment: " + tot);
                    }
                    await Context.Channel.SendMessageAsync("Hey that compliment doesnt exist!");
                }
                if (args[0] == "list")
                {
                    string ins = File.ReadAllText(Global.ComplimentFilePath);
                    EmbedBuilder b = new EmbedBuilder()
                    {
                        Color = Color.Purple,
                        Title = "Compliment List uwu",
                        Description = "Heres the compliments\n```" + ins + "```"
                    };
                    await Context.Channel.SendMessageAsync("", false, b.Build());
                }
            }
        }
        [Command("pickupline")]
        public async Task pickupline(params string[] args)
        {
            if (args.Length == 0)
            {
                string[] pickuplines = File.ReadAllLines(Global.ComplimentFilePath);
                Random r = new Random();
                string pick = pickuplines[r.Next(0, pickuplines.Length - 1)];
                //string pick = null;
                await Context.Channel.SendMessageAsync("Hey" + Context.Message.Author.Mention + pick);
            }
        }
        bool active = true;
        [Command("repeat")]
        public async Task repeat(params string[] args)
        {
            string phrase = string.Join(" ", args);
            active = true;
            Timer t = new Timer()
            {
                Interval = 2500,
                AutoReset = true,
            };
            t.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                if (active)
                    Context.Channel.SendMessageAsync(phrase, false);
                else
                    t.Stop(); t.Dispose();
            };
            t.Enabled = true;
            t.Start();
            timerList.Add(t);
        }
        [Command("stop")]
        public async Task stop()
        {
            active = false;
            await Context.Channel.SendMessageAsync("Stopped!");
        }
    }
}
