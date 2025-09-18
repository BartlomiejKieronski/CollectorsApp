using CollectorsApp.Models;
using CollectorsApp.Models.Analytics;
using CollectorsApp.Models.APILogs;
using CollectorsApp.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Data
{
    public class appDatabaseContext : DbContext
    {
        public appDatabaseContext(DbContextOptions<appDatabaseContext> options) : base(options)
        {

        }
        public DbSet<CollectableItems> CollectableItems { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<ImagePath> ImagePaths { get; set; }
        public DbSet<RefreshTokenInfo> RefreshTokens { get; set; }
        public DbSet<Collections> Collections { get; set; }
        public DbSet<PasswordReset> PwdReset { get; set; }
        public DbSet<UserPreferences> UserPreferences { get; set; }
        public DbSet<AdminComment> AdminComments { get; set; }
        public DbSet<APILog> APILogs { get; set; }
        public DbSet<UserConsent> UserConsents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var property = entityType.FindProperty("TimeStamp");
                if( property!=null && property.ClrType == typeof(DateTime))
                {
                    property.SetDefaultValueSql("CURRENT_TIMESTAMP");
                    property.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
                }
            }

            modelBuilder.Entity<ImagePath>()
                .HasOne<CollectableItems>()
                .WithMany()              
                .HasForeignKey(id => id.ItemId)  
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Collections>()
                .HasOne<Users>()
                .WithMany()
                .HasForeignKey(id => id.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CollectableItems>()
                .HasOne<Users>()
                .WithMany()
                .HasForeignKey(id => id.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CollectableItems>()
                .HasOne<Collections>()
                .WithMany()
                .HasForeignKey(id => id.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RefreshTokenInfo>()
                .HasOne<Users>()
                .WithMany()
                .HasForeignKey(id => id.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ImagePath>()
                .HasOne<Users>()
                .WithMany()
                .HasForeignKey(id => id.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<PasswordReset>()
                .HasOne<Users>()
                .WithMany()
                .HasForeignKey(id => id.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPreferences>()
                .HasOne<Users>()
                .WithMany()
                .HasForeignKey(id => id.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPreferences>()
                .Property(u => u.Theme)
                .HasMaxLength(32)
                .HasDefaultValue("Dark");

            modelBuilder.Entity<UserPreferences>()
                .Property(u => u.Layout)
                .HasMaxLength(32)
                .HasDefaultValue("Classic");

            modelBuilder.Entity<UserPreferences>()
                .Property(u => u.ItemsPerPage)
                .HasDefaultValue(20);

            modelBuilder.Entity<UserPreferences>()
                .Property(u => u.Pagination)
                .HasDefaultValue(true);

            modelBuilder.Entity<UserConsent>()
                .HasOne<Users>()
                .WithMany()
                .HasForeignKey(id => id.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Users>()
                .Property(u=>u.Role)
                .HasMaxLength(32)
                .HasDefaultValue("user");
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is ILastUpdated &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                ((ILastUpdated)entry.Entity).LastUpdated = DateTime.UtcNow;
            }
        }
    }
}
