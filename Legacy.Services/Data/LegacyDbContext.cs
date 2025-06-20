﻿using Legacy.Services.Models;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<RegionsModel> Regions { get; set; }
        public DbSet<AmenitiesModel> Amenities { get; set; }
        public DbSet<PicsModel> Pics { get; set; }
        public DbSet<BrioClubLeadsModel> BrioClubLeads { get; set; }
        public DbSet<AccessDevelopmentMemberAuditModel> AccessDevelopmentMemberAudit { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
