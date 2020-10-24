using Microsoft.EntityFrameworkCore;

namespace BLL.Data
{
    public class DrinkAppContext : DbContext
    {
        public DrinkAppContext(DbContextOptions<DrinkAppContext> options) : base(options)
        {
        }

        public DbSet<Drink> Drinks { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<DrinkReview> DrinkReviews { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Drink>().HasMany(i => i.Ingredients);
        }
    }
}