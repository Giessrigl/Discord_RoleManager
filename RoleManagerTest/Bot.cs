using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using RoleManagerTest.Commands;

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
            var token = Environment.GetEnvironmentVariable("Token");
            var prefix = Environment.GetEnvironmentVariable("Prefix");

            if (string.IsNullOrWhiteSpace(token))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("token is null");
            }

            if (string.IsNullOrWhiteSpace(prefix))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("prefix is null");
            }

            try
            {
                var client = ConfigureAndConnectBot(token);
                ConfigureInteractivity(client);

                ConfigureCommands(client, services, prefix);

                client.Ready += OnClientReady;
                client.MessageReactionAdded += ClassCommand.ReactToAddingClass;
                client.MessageReactionAdded += RoleCommand.ReactToAddingRole;

                client.MessageReactionRemoved += ClassCommand.ReactToRemovingClass;
                client.MessageReactionRemoved += RoleCommand.ReactToRemovingRole;

                client.ConnectAsync();

                this.Client = client;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return;
            }
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

            //this.Commands.RegisterCommands<BotSettingsCommand>();
            this.Commands.RegisterCommands<PurgeCommand>();
            //this.Commands.RegisterCommands<AssignMainCharacterCommand>();
            //this.Commands.RegisterCommands<AssignAltCommand>();
            this.Commands.RegisterCommands<ClassCommand>();
            this.Commands.RegisterCommands<RoleCommand>();
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
