using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace RoleManagerTest.Commands
{
    public class ListCommandsCommand : BaseCommandModule
    {
        private readonly List<string> commandDescriptions;

        public ListCommandsCommand()
        {
            commandDescriptions = new List<string>()
            {
                Statics.GetCommandDescription(typeof(ListCommandsCommand), "ListCommands"),
                Statics.GetCommandDescription(typeof(PurgeCommand), "PurgeMessages"),
                Statics.GetCommandDescription(typeof(AssignClassCommand), "AssignClass"),
                Statics.GetCommandDescription(typeof(AssignClassCommand), "AssignClasses"),
                Statics.GetCommandDescription(typeof(AssignRoleCommand), "AssignRole"),
                Statics.GetCommandDescription(typeof(AssignRoleCommand), "AssignRoles")
            };
        }

        [Command("commands")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.SendMessagesInThreads)]
        [Description("Lists all available commands.")]
        public async Task ListCommands(CommandContext ctx)
        {
            string message = "\r\n";
            foreach(var command in commandDescriptions)
            {
                message += $"_{command}\n";
            }

            await ctx.Channel.SendMessageAsync(message).ConfigureAwait(false);
        }
    }
}
