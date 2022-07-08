using DatabaseClasses;
using DatabaseClasses.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.AzureAppServices;
using RoleManager;
using RoleManagerTest;
using RoleManagerTest.Services;
using RoleManagerTest.Services.Interfaces;

class Programm
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("config.json");
        var connString = builder.Configuration.GetConnectionString("databaseConnectionString");

        builder.Services.AddSingleton<IStorageService<UserWoWChar>, UserClassStorageService<UserWoWChar>>();

        //builder.Services.AddDbContext<UserClassContext>(options =>
        //{
        //    options.UseSqlServer(connString,
        //        x => x.MigrationsAssembly("DiscordBot.Migrations"));
        //});

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

        using (ServiceProvider serviceProvider = builder.Services.BuildServiceProvider())
        {
            var bot = new Bot(serviceProvider, builder.Configuration);
            builder.Services.AddSingleton(bot);
        }

        var app = builder.Build();
        await app.RunAsync();
    }

    //public static IHostBuilder CreateHostBuilder(string[] args) =>
    //    Host.CreateDefaultBuilder(args)
    //        .ConfigureWebHostDefaults(webBuilder =>
    //        {
    //            webBuilder.UseStartup<Startup>();
    //        });
}

