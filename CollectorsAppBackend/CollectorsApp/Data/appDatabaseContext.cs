using CollectorsApp.Models;
using CollectorsApp.Models.Analytics;
using CollectorsApp.Models.APILogs;
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
        public DbSet<PasswordResetModel> PwdReset { get; set; }
        public DbSet<UserPreferences> UserPreferences { get; set; }
        public DbSet<AdminComment> AdminComments { get; set; }
        public DbSet<APILog> APILogs { get; set; }
        public DbSet<UserConsent> UserConsents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
            
            modelBuilder.Entity<PasswordResetModel>()
                .HasOne<Users>()
                .WithMany()
                .HasForeignKey(id => id.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserPreferences>()
                .HasOne<Users>()
                .WithMany()
                .HasForeignKey(id => id.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserConsent>()
                .HasOne<Users>()
                .WithMany()
                .HasForeignKey(id => id.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
