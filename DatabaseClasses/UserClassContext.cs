using DatabaseClasses.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClasses
{
    public class UserClassContext : DbContext
    {
        public UserClassContext(DbContextOptions<UserClassContext> options) : base(options)
        {

        }

        public DbSet<UserWoWChar> UserClasses { get; set; }
    }
}
