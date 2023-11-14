using Microsoft.EntityFrameworkCore;
using The_Bank.Models;

namespace The_Bank.Data
{
    public class BankContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<StockPrice> StockPrices { get; set; }
        public DbSet<CryptoPrice> CryptoPrices { get; set; }
        public DbSet<CryptoTransaction> CryptoTransactions { get; set; }
        public DbSet<CryptoInvestment> CryptoInvestments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\.;Initial Catalog=Bank;Integrated Security=True;Pooling=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define relationships here
            modelBuilder.Entity<Account>()
        .Property(a => a.Currency)
        .IsRequired();
            modelBuilder.Entity<StockPrice>()
                .HasOne(sp => sp.Account)
                .WithMany(a => a.StockPrices)
                .HasForeignKey(sp => sp.Id)  // Use the correct property name
                .OnDelete(DeleteBehavior.Restrict); // Choose the appropriate delete behavior

            // Configure the relationship between User and Account
            modelBuilder.Entity<User>()
                .HasMany(u => u.Accounts)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Choose the appropriate delete behavior

            // Add other configurations as needed

            base.OnModelCreating(modelBuilder);
        }
    }
}