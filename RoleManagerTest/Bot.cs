using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Newtonsoft.Json;
using RoleManagerTest.Commands;
using System.Text;

namespace RoleManager
{
    public class Bot
    {
        private string Path = @"C:\Users\Christian\Documents\DiscordBot\RoleManager\Discord_RoleManager\RoleManagerTest" + "/config.json";

        private Dictionary<string, string> configJson;
        internal Dictionary<string, string> ConfigJson
        {
            get
            {
                return this.configJson;
            }
            private set
            {
                this.configJson = value ?? throw new ArgumentNullException("config.json not deserializeable to Dictionary<string, string>.");
            }
        }

        public DiscordClient Client
        {
            get;
            private set;
        }

        public InteractivityExtension Interactivity 
        { 
            get; 
            private set; 
        }

        public CommandsNextExtension Commands
        {
            get;
            private set;
        }

        private async Task<DiscordClient> ConfigureAndConnectBot(string configPath)
        {
            var json = string.Empty;
            using (var fs = File.OpenRead(configPath))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
            {
                json = await sr.ReadToEndAsync().ConfigureAwait(false);
            }

            this.ConfigJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            var config = new DiscordConfiguration
            {
                Token = this.ConfigJson?.First(x => x.Key == "token").Value,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                Intents = DiscordIntents.All
            };

            var client = new DiscordClient(config);

            return client;
        }

        private async Task ConfigureCommands(DiscordClient client)
        {
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { this.ConfigJson.First(x => x.Key == "prefix").Value },
                EnableDms = false,
                EnableMentionPrefix = true,
                DmHelp = true
            };

            this.Commands = client.UseCommandsNext(commandsConfig);

            this.Commands.RegisterCommands<AssignClassCommand>();
            this.Commands.RegisterCommands<AssignRoleCommand>();
            this.Commands.RegisterCommands<PurgeCommand>();
            this.Commands.RegisterCommands<ListCommandsCommand>();
            this.Commands.RegisterCommands<AssignCharacterCommand>();
        }

        private async Task ConfigureInteractivity(DiscordClient client)
        {
            client.UseInteractivity(new InteractivityConfiguration
            {
                 ResponseBehavior = DSharpPlus.Interactivity.Enums.InteractionResponseBehavior.Ignore
            });
        }

        public async Task RunAsync()
        {
            var client = await ConfigureAndConnectBot(this.Path);
            await ConfigureInteractivity(client);
            await ConfigureCommands(client);
            
            client.Ready += OnClientReady;

            await client.ConnectAsync();

            await Task.Delay(-1);
        }

        private Task OnClientReady(object sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
