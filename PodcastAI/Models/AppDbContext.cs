using Microsoft.EntityFrameworkCore;

namespace PodcastAI.Models
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("workstation id=podcast_db.mssql.somee.com;packet size=4096;user id=podcastAi_SQLLogin_1;pwd=24asyafazc;data source=podcast_db.mssql.somee.com;persist security info=False;initial catalog=podcast_db;TrustServerCertificate=True");
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Podcast> podcasts { get; set; }
    }
}
