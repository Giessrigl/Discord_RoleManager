using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using RoleManagerTest.Affixes;

namespace RoleManagerTest.Commands
{
    public class GetMPlusAffixesCommand : BaseCommandModule
    {
        private bool isRunning;

        private AffixRotation rotation;

        public GetMPlusAffixesCommand() //UserClassContext context)
        {
            isRunning = false;
            rotation = new AffixRotation();
        }

        [Command("Affixes")]
        [RequireGuild]
        [RequireBotPermissions(Permissions.SendMessages)]
        [Description("")]
        public async Task GetAffixes(CommandContext ctx)
        {
            if (isRunning)
                return;

            await ctx.Channel.SendMessageAsync("Start affix rotation.").ConfigureAwait(false);

            isRunning = true;

            while (!(DateTime.Now.DayOfWeek == DayOfWeek.Wednesday))
            {
                await Task.Delay(100000);
            }
            
            try
            {
                var embed = BuildAffixDiscordEmbed();
                await ctx.Channel.SendMessageAsync(embed).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }


            while (isRunning)
            {
                try
                {
                    await StartCountdown();
                    var embed = BuildAffixDiscordEmbed();
                    await ctx.Channel.SendMessageAsync(embed).ConfigureAwait(false);
                }
                catch (Exception ex)
                {

                }
            }
        }

        private DiscordEmbed BuildAffixDiscordEmbed()
        {
            var affixWeek = rotation.GetAffixWeek();

            DiscordEmbedBuilder blueprint = new DiscordEmbedBuilder();

            blueprint.Color = DiscordColor.Gold;
            blueprint.Title = "Affixes of this week";

            foreach(var affix in affixWeek.Affixes)
            {
                blueprint.AddField(affix.Name, "");
            }
            
            return blueprint.Build();
        }

        private async Task StartCountdown()
        {
            await Task.Delay(604799000);
            return;
        }
    }
}
