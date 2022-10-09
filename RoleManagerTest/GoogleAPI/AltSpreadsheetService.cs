using DSharpPlus;
using DSharpPlus.EventArgs;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using RoleManagerTest.GoogleAPI.Interfaces;

namespace RoleManagerTest.GoogleAPI
{
    public class AltSpreadsheetService : ISpreadsheetService, ICharacterRemover
    {
        private readonly SheetsService sheetService;

        private readonly string spreadsheetID;
        private readonly string tableName;
        private string range;

        public AltSpreadsheetService(GoogleSheetServiceContainer sheetService)
        {
            this.spreadsheetID = "1m_ymqXxKclliNj-TJBgFhYsM7KoAwxEeVXwYhA7M3Nw";
            this.tableName = "alts";
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

        public async Task DeleteChars(DiscordClient client, GuildMemberRemoveEventArgs args)
        {
            var userID = args.Member.Id;
            var rows = await GetSpreadsheetRowsOfMember(userID.ToString());
            if (rows.Count <= 0)
                return;

            var requestBody = new ClearValuesRequest();
            foreach (var row in rows)
            {
                var range = $"{tableName}!A{row + 2}:G{row + 2}";
                var res = await this.sheetService.Spreadsheets.Values.Clear(requestBody, spreadsheetID, range).ExecuteAsync();
            }
        }

        public async Task DeleteChar(DiscordClient client, string charName)
        {
            var data = await GetSheetData();
            var characters = SpreadsheetMapper.MapFromRangeData(data);

            var requestBody = new ClearValuesRequest();
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i] != null && characters[i].CharName == charName && characters[i].DiscordID == client.CurrentUser.Id.ToString())
                {
                    var range = $"{tableName}!A{i + 2}:G{i + 2}";
                    var res = await this.sheetService.Spreadsheets.Values.Clear(requestBody, spreadsheetID, range).ExecuteAsync();
                }
            }
        }

        private async Task<List<int>> GetSpreadsheetRowsOfMember(string discordID)
        {
            var data = await GetSheetData();
            var characterData = SpreadsheetMapper.MapFromRangeData(data);

            List<int> rows = new();

            for (int i = 0; i < characterData.Length; i++)
            {
                if (characterData[i] != null && characterData[i].DiscordID == discordID)
                    rows.Add(i);
            }

            return rows;
        }
    }
}
