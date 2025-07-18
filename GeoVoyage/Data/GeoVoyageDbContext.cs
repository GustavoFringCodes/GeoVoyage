using GeoVoyage.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace GeoVoyage.Data
{
    public class GeoVoyageDbContext : DbContext
    {
        public GeoVoyageDbContext(DbContextOptions<GeoVoyageDbContext> options) : base(options)
        {
        }

        // Existing DbSets
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Place> Places { get; set; }

        // New DbSets
        public DbSet<CustomerAccount> CustomerAccounts { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<NewsletterSubscription> NewsletterSubscriptions { get; set; }
        public DbSet<TourPackage> TourPackages { get; set; }
        public DbSet<FAQ> FAQ { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Existing configurations
            modelBuilder.Entity<Dish>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.ImageUrl).HasMaxLength(8000);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Id).ValueGeneratedOnAdd();
                entity.Property(t => t.Name).HasMaxLength(100).IsRequired();
                entity.Property(t => t.Description).HasMaxLength(1000).IsRequired();
                entity.Property(t => t.Country).HasMaxLength(200).IsRequired();
                entity.Property(t => t.ImageUrl).HasMaxLength(8000);
            });

            modelBuilder.Entity<Place>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.Id).ValueGeneratedOnAdd();
                entity.Property(f => f.Name).HasMaxLength(200).IsRequired();
                entity.Property(f => f.Description).HasMaxLength(5000).IsRequired();
                entity.Property(f => f.ImageUrl).HasMaxLength(8000);
            });

            // New configurations
            modelBuilder.Entity<CustomerAccount>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
                entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.ProfileImageUrl).HasMaxLength(200);
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.CustomerName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.PackageType).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Pending");
                entity.Property(e => e.SpecialRequests).HasMaxLength(500);
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(10,2)");
                entity.HasOne(e => e.CustomerAccount).WithMany().HasForeignKey(e => e.CustomerAccountId);
            });

            modelBuilder.Entity<ContactMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Subject).HasMaxLength(200);
                entity.Property(e => e.Message).HasMaxLength(1000).IsRequired();
            });

            modelBuilder.Entity<NewsletterSubscription>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<TourPackage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Duration).HasMaxLength(50);
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Difficulty).HasMaxLength(20);
                entity.Property(e => e.ImageUrl).HasMaxLength(200);
                entity.Property(e => e.IncludedServices).HasMaxLength(500);
            });

            modelBuilder.Entity<FAQ>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Question).HasMaxLength(300).IsRequired();
                entity.Property(e => e.Answer).HasMaxLength(1000).IsRequired();
                entity.Property(e => e.Category).HasMaxLength(50);
            });

            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Token).HasMaxLength(255).IsRequired();
                entity.HasOne(e => e.CustomerAccount).WithMany().HasForeignKey(e => e.CustomerAccountId);
            });

            modelBuilder.Entity<EmailVerificationToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Token).HasMaxLength(255).IsRequired();
                entity.HasOne(e => e.CustomerAccount).WithMany().HasForeignKey(e => e.CustomerAccountId);
            });

            modelBuilder.Entity<UserSession>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.SessionToken).HasMaxLength(255).IsRequired();
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.HasOne(e => e.CustomerAccount).WithMany().HasForeignKey(e => e.CustomerAccountId);
            });
        }
    }
}