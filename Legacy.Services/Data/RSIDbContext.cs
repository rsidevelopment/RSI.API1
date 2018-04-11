using Legacy.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Legacy.Services.Data
{
    public class RSIDbContext : DbContext
    {
        public RSIDbContext(DbContextOptions<RSIDbContext> options)
            : base(options)
        { }

        public DbSet<MemberModel> Users { get; set; }
        public DbSet<HoldModel> Holds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
