using Microsoft.EntityFrameworkCore;
using The_Bank.Models;

namespace The_Bank.Data
{
    public class BankContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<StockPrice> StockPrices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\.;Initial Catalog=Bank;Integrated Security=True;Pooling=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define relationships here
            modelBuilder.Entity<StockPrice>()
                .HasOne(sp => sp.User)
                .WithMany(u => u.StockPrices)
                .HasForeignKey(sp => sp.Id)
                .OnDelete(DeleteBehavior.Restrict); // Choose the appropriate delete behavior

            // Add other configurations as needed

            base.OnModelCreating(modelBuilder);
        }
    }
}