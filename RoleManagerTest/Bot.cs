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

        public Bot(IServiceProvider services, ConfigurationManager config)
        {
            var token = config.GetValue<string>("token");
            var prefix = config.GetValue<string>("prefix");

            var client = ConfigureAndConnectBot(token);
            ConfigureInteractivity(client);
            ConfigureCommands(client, services, prefix);

            client.Ready += OnClientReady;

            client.ConnectAsync();

            this.Client = client;
        }

        private DiscordClient ConfigureAndConnectBot(string token)
        {
            var config = new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                Intents = DiscordIntents.All
            };

            var client = new DiscordClient(config);

            return client;
        }

        private void ConfigureCommands(DiscordClient client, IServiceProvider services, string prefix)
        {
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { prefix },
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
            this.Interactivity = client.UseInteractivity(new InteractivityConfiguration
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
