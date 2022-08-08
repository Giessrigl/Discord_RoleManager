using DatabaseClasses.Models;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;

namespace RoleManagerTest.GoogleAPI
{
    public class SpreadsheetService
    {
        private readonly SheetsService sheetService;

        private readonly string spreadsheetID;
        private readonly string tableName;
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string ApplicationName = Environment.GetEnvironmentVariable("PROJECT_NAME");

        public SpreadsheetService()
        {
            this.spreadsheetID = "1m_ymqXxKclliNj-TJBgFhYsM7KoAwxEeVXwYhA7M3Nw";
            this.tableName = "Characters";
            this.sheetService = GetSheetService().GetAwaiter().GetResult();
        }

        private async Task<SheetsService> GetSheetService()
        {
            UserCredential credential;   
            try
            {
                ClientSecrets secrets = new ClientSecrets()
                {
                    ClientId = Environment.GetEnvironmentVariable("GoogleID"),
                    ClientSecret = Environment.GetEnvironmentVariable("GoogleSecret")
                };

                // Load client secrets.
                //using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                //{
                    /* The file token.json stores the user's access and refresh tokens, and is created
                     automatically when the authorization flow completes for the first time. */
                    string credPath = "token.json";
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        //GoogleClientSecrets.FromStream(stream).Secrets,
                        secrets,
                        Scopes,
                        "Bot",
                        CancellationToken.None,
                        new FileDataStore(credPath, true));
                //}
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            // Create Google Sheets API service.
            return new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
        }

        //private SheetsService GetSheetServiceTest()
        //{
        //    GoogleCredential credential;
        //    try
        //    {
        //        credential = GoogleCredential.FromFile("credentials.json").CreateScoped(Scopes);

        //        var service = new SheetsService(new BaseClientService.Initializer()
        //        {
        //            HttpClientInitializer = credential,
        //            ApplicationName = ApplicationName,
        //        });
        //    }
        //    catch (FileNotFoundException e)
        //    {
        //        Console.WriteLine(e.Message);
        //        return null;
        //    }

        //    // Create Google Sheets API service.
        //    return new SheetsService(new BaseClientService.Initializer
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = ApplicationName
        //    });
        //}

        public async Task<SpreadsheetCharacter[]> GetSheetData()
        {
            var range = $"{tableName}!A2:F";

            var req = this.sheetService.Spreadsheets.Values.Get(spreadsheetID, range);
            var res = await req.ExecuteAsync();
            if (res.Values == null || res.Values.Count == 0)
            {
                Console.WriteLine("No entries found.");
                return new SpreadsheetCharacter[0];
            }

            return SpreadsheetMapper.MapFromRangeData(res.Values);
        }

        public async Task<SpreadsheetCharacter> GetCharacterOfUser(string discordID)
        {
            var data = await GetSheetData();

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != null && data[i].DiscordID == discordID)
                    return data[i];
            }

            return null;
        }

        public async Task<bool> IsCharacterRegistered(string charName, string server, string region)
        {
            var data = await GetSheetData();

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != null && 
                    data[i].CharName == charName && 
                    data[i].ServerName == server && 
                    data[i].Region == region)
                    return true;
            }

            return false;
        }

        public async Task AddCharacter(SpreadsheetCharacter character)
        {
            var data = await this.GetSheetData();

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == null)
                {
                    await UpdateCharacter(character, i);
                    return;
                }

                if (data[i].Equals(character))
                {
                    await UpdateCharacter(character, i);
                    return;
                }
            }

            await PostCharacter(character);
        }

        private async Task PostCharacter(SpreadsheetCharacter character)
        {
            var range = $"{tableName}!A:F";
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
            var range = $"{tableName}!A{row + 2}:F{row + 2}";
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

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != null && data[i].DiscordID == discordID)
                    return i;
            }

            return -1;
        }

        public async Task DeleteChar(DiscordClient client, GuildMemberRemoveEventArgs args)
        {
            var userID = args.Member.Id;
            var row = await GetSpreadsheetRowOfMember(userID.ToString());
            if (row < 0)
                return;

            var range = $"{tableName}!A{row + 2}:F{row + 2}";
            var requestBody = new ClearValuesRequest();
            var res = await this.sheetService.Spreadsheets.Values.Clear(requestBody, spreadsheetID, range).ExecuteAsync();
        }
    }
}
