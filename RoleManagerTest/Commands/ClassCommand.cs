using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;

namespace RoleManagerTest.Commands
{
    public class ClassCommand : BaseCommandModule
    {
        [Command("Class")]
        [RequireGuild]
        [RequireUserPermissions(Permissions.Administrator)]
        [RequireBotPermissions(Permissions.ManageRoles)]
        [Description("")]
        public async Task AssignClass(CommandContext ctx)
        {
            if (ctx.Channel.Id != Statics.classes_and_roles_ChannelID)
                return;

            var interactivity = ctx.Client.GetInteractivity();
            var message = await ctx.Channel.SendMessageAsync(BuildDiscordMessage(ctx)).ConfigureAwait(false);

            await this.CreateClassReactions(ctx, message).ConfigureAwait(false);

            interactivity.Client.MessageReactionAdded += AddClass;

            interactivity.Client.MessageReactionRemoved += RemoveClass;
        }

        private async Task AddClass(DiscordClient sender, MessageReactionAddEventArgs e)
        {
            var classToassign = WoW.Classes.FirstOrDefault(c => c.EmojiID == e.Emoji.Id);
            if (classToassign == default)
            {
                Console.Error.WriteLine($"{e.Emoji.Id} is not a selectable class! {sender} - {DateTime.Now}");
                return;
            }

            Console.WriteLine($"{e.User.Username} wants to be a {classToassign.Name}!");
            var member = await e.Guild.GetMemberAsync(e.User.Id);
            var roleToassign = e.Guild.GetRole(classToassign.ClassID);

            await member.GrantRoleAsync(roleToassign);
        }

        private async Task RemoveClass(DiscordClient sender, MessageReactionRemoveEventArgs e)
        {
            var classToassign = WoW.Classes.FirstOrDefault(c => c.EmojiID == e.Emoji.Id);
            if (classToassign == default)
            {
                Console.Error.WriteLine($"{e.Emoji.Id} is not a selectable class! {sender} - {DateTime.Now}");
                return;
            }

            Console.WriteLine($"{e.User.Username} wants to be a {classToassign.Name}!");
            var member = await e.Guild.GetMemberAsync(e.User.Id);
            var roleToassign = e.Guild.GetRole(classToassign.ClassID);
            if (roleToassign != default)
            {
                await member.RevokeRoleAsync(e.Guild.GetRole(roleToassign.Id));
            }
        }

        private async Task CreateClassReactions(CommandContext ctx, DiscordMessage message)
        {
            foreach (var wowClass in WoW.Classes)
            {
                if (wowClass.EmojiID != default)
                    await message.CreateReactionAsync(DiscordEmoji.FromGuildEmote(ctx.Client, wowClass.EmojiID)).ConfigureAwait(false);
            }
        }

        private DiscordMessageBuilder BuildDiscordMessage(CommandContext ctx)
        {
            string lines = "";
            foreach (var wowClass in WoW.Classes)
            {
                var emoji = DiscordEmoji.FromGuildEmote(ctx.Client, wowClass.EmojiID);

                lines += $"<:{emoji.Name}:{emoji.Id}> <@&{wowClass.ClassID}> \n";
            }

            var message = new DiscordMessageBuilder().WithContent("**React to choose your class(es)! ** \n  \n" +
                lines);

            return message;
        }
    }
}
