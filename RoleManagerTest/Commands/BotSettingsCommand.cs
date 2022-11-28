using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace RoleManagerTest.Commands
{
    //public class BotSettingsCommand : BaseCommandModule
    //{
    //    private readonly Action<DiscordClient, bool> attachRemovers;

    //    public BotSettingsCommand(params ICharacterRemover[] removers)
    //    {
    //        this.attachRemovers = new Action<DiscordClient, bool>((client, remove) =>
    //        {
    //            if (remove)
    //            {
    //                foreach (var remover in removers)
    //                {
    //                    client.GuildMemberRemoved += remover.DeleteChars;
    //                }
    //            }
    //            else
    //            {
    //                foreach (var remover in removers)
    //                {
    //                    client.GuildMemberRemoved -= remover.DeleteChars;
    //                }
    //            }
    //        });
    //    }

    //    [Command("AutoRemoveChars")]
    //    [RequireGuild]
    //    [RequirePermissions(Permissions.Administrator)]
    //    [Description("Removes the characters of a former member after leaving, kicking, banning from the server.")]
    //    public async Task RemoveCharacters(
    //        CommandContext ctx, string removeBool)
    //    {
    //        if (!bool.TryParse(removeBool, out bool shouldRemove))
    //            return;

    //        attachRemovers.Invoke(ctx.Client, shouldRemove);
    //    }
    //}
}
