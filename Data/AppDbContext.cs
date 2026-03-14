using Microsoft.EntityFrameworkCore;
using HackathonMvcSqlite.Models;

namespace HackathonMvcSqlite.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
        }
    }
}