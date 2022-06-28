using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace RoleManager
{
    public class Bot
    {
        private string Path = @"C:\Users\Christian\Documents\DiscordBot\RoleManagerTest\RoleManagerTest" + "/config.json";

        public DiscordClient Client
        {
            get;
            private set;
        }

        public CommandsNextExtension Commands
        {
            get;
            private set;
        }

        private async Task<DiscordClient> ConfigureBot(string configPath)
        {
            var json = string.Empty;
            using (var fs = File.OpenRead(configPath))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
            {
                json = await sr.ReadToEndAsync().ConfigureAwait(false);
            }

            var configJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            if (configJson == null)
                throw new Exception("config.json not deserializeable to Dictionary<string, string>.");

            var config = new DiscordConfiguration
            {
                Token = configJson.First(x => x.Key == "token").Value,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            var client = new DiscordClient(config);

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { configJson.First(x => x.Key == "prefix").Value },
                EnableDms = false,
                EnableMentionPrefix = true
            };

            this.Commands = client.UseCommandsNext(commandsConfig);
            return client;
        }

        public async Task RunAsync()
        {
            this.Client = await ConfigureBot(this.Path);

            Client.Ready += OnClientReady;

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private Task OnClientReady(object sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
