using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using RoleManagerTest.GoogleAPI;

namespace RoleManagerTest.Commands
{
    class AssignMainCharacterCommand : BaseCommandModule
    {
        private readonly MainCharacterSpreadsheetService _mainCharTableService;
        private readonly AltSpreadsheetService _altTableService;

        private readonly HttpClient _client;

        public AssignMainCharacterCommand(HttpClient client, MainCharacterSpreadsheetService mainCharTableService, AltSpreadsheetService altTableService) //UserClassContext context)
        {
            this._client = client;
            this._mainCharTableService = mainCharTableService;
            this._altTableService = altTableService;
        }

        [Command("Main")]
        [RequireGuild]
        [RequireBotPermissions(Permissions.ManageRoles)]
        [Description("Assigns a specific class and role to a user, depending on their character name and server name. (Gets the information from Raider.io)")]
        public async Task AssignCharacter(
            CommandContext ctx,
            [Description("The name of the character.")] string characterName,
            [Description("The name of the server.")] string serverName,
            [Description("The name of the offspecc")] string offspecc = "",
            [Description("The name of the region.")] string region = "eu")
        {
            Console.WriteLine($"{ctx.Member.DisplayName} assignes a character: {characterName}, {serverName}, {region}, {offspecc}");

            if (string.IsNullOrWhiteSpace(characterName) || string.IsNullOrWhiteSpace(serverName))
                return;

            if (ctx.Channel.Id != Statics.classes_and_roles_ChannelID)
                return;

            if (await this._mainCharTableService.IsCharacterRegistered(characterName, serverName, region))
            {
                await ctx.Channel.SendMessageAsync("The character is already registered.").ConfigureAwait(false);
                return;
            }

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

            if (!WoW.IsSpeccValid(className, offspecc) || specName == offspecc)
                offspecc = string.Empty;

            var offspecToAssign = this.GetCharacterOffspecRole(ctx, className, offspecc);

            await this._altTableService.DeleteChar(ctx.Client, characterName);

            if (!await this.AddCharacterToUser(ctx.User.Id, characterName, serverName, region, className, specName, offspecc))
                return;

            try
            {
                await RemoveClassRoles(ctx, member);
                await member.GrantRoleAsync(classToAssign);
                await member.GrantRoleAsync(roleToAssign);

                if (offspecToAssign != null && !offspecToAssign.Name.StartsWith(roleToAssign.Name))
                    await member.GrantRoleAsync(offspecToAssign);

                var embed = this.BuildCharacterDiscordEmbed(ctx, $"{ctx.User.Username} is now: {characterName}!", characterInfo, offspecc);

                await ctx.Channel.SendMessageAsync(embed).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [Command("Respec")]
        [RequireGuild]
        [RequireBotPermissions(Permissions.ManageRoles)]
        [Description("Assigns a new main spec to an already existing user-corresponding main character.")]
        public async Task Respecc(
            CommandContext ctx,
            [Description("The name of the WoW char specialization.")] string specc)
        {
            var WoWChar = await this._mainCharTableService.GetCharacterOfUser(ctx.User.Id.ToString());

            if (WoWChar == null)
                return;

            // look if specc is valid
            if (!WoW.IsSpeccValid(WoWChar.ClassName, specc))
                return;

            if (WoWChar.SpeccName == specc)
                return;

            var classToAssign = this.GetCharacterClass(ctx, WoWChar.ClassName);

            DiscordRole roleToAssign;
            DiscordRole offspecToAssign;
            if (WoWChar.Offspecc == specc)
            {
                // change Mainspec to Offspec and Offspec to MainSpec
                roleToAssign = this.GetCharacterRole(ctx, WoWChar.ClassName, WoWChar.Offspecc);
                offspecToAssign = this.GetCharacterOffspecRole(ctx, WoWChar.ClassName, WoWChar.SpeccName);
            }
            else
            {
                // change mainspec but keep offspec
                roleToAssign = this.GetCharacterRole(ctx, WoWChar.ClassName, specc);
                offspecToAssign = this.GetCharacterOffspecRole(ctx, WoWChar.ClassName, WoWChar.Offspecc);
            }

            if (classToAssign == null || roleToAssign == null)
                return;

            var member = await ctx.Guild.GetMemberAsync(ctx.User.Id);
            if (member == null)
                return;

            if (WoWChar.Offspecc == specc)
            {
                // change Mainspec to Offspec and Offspec to MainSpec
                if (!await this.AddCharacterToUser(ctx.User.Id, WoWChar.CharName, WoWChar.ServerName, WoWChar.Region, WoWChar.ClassName, WoWChar.Offspecc, WoWChar.SpeccName))
                    return;
            }
            else
            {
                // change mainspec but keep offspec
                if (!await this.AddCharacterToUser(ctx.User.Id, WoWChar.CharName, WoWChar.ServerName, WoWChar.Region, WoWChar.ClassName, specc, WoWChar.Offspecc))
                    return;
            }

            try
            {
                await RemoveClassRoles(ctx, member);
                await member.GrantRoleAsync(classToAssign);
                await member.GrantRoleAsync(roleToAssign);
                if (offspecToAssign != null && !offspecToAssign.Name.StartsWith(roleToAssign.Name))
                    await member.GrantRoleAsync(offspecToAssign);

                await ctx.Channel.SendMessageAsync($"{member.Username} respec'd mainspec to {specc}.").ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [Command("Offspec")]
        [RequireGuild]
        [RequireBotPermissions(Permissions.ManageRoles)]
        [Description("Assigns a new offspec to an already existing user-corresponding main character.")]
        public async Task OffSpec(
            CommandContext ctx,
            [Description("The name of the WoW char specialization.")] string specc)
        {
            var WoWChar = await this._mainCharTableService.GetCharacterOfUser(ctx.User.Id.ToString());

            if (WoWChar == null)
                return;

            // look if specc is valid
            if (!WoW.IsSpeccValid(WoWChar.ClassName, specc))
                return;

            if (WoWChar.SpeccName == specc || WoWChar.Offspecc == specc)
                return;

            var classToAssign = this.GetCharacterClass(ctx, WoWChar.ClassName);

            DiscordRole roleToAssign = this.GetCharacterRole(ctx, WoWChar.ClassName, WoWChar.SpeccName);
            DiscordRole offspecToAssign = this.GetCharacterOffspecRole(ctx, WoWChar.ClassName, specc);

            if (classToAssign == null || roleToAssign == null || offspecToAssign == null)
                return;

            var member = await ctx.Guild.GetMemberAsync(ctx.User.Id);
            if (member == null)
                return;

            // change Mainspec to Offspec and Offspec to MainSpec
            if (!await this.AddCharacterToUser(ctx.User.Id, WoWChar.CharName, WoWChar.ServerName, WoWChar.Region, WoWChar.ClassName, WoWChar.SpeccName, specc))
                return;

            try
            {
                await RemoveClassRoles(ctx, member);
                await member.GrantRoleAsync(classToAssign);
                await member.GrantRoleAsync(roleToAssign);
                if (offspecToAssign != null && !offspecToAssign.Name.StartsWith(roleToAssign.Name)) // if Range Offspec == Range Main
                    await member.GrantRoleAsync(offspecToAssign);

                await ctx.Channel.SendMessageAsync($"{member.Username} respec'd offspec to {specc}.").ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [Command("GetChar")]
        [Aliases("WhoamI")]
        [RequireGuild]
        [RequireBotPermissions(Permissions.SendMessages)]
        [Description("")]
        public async Task GetCharacter(CommandContext ctx)
        {
            var WoWChar = await this._mainCharTableService.GetCharacterOfUser(ctx.User.Id.ToString());
            
            if (WoWChar == null)
                return;
                
            var charInfo = await this.GetCharacterInfo(WoWChar.CharName, WoWChar.ServerName, WoWChar.Region);

            if (charInfo == null)
                return;

            var characterName = charInfo.First(x => x.Key == "name").Value;

            try
            {
                var embed = this.BuildCharacterDiscordEmbed(ctx, $"{ctx.User.Username}, you are {characterName}!", charInfo, WoWChar.Offspecc);

                await ctx.Channel.SendMessageAsync(embed).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }
        }

        // retrieves character info from the Raider.io API.
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

            var response = await this._client.GetAsync(url);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            var characterInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

            return characterInfo;
        }

        // gets the corresponding discord role for a specific WoW class.
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

        // gets the corresponding discord role for a specific WoW class specialization.
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

        private DiscordRole GetCharacterOffspecRole(CommandContext ctx, string className, string specName)
        {
            var characterWoWClass = WoW.Classes.FirstOrDefault(c => c.Name == className);

            if (characterWoWClass == null)
                return null;

            if (!characterWoWClass.WoWRolesNames.TryGetValue(specName, out string characterRoleName))
                return null;

            var characterWoWRole = WoW.OffspecRoles.FirstOrDefault(x => x.Name == characterRoleName + " Offspec");
            if (characterWoWRole == null)
                return null;

            return ctx.Guild.GetRole(characterWoWRole.Id);
        }

        private async Task RemoveClassRoles(CommandContext ctx, DiscordMember member)
        {
            var userClass = member.Roles.FirstOrDefault(role => WoW.Classes.Find(wowClass => wowClass.ClassID == role.Id) != default);
            if (userClass != default)
                await member.RevokeRoleAsync(ctx.Guild.GetRole(userClass.Id));

            var userWoWRole = member.Roles.FirstOrDefault(role => WoW.Roles.Find(wowRole => wowRole.Id == role.Id) != default);
            if (userWoWRole != default)
                await member.RevokeRoleAsync(ctx.Guild.GetRole(userWoWRole.Id));

            var offspecRole = member.Roles.FirstOrDefault(role => WoW.OffspecRoles.Find(wowRole => wowRole.Id == role.Id) != default);
            if (offspecRole != default)
                await member.RevokeRoleAsync(ctx.Guild.GetRole(offspecRole.Id));
        }

        // Builds a Discord Embed after correctly assigning a Character to a Discord Member.
        private DiscordEmbed BuildCharacterDiscordEmbed(CommandContext ctx, string title, Dictionary<string, string> characterInfo, string offspecc)
        {
            // get values
            var characterName = characterInfo.First(x => x.Key == "name").Value;
            var faction = characterInfo.First(x => x.Key == "faction").Value;
            var gender = characterInfo.First(x => x.Key == "gender").Value;
            var race = characterInfo.First(x => x.Key == "race").Value;
            var className = characterInfo.First(x => x.Key == "class").Value;
            var specName = characterInfo.First(x => x.Key == "active_spec_name").Value;
            var realm = characterInfo.First(x => x.Key == "realm").Value;
            var charPortrait = characterInfo.First(x => x.Key == "thumbnail_url").Value;

            var profileUrl = characterInfo.First(x => x.Key == "profile_url").Value;

            var WoWClass = WoW.Classes.FirstOrDefault(x => x.Name == className);
            if (WoWClass == null)
                return null;

            var classEmoji = DiscordEmoji.FromGuildEmote(ctx.Client, WoWClass.EmojiID);

            WoW.ClassColors.TryGetValue(className, out var classColor);

            // build the embed
            DiscordEmbedBuilder blueprint = new DiscordEmbedBuilder();

            blueprint.Color = classColor;
            blueprint.Title = title;
            blueprint.WithThumbnail(classEmoji.Url);

            blueprint.WithImageUrl(charPortrait);
            blueprint.AddField("CharacterInfo",
                $"Realm: {realm} \n" +
                $"Faction: {char.ToUpper(faction[0])}{faction.Substring(1)} \n" +
                $"Class: {className} \n" + 
                $"Spec: {specName} \n" +
                $"Offspec: {offspecc} \n" +
                $"Race: {race} \n" +
                $"Sex: {char.ToUpper(gender[0])}{gender.Substring(1)} \n");

            if (gender == "female")
                blueprint.AddField("Check her out at:", $"{profileUrl}");
            else
                blueprint.AddField("Check him out at:", $"{profileUrl}");

            return blueprint.Build();
        }

        private async Task<bool> AddCharacterToUser(ulong userID, string charName, string serverName, string region, string className, string speccName, string offspecc)
        {
            try
            {
                await this._mainCharTableService.AddCharacter(new SpreadsheetCharacter(userID.ToString(), region, serverName, charName, className, speccName, offspecc));
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(false);
            }
        }

        
    }
}
