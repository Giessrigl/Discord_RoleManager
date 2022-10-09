using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;

namespace RoleManagerTest.GoogleAPI
{
    public class GoogleSheetServiceContainer
    {
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string ApplicationName = Environment.GetEnvironmentVariable("PROJECT_NAME");

        private SheetsService sheetService;

        public GoogleSheetServiceContainer()
        {
            new Action(async () =>
            {
                this.sheetService = await BuildSheetService();
            }).Invoke();
        }

        public async Task<SheetsService> GetSheetService()
        {
            if (sheetService == null)
                this.sheetService = await BuildSheetService();

            return this.sheetService;
        }

        private async Task<SheetsService> BuildSheetService()
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
                    new FileDataStore(credPath));
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
    }
}
