using Microsoft.EntityFrameworkCore;

namespace PodcastAI.Models
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("");
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Podcast> podcasts { get; set; }
    }
}
