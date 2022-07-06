using DatabaseClasses;
using Microsoft.EntityFrameworkCore;
using RoleManager;

namespace RoleManagerTest
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<UserClassContext>(options => 
            {
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=UserClassContext;Trusted_Connection=True;MultipleActiveResultSets=true",
                    x => x.MigrationsAssembly("DiscordBot.Migrations"));
            });

            var serviceProvider = services.BuildServiceProvider();

            var bot = new Bot(serviceProvider);
            services.AddSingleton(bot);
            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
