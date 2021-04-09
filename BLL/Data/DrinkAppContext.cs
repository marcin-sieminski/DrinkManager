using BLL.Admin.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BLL.Data
{
    public class DrinkAppContext : IdentityDbContext
    {
        public DrinkAppContext(DbContextOptions<DrinkAppContext> options) : base(options)
        {
        }

        public new DbSet<AppUser> Users { get; set; }
        public DbSet<Drink> Drinks { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<DrinkReview> Reviews { get; set; }
        public DbSet<UserDrink> UserDrinks { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserDrink>()
                .HasKey(cs => new { cs.AppUserId, cs.DrinkId });

            modelBuilder.Entity<DrinkReview>()
                .HasOne(d => d.Drink)
                .WithMany(d => d.DrinkReviews)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DrinkReview>()
                .HasOne(d => d.Author)
                .WithMany(d => d.Reviews)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}