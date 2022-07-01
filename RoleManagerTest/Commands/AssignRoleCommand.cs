using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoleManagerTest.Commands
{
    public class AssignRoleCommand : BaseCommandModule
    {
        [Command("role")]
        [RequireGuild]
        [RequireBotPermissions(Permissions.ManageRoles)]
        [RequireUserPermissions(Permissions.Administrator)]
        [Description("Assigns a specific role to a user, depending on their class (Admin-only).")]
        public async Task AssignRole(CommandContext ctx, string roleName)
        {
            if (ctx.Channel.Id != Statics.classes_and_roles_ChannelID)
                return;

            var roleToassign = WoW.Roles.FirstOrDefault(c => c.Name == roleName);
            if (roleToassign == default)
            {
                Console.Error.WriteLine($"{roleName} is not a selectable role! {ctx.User} - {DateTime.Now}");
                return;
            }

            Console.WriteLine($"{ctx.User.Username} wants to be a {roleToassign.Name}!");
            var member = await ctx.Guild.GetMemberAsync(ctx.User.Id);

            var userWoWClass = WoW.Classes.FirstOrDefault(@class => member.Roles.FirstOrDefault(role => role.Id == @class.ClassID) != default);
            if (userWoWClass == null)
            {
                Console.WriteLine($"{ctx.User.Username} has no class assigned and therefore can not have a role!");
                return;
            }

            if (!userWoWClass.WoWRolesNames.Exists(role => role == roleToassign.Name))
            {
                Console.WriteLine($"{ctx.User.Username} can not have the role {roleToassign.Name} because his class is a {userWoWClass.Name}!");
                return;
            }

            var roleToAssign = ctx.Guild.GetRole(roleToassign.Id);
            var userWoWRole = member.Roles.FirstOrDefault(memberRole => WoW.Roles.Find(role => memberRole.Id == role.Id) != default);
            if (userWoWRole != default)
            {
                await member.RevokeRoleAsync(ctx.Guild.GetRole(userWoWRole.Id));
            }

            await member.GrantRoleAsync(roleToAssign);
        }

        [Command("roles")]
        [RequireBotPermissions(Permissions.ManageRoles)]
        [RequireUserPermissions(Permissions.Administrator)]
        [Description("Assigns a role to a user, depending on their class, after reacting to the role assignment text (Admin-only).")]
        public async Task AssignRoles(CommandContext ctx)
        {
            if (ctx.Channel.Id != Statics.classes_and_roles_ChannelID)
                return;

            var interactivity = ctx.Client.GetInteractivity();
            var message = await ctx.Channel.SendMessageAsync("Only choose your main role!").ConfigureAwait(false);

            await this.CreateRoleReactions(ctx, message).ConfigureAwait(false);

            interactivity.Client.MessageReactionAdded += async (sender, args) =>
            {
                var roleToassign = WoW.Roles.FirstOrDefault(c => c.EmojiID == args.Emoji.Id);
                if (roleToassign == default)
                {
                    Console.Error.WriteLine($"{args.Emoji.Id} is not a selectable role! {ctx.User} - {DateTime.Now}");
                    return;
                }

                Console.WriteLine($"{args.User.Username} wants to be a {roleToassign.Name}!");
                var member = await ctx.Guild.GetMemberAsync(args.User.Id);

                var userWoWClass = WoW.Classes.FirstOrDefault(@class => member.Roles.FirstOrDefault(role => role.Id == @class.ClassID) != default);
                if (userWoWClass == null)
                {
                    Console.WriteLine($"{args.User.Username} has no class assigned and therefore can not have a role!");
                    return;
                }
                    
                if (!userWoWClass.WoWRolesNames.Exists(role => role == roleToassign.Name))
                {
                    Console.WriteLine($"{args.User.Username} can not have the role {roleToassign.Name} because his class is a {userWoWClass.Name}!");
                    return;
                }

                var roleToAssign = args.Guild.GetRole(roleToassign.Id);
                var userWoWRole = member.Roles.FirstOrDefault(memberRole => WoW.Roles.Find(role => memberRole.Id == role.Id) != default);
                if (userWoWRole != default)
                {
                    await member.RevokeRoleAsync(args.Guild.GetRole(userWoWRole.Id));
                }

                await member.GrantRoleAsync(roleToAssign);
                //await this.SendAssignmentMessage(args);
            };
        }

        private async Task CreateRoleReactions(CommandContext ctx, DiscordMessage message)
        {
            foreach (var role in WoW.Roles)
            {
                if (role.EmojiID != default)
                    await message.CreateReactionAsync(DiscordEmoji.FromGuildEmote(ctx.Client, role.EmojiID)).ConfigureAwait(false);
            }
        }
    }
}
