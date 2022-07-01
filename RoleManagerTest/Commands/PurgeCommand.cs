using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace RoleManagerTest.Commands
{
    public class PurgeCommand : BaseCommandModule
    {
        public string GetAttribute()
        {
            throw new NotImplementedException();
        }

        [Command("purge")]
        [Description("Removes X messages from the current channel. If X is not defined, it is set to 1000. Messages older than 14 days can't be removed.")]
        [RequirePermissions(Permissions.ManageMessages)]
        public async Task PurgeMessages(CommandContext ctx, [Description("The amount of messages to delete. (if not mentioned, set to 1000)")] int amount = 1000)
        {
            var filteredMessages = (await ctx.Channel.GetMessagesAsync(amount)).Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);

            // Get the total amount of messages.
            var count = filteredMessages.Count();

            // Check if there are any messages to delete.
            if (count == 0)
                return;

            // The cast here isn't needed if you're using Discord.Net 1.x,
            // but I'd recommend leaving it as it's what's required on 2.x, so
            // if you decide to update you won't have to change this line.
            await ctx.Channel.DeleteMessagesAsync(filteredMessages);
        }
    }
}
