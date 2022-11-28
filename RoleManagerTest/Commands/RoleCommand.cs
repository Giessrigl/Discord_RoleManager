using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;

namespace RoleManagerTest.Commands
{
    public class RoleCommand : BaseCommandModule
    {
        [Command("Role")]
        [RequireGuild]
        [RequireUserPermissions(Permissions.Administrator)]
        [RequireBotPermissions(Permissions.ManageRoles)]
        [Description("")]
        public async Task AssignRole(CommandContext ctx)
        {
            if (ctx.Channel.Id != Statics.classes_and_roles_ChannelID)
                return;

            var classRole = ctx.Guild.GetRole(WoW.Seperators[0].Id);
            var roleRole = ctx.Guild.GetRole(WoW.Seperators[1].Id);
            foreach (var member in ctx.Guild.Members)
            {
                if (member.Value.Roles.FirstOrDefault(x => x.Id == classRole.Id) == null)
                    await member.Value.GrantRoleAsync(classRole);

                if (member.Value.Roles.FirstOrDefault(x => x.Id == roleRole.Id) == null)
                    await member.Value.GrantRoleAsync(roleRole);
            }

            var interactivity = ctx.Client.GetInteractivity();
            var message = await ctx.Channel.SendMessageAsync(BuildDiscordMessage(ctx)).ConfigureAwait(false);

            await this.CreateRoleReactions(ctx, message).ConfigureAwait(false);

            interactivity.Client.MessageReactionAdded += AddRole;

            interactivity.Client.MessageReactionRemoved += RemoveRole;
        }

        private async Task AddRole(DiscordClient sender, MessageReactionAddEventArgs e)
        {
            var classToassign = WoW.Roles.FirstOrDefault(c => c.EmojiID == e.Emoji.Id);
            if (classToassign == default)
            {
                Console.Error.WriteLine($"{e.Emoji.Id} is not a selectable role! {sender} - {DateTime.Now}");
                return;
            }

            Console.WriteLine($"{e.User.Username} wants to be a {classToassign.Name}!");
            var member = await e.Guild.GetMemberAsync(e.User.Id);
            var roleToassign = e.Guild.GetRole(classToassign.Id);

            await member.GrantRoleAsync(roleToassign);
        }

        private async Task RemoveRole(DiscordClient sender, MessageReactionRemoveEventArgs e)
        {
            var classToassign = WoW.Roles.FirstOrDefault(c => c.EmojiID == e.Emoji.Id);
            if (classToassign == default)
            {
                Console.Error.WriteLine($"{e.Emoji.Id} is not a selectable class! {sender} - {DateTime.Now}");
                return;
            }

            Console.WriteLine($"{e.User.Username} wants to be a {classToassign.Name}!");
            var member = await e.Guild.GetMemberAsync(e.User.Id);
            var roleToassign = e.Guild.GetRole(classToassign.Id);
            if (roleToassign != default)
            {
                await member.RevokeRoleAsync(e.Guild.GetRole(roleToassign.Id));
            }
        }

        private async Task CreateRoleReactions(CommandContext ctx, DiscordMessage message)
        {
            foreach (var wowRole in WoW.Roles)
            {
                if (wowRole.EmojiID != default)
                    await message.CreateReactionAsync(DiscordEmoji.FromGuildEmote(ctx.Client, wowRole.EmojiID)).ConfigureAwait(false);
            }
        }

        private DiscordMessageBuilder BuildDiscordMessage(CommandContext ctx)
        {
            string lines = "";
            foreach (var wowRoles in WoW.Roles)
            {
                var emoji = DiscordEmoji.FromGuildEmote(ctx.Client, wowRoles.EmojiID);

                lines += $"<:{emoji.Name}:{emoji.Id}> <@&{wowRoles.Id}> \n";
            }

            var message = new DiscordMessageBuilder().WithContent("**React to choose your role(s)! ** \n  \n" +
                lines);

            return message;
        }

    }
}
