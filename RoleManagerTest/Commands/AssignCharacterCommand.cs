using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace RoleManagerTest.Commands
{
    class AssignCharacterCommand : BaseCommandModule
    {
        private readonly HttpClient client = new HttpClient();

        [Command("MyNameIs")]
        [Aliases("Iam")]
        [RequireGuild]
        [RequireBotPermissions(Permissions.ManageRoles)]
        [Description("Assigns a specific class and role to a user, depending on their character name and server name. (Gets the information from Raider.io)")]
        public async Task AssignCharacter(
            CommandContext ctx, 
            [Description("The name of the character.")] string characterName, 
            [Description("The name of the server.")] string serverName, 
            [Description("The name of the region.")]string region = "eu")
        {
            if (string.IsNullOrWhiteSpace(characterName) || string.IsNullOrWhiteSpace(serverName))
                return;

            if (ctx.Channel.Id != Statics.classes_and_roles_ChannelID)
                return;

            var characterInfo = await GetCharacterInfo(characterName, serverName, region);
            if (characterInfo == null)
                return;

            // handle values of characterInfo response content
            var className = characterInfo.First(x => x.Key == "class").Value;
            var specName = characterInfo.First(x => x.Key == "active_spec_name").Value;

            var classToAssign = this.GetCharacterClass(ctx, className);
            var roleToAssign = this.GetCharacterRole(ctx, className, specName);

            if (classToAssign == null || roleToAssign == null)
                return;

            var member = await ctx.Guild.GetMemberAsync(ctx.User.Id);
            if (member == null)
                return;

            try
            {
                await RemoveClassRoles(ctx, member);
                await member.GrantRoleAsync(classToAssign);
                await member.GrantRoleAsync(roleToAssign);

                var embed = this.BuildDiscordEmbed(ctx, characterInfo);

                await ctx.Channel.SendMessageAsync(embed).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task<Dictionary<string, string>> GetCharacterInfo(string characterName, string serverName, string region)
        {
            // defines the query to search for a specified character
            string query;
            using (var parameters = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "region", $"{region}"},
                { "realm", $"{serverName}"},
                { "name", $"{characterName}"},
            }))
            {
                query = await parameters.ReadAsStringAsync();
            }

            // gets the specified character from raider.io api
            var url = @"https://raider.io/api/v1/characters/profile?" + query;

            var response = await client.GetAsync(url);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            var characterInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

            return characterInfo;
        }

        private DiscordRole GetCharacterClass(CommandContext ctx, string className)
        {
            var characterWoWClass = WoW.Classes.FirstOrDefault(c => c.Name == className);
            if (characterWoWClass == null)
            {
                Console.Error.WriteLine($"{className} is not a selectable class!");
                return null;
            }

            return ctx.Guild.GetRole(characterWoWClass.ClassID);
        }

        private DiscordRole GetCharacterRole(CommandContext ctx, string className, string specName)
        {
            var characterWoWClass = WoW.Classes.FirstOrDefault(c => c.Name == className);

            if (characterWoWClass == null)
                return null;

            if (!characterWoWClass.WoWRolesNames.TryGetValue(specName, out string characterRoleName))
                return null;

            var characterWoWRole = WoW.Roles.FirstOrDefault(x => x.Name == characterRoleName);
            if (characterWoWRole == null)
                return null;

            return ctx.Guild.GetRole(characterWoWRole.Id);
        }

        private async Task RemoveClassRoles(CommandContext ctx, DiscordMember member)
        {
            var userClass = member.Roles.FirstOrDefault(role => WoW.Classes.Find(wowClass => wowClass.ClassID == role.Id) != default);
            if (userClass != default)
            {
                await member.RevokeRoleAsync(ctx.Guild.GetRole(userClass.Id));
            }

            var userWoWRole = member.Roles.FirstOrDefault(role => WoW.Roles.Find(wowRole => wowRole.Id == role.Id) != default);
            if (userWoWRole != default)
            {
                await member.RevokeRoleAsync(ctx.Guild.GetRole(userWoWRole.Id));
            }
        }

        private DiscordEmbed BuildDiscordEmbed(CommandContext ctx, Dictionary<string, string> characterInfo)
        {
            // get values
            var characterName = characterInfo.First(x => x.Key == "name").Value;
            var faction = characterInfo.First(x => x.Key == "faction").Value;
            var gender = characterInfo.First(x => x.Key == "gender").Value;
            var race = characterInfo.First(x => x.Key == "race").Value;
            var className = characterInfo.First(x => x.Key == "class").Value;
            var specName = characterInfo.First(x => x.Key == "active_spec_name").Value;
            var realm = characterInfo.First(x => x.Key == "realm").Value;

            var profileUrl = characterInfo.First(x => x.Key == "profile_url").Value;

            var WoWClass = WoW.Classes.FirstOrDefault(x => x.Name == className);
            if (WoWClass == null)
                return null;

            var classEmoji = DiscordEmoji.FromGuildEmote(ctx.Client, WoWClass.EmojiID);

            WoW.ClassColors.TryGetValue(className, out var classColor);

            // build the embed
            DiscordEmbedBuilder blueprint = new DiscordEmbedBuilder();

            blueprint.Color = classColor;
            blueprint.Title = $"{ctx.User.Username} is now: {characterName}!";
            blueprint.WithThumbnail(classEmoji.Url);

            blueprint.AddField("CharacterInfo",
                $"Realm: {realm} \n" +
                $"Faction: {char.ToUpper(faction[0])}{faction.Substring(1)} \n" +
                $"Class: {className} \n" + 
                $"Spec: {specName} \n" +
                $"Race: {race} \n" +
                $"Sex: {char.ToUpper(gender[0])}{gender.Substring(1)} \n");

            if (gender == "female")
                blueprint.AddField("Check her out at:", $"{profileUrl}");
            else
                blueprint.AddField("Check him out at:", $"{profileUrl}");

            return blueprint.Build();
        }
    }
}
