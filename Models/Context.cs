using Microsoft.EntityFrameworkCore;

namespace TestApi.Models
{
    public class Context : DbContext
    {
        public DbSet<FileModel> Files { get; set; }

        public Context(DbContextOptions<Context> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}