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

        public Bot(IServiceProvider services)
        {
            var client = ConfigureAndConnectBot(this.Path);
            ConfigureInteractivity(client);
            ConfigureCommands(client, services);

            client.Ready += OnClientReady;

            client.ConnectAsync();
        }

        private DiscordClient ConfigureAndConnectBot(string configPath)
        {
            var json = string.Empty;
            using (var fs = File.OpenRead(configPath))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
            {
                json = sr.ReadToEnd();
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

        private void ConfigureCommands(DiscordClient client, IServiceProvider services)
        {
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { this.ConfigJson.First(x => x.Key == "prefix").Value },
                EnableDms = false,
                EnableMentionPrefix = true,
                DmHelp = true,
                Services = services
            };

            this.Commands = client.UseCommandsNext(commandsConfig);

            this.Commands.RegisterCommands<AssignClassCommand>();
            this.Commands.RegisterCommands<AssignRoleCommand>();
            this.Commands.RegisterCommands<PurgeCommand>();
            this.Commands.RegisterCommands<ListCommandsCommand>();
            this.Commands.RegisterCommands<AssignCharacterCommand>();
            this.Commands.RegisterCommands<RankingCommand>();
        }

        private void ConfigureInteractivity(DiscordClient client)
        {
            client.UseInteractivity(new InteractivityConfiguration
            {
                 ResponseBehavior = DSharpPlus.Interactivity.Enums.InteractionResponseBehavior.Ignore
            });
        }

        private Task OnClientReady(object sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
