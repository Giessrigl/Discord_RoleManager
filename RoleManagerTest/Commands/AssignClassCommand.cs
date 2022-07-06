using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;

namespace RoleManagerTest.Commands
{
    public class AssignClassCommand : BaseCommandModule
    {
        private readonly Random rndm;

        public AssignClassCommand()
        {
            this.rndm = new Random();
        }

        [Command("class")]
        [RequireGuild]
        [RequireBotPermissions(Permissions.ManageRoles)]
        [RequireUserPermissions(Permissions.Administrator)]
        [Description("Assigns a specific class to a user. " +
            "Adding a role is optional. " +
            "If your role does not match with your new class, you will not be assigned a role.")]
        public async Task AssignClass(CommandContext ctx, [Description("The name of the class.")] string className, [Description("The name of the role.")] string role = "")
        {
            if (ctx.Channel.Id != Statics.classes_and_roles_ChannelID)
                return;

            var classToassign = WoW.Classes.FirstOrDefault(c => c.Name == className);
            if (classToassign == default)
            {
                Console.Error.WriteLine($"{className} is not a selectable class! {ctx.User} - {DateTime.Now}");
                return;
            }

            Console.WriteLine($"{ctx.User.Username} wants to be a {classToassign.Name}!");
            var member = await ctx.Guild.GetMemberAsync(ctx.User.Id);
            var roleToassign = ctx.Guild.GetRole(classToassign.ClassID);
            var userWoWClass = member.Roles.FirstOrDefault(role => WoW.Classes.Find(wowClass => wowClass.ClassID == role.Id) != default);
            if (userWoWClass != default)
            {
                await member.RevokeRoleAsync(ctx.Guild.GetRole(userWoWClass.Id));
            }

            await member.GrantRoleAsync(roleToassign);

            var userWoWRole = member.Roles.FirstOrDefault(role => WoW.Roles.Find(wowRole => wowRole.Id == role.Id) != default);
            if (userWoWRole != default)
            {
                await member.RevokeRoleAsync(ctx.Guild.GetRole(userWoWRole.Id));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                var roleCommand = new AssignRoleCommand();
                await roleCommand.AssignRole(ctx, role);
            }
        }

        [Command("classes")]
        [RequireGuild]
        [RequireBotPermissions(Permissions.ManageRoles)]
        [RequireUserPermissions(Permissions.Administrator)]
        [Description("Assigns a class to a user after reacting to the role assignment text (Admin-only).")]
        public async Task AssignClasses(CommandContext ctx)
        {
            if (ctx.Channel.Id != Statics.classes_and_roles_ChannelID)
                return;

            var interactivity = ctx.Client.GetInteractivity();
            var message = await ctx.Channel.SendMessageAsync("Only choose your main class!").ConfigureAwait(false);

            await this.CreateClassReactions(ctx, message).ConfigureAwait(false);

            interactivity.Client.MessageReactionAdded += async (sender, args) =>
            {
                var classToassign = WoW.Classes.FirstOrDefault(c => c.EmojiID == args.Emoji.Id);
                if (classToassign == default)
                {
                    Console.Error.WriteLine($"{args.Emoji.Id} is not a selectable class! {ctx.User} - {DateTime.Now}");
                    return;
                }

                Console.WriteLine($"{args.User.Username} wants to be a {classToassign.Name}!");
                var member = await ctx.Guild.GetMemberAsync(args.User.Id);
                var roleToassign = args.Guild.GetRole(classToassign.ClassID);
                var userWoWClass = member.Roles.FirstOrDefault(role => WoW.Classes.Find(wowClass => wowClass.ClassID == role.Id) != default);
                if (userWoWClass != default)
                {
                    await member.RevokeRoleAsync(args.Guild.GetRole(userWoWClass.Id));
                }

                await member.GrantRoleAsync(roleToassign);

                var userWoWRole = member.Roles.FirstOrDefault(role => WoW.Roles.Find(wowRole => wowRole.Id == role.Id) != default);
                if (userWoWRole != default)
                {
                    await member.RevokeRoleAsync(args.Guild.GetRole(userWoWRole.Id));
                }
            };
        }

        private async Task CreateClassReactions(CommandContext ctx, DiscordMessage message)
        { 
            foreach (var wowClass in WoW.Classes)
            {
                if (wowClass.EmojiID != default)
                    await message.CreateReactionAsync(DiscordEmoji.FromGuildEmote(ctx.Client, wowClass.EmojiID)).ConfigureAwait(false);
            }
        }
    }
}
