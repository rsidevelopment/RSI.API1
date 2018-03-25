using Legacy.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legacy.Services.Data
{
    public class LegacyDbContext : DbContext
    {
        public LegacyDbContext(DbContextOptions<LegacyDbContext> options)
            : base(options)
        { }
        
        public DbSet<MemberModel> Users { get; set; }
        public DbSet<OrganizationModel> Organizations { get; set; }
        public DbSet<RenewalInfoModel> RenewalInfo { get; set; }
        public DbSet<TranslatorModel> Translators { get; set; }
        public DbSet<ReferralModel> Referrals { get; set; }
        public DbSet<UpgradeAuditModel> UpgradeAudits { get; set; }
        public DbSet<InventoryModel> Inventories { get; set; }
        public DbSet<UnitModel> Units { get; set; }
        public DbSet<ReservationModel> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
