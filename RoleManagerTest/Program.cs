using DatabaseClasses.Models;
using RoleManager;
using RoleManagerTest;
using RoleManagerTest.GoogleAPI;
using RoleManagerTest.Services;
using RoleManagerTest.Services.Interfaces;

class Programm
{
    public static async Task Main(string[] args)
    {
        //EnvironmentVariables envVariables = new EnvironmentVariables();
        //envVariables.SetEnvironmentVariables();

        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddSingleton(s => new SpreadsheetService("1m_ymqXxKclliNj-TJBgFhYsM7KoAwxEeVXwYhA7M3Nw", "Characters"))
            .AddTransient<HttpClient>();
        

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
        var googleService = serviceProvider.GetRequiredService<SpreadsheetService>();

        var bot = new Bot(serviceProvider, builder.Configuration, googleService);
        builder.Services.AddSingleton(bot);

        var app = builder.Build();
        await app.RunAsync();

        Console.WriteLine("Bot terminated.");
    }
}

