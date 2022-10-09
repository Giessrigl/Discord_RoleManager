using DatabaseClasses.Models;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using RoleManagerTest.GoogleAPI.Interfaces;
using System.Collections.ObjectModel;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;

namespace RoleManagerTest.GoogleAPI
{
    public class MainCharacterSpreadsheetService : ISpreadsheetService, ICharacterRemover
    {
        private readonly SheetsService sheetService;

        private readonly string spreadsheetID;
        private readonly string tableName;
        private string range;

        public MainCharacterSpreadsheetService(GoogleSheetServiceContainer sheetService)
        {
            this.spreadsheetID = "1m_ymqXxKclliNj-TJBgFhYsM7KoAwxEeVXwYhA7M3Nw";
            this.tableName = "Characters";
            this.sheetService = sheetService.GetSheetService().GetAwaiter().GetResult();
            this.range = $"{this.tableName}!A2:G";
        }

        public async Task<IList<IList<object>>> GetSheetData()
        {
            var req = this.sheetService.Spreadsheets.Values.Get(spreadsheetID, range);
            var res = await req.ExecuteAsync();
            if (res.Values == null || res.Values.Count == 0)
            {
                Console.WriteLine("No entries found.");
                return Array.Empty<object[]>();
            }

            return res.Values;
        }

        public async Task<SpreadsheetCharacter> GetCharacterOfUser(string discordID)
        {
            var data = await GetSheetData();
            var characterData = SpreadsheetMapper.MapFromRangeData(data);

            for (int i = 0; i < characterData.Length; i++)
            {
                if (data[i] != null && characterData[i].DiscordID == discordID)
                    return characterData[i];
            }

            return null;
        }

        public async Task<bool> IsCharacterRegistered(string charName, string server, string region)
        {
            var data = await GetSheetData();
            var characterData = SpreadsheetMapper.MapFromRangeData(data);

            for (int i = 0; i < characterData.Length; i++)
            {
                if (characterData[i] != null &&
                    characterData[i].CharName == charName &&
                    characterData[i].ServerName == server &&
                    characterData[i].Region == region)
                    return true;
            }

            return false;
        }

        public async Task AddCharacter(SpreadsheetCharacter character)
        {
            var data = await this.GetSheetData();
            var characterData = SpreadsheetMapper.MapFromRangeData(data);

            for (int i = 0; i < characterData.Length; i++)
            {
                if (characterData[i] == null)
                {
                    await UpdateCharacter(character, i);
                    return;
                }

                if (characterData[i].Equals(character))
                {
                    await UpdateCharacter(character, i);
                    return;
                }
            }

            await PostCharacter(character);
        }

        private async Task PostCharacter(SpreadsheetCharacter character)
        {
            var valueRange = new ValueRange
            {
                Values = SpreadsheetMapper.MapToRangeData(character)
            };
            var appendRequest = sheetService.Spreadsheets.Values.Append(valueRange, spreadsheetID, range);
            appendRequest.ValueInputOption = AppendRequest.ValueInputOptionEnum.USERENTERED;
            var res = await appendRequest.ExecuteAsync();

        }

        private async Task UpdateCharacter(SpreadsheetCharacter character, int row)
        {
            var range = $"{tableName}!A{row + 2}:G{row + 2}";
            var valueRange = new ValueRange
            {
                Values = SpreadsheetMapper.MapToRangeData(character)
            };
            var updateRequest = sheetService.Spreadsheets.Values.Update(valueRange, spreadsheetID, range);
            updateRequest.ValueInputOption = UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var res = await updateRequest.ExecuteAsync();
        }

        private async Task<int> GetSpreadsheetRowOfMember(string discordID)
        {
            var data = await GetSheetData();
            var characterData = SpreadsheetMapper.MapFromRangeData(data);

            for (int i = 0; i < characterData.Length; i++)
            {
                if (characterData[i] != null && characterData[i].DiscordID == discordID)
                    return i;
            }

            return -1;
        }

        public async Task DeleteChars(DiscordClient client, GuildMemberRemoveEventArgs args)
        {
            var userID = args.Member.Id;
            var row = await GetSpreadsheetRowOfMember(userID.ToString());
            if (row < 0)
                return;

            var range = $"{tableName}!A{row + 2}:G{row + 2}";
            var requestBody = new ClearValuesRequest();
            var res = await this.sheetService.Spreadsheets.Values.Clear(requestBody, spreadsheetID, range).ExecuteAsync();
        }

        // NOT TO USE RIGHT NOW!
        public async Task DeleteChar(DiscordClient client, string charName)
        {
            throw new NotImplementedException();
        }
    }
}
