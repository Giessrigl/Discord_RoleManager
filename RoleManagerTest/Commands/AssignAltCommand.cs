using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json;
using RoleManagerTest.GoogleAPI;

namespace RoleManagerTest.Commands
{
    public class AssignAltCommand : BaseCommandModule
    {
        private readonly MainCharacterSpreadsheetService _mainCharTableService;
        private readonly AltSpreadsheetService _altTableService;

        private readonly HttpClient _client;

        public AssignAltCommand(HttpClient client, MainCharacterSpreadsheetService mainCharTableService, AltSpreadsheetService altService) //UserClassContext context)
        {
            this._client = client;
            this._mainCharTableService = mainCharTableService;
            this._altTableService = altService;
        }

        [Command("Alt")]
        [RequireGuild]
        [RequireBotPermissions(Permissions.ManageRoles)]
        [Description("Assigns a specific class and role to a user as alt / alt, depending on their character name and server name. (Gets the information from Raider.io)")]
        public async Task AssignCharacter(
            CommandContext ctx,
            [Description("The name of the character.")] string characterName,
            [Description("The name of the server.")] string serverName,
            [Description("The name of the region.")] string region = "eu",
            [Description("The name of the offspecc")] string offspecc = "")
        {
            //Console.WriteLine($"{ctx.Member.DisplayName} assignes a alt: {characterName}, {serverName}, {region}, {offspecc}");

            //if (string.IsNullOrWhiteSpace(characterName) || string.IsNullOrWhiteSpace(serverName))
            //    return;

            //if (ctx.Channel.Id != Statics.classes_and_roles_ChannelID)
            //    return;

            //if (await this._altTableService.IsCharacterRegistered(characterName, serverName, region))
            //{
            //    await ctx.Channel.SendMessageAsync("The character is already registered.").ConfigureAwait(false);
            //    return;
            //}

            //var characterInfo = await GetCharacterInfo(characterName, serverName, region);
            //if (characterInfo == null)
            //    return;

            //// handle values of characterInfo response content
            //var className = characterInfo.First(x => x.Key == "class").Value;
            //var specName = characterInfo.First(x => x.Key == "active_spec_name").Value;

            //var classToAssign = this.GetCharacterClass(ctx, className);
            //var roleToAssign = this.GetCharacterRole(ctx, className, specName);

            //if (classToAssign == null || roleToAssign == null)
            //    return;

            //var member = await ctx.Guild.GetMemberAsync(ctx.User.Id);
            //if (member == null)
            //    return;

            //if (!IsSpeccValid(className, offspecc) || specName == offspecc)
            //    offspecc = string.Empty;

            //var offspecToAssign = this.GetCharacterOffspecRole(ctx, className, offspecc);

            //if (!await this.AddCharacterToUser(ctx.User.Id, characterName, serverName, region, className, specName, offspecc))
            //    return;

            //try
            //{
            //    await RemoveClassRoles(ctx, member);
            //    await member.GrantRoleAsync(classToAssign);
            //    await member.GrantRoleAsync(roleToAssign);

            //    if (offspecToAssign != null)
            //        await member.GrantRoleAsync(offspecToAssign);

            //    await ctx.Channel.SendMessageAsync($"New alt for {member.Username}.").ConfigureAwait(false);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
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
    }
}
