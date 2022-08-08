using RoleManager;
using RoleManagerTest.GoogleAPI;

class Programm
{
    public static async Task Main(string[] args)
    {
        //EnvironmentVariables envVariables = new EnvironmentVariables();
        //envVariables.SetEnvironmentVariables();

        var builder = WebApplication.CreateBuilder(args);

        var googleService = new SpreadsheetService();

        builder.Services
            .AddSingleton(googleService)
            .AddSingleton<HttpClient>();
        
        //builder.Services.AddLogging(config =>
        //{
        //    config.ClearProviders();
        //    config.AddDebug();
        //    config.AddConsole();
        //    config.AddAzureWebAppDiagnostics();
        //}).Configure<AzureFileLoggerOptions>(options =>
        //{
        //    options.FileName = "first-azure-log";
        //    options.FileSizeLimit = 50 * 1024;
        //    options.RetainedFileCountLimit = 10;
        //});

        ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();

        var bot = new Bot(serviceProvider, builder.Configuration, googleService);
        builder.Services.AddSingleton(bot);

        var app = builder.Build();
        await app.RunAsync();

        Console.WriteLine("Bot terminated.");
    }
}

