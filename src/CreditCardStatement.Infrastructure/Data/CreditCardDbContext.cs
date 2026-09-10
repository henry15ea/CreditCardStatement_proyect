using CreditCardStatement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CreditCardStatement.Infrastructure.Data
{
    public class CreditCardDbContext : DbContext
    {
        public CreditCardDbContext(DbContextOptions<CreditCardDbContext> options) : base(options)
        {
        }

        public DbSet<CardHolder> CardHolders => Set<CardHolder>();
        public DbSet<CreditCard> CreditCards => Set<CreditCard>();
        public DbSet<Transaction> Transactions => Set<Transaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CardHolder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<CreditCard>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CardNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CreditLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CurrentBalance).HasColumnType("decimal(18,2)");
                entity.Property(e => e.InterestRate).HasColumnType("decimal(5,2)");
                entity.Property(e => e.MinimumPaymentRate).HasColumnType("decimal(5,2)");
                entity.HasIndex(e => e.CardNumber).IsUnique();
                entity.HasOne(e => e.CardHolder)
                    .WithMany(h => h.CreditCards)
                    .HasForeignKey(e => e.CardHolderId);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.HasOne(e => e.CreditCard)
                    .WithMany(c => c.Transactions)
                    .HasForeignKey(e => e.CreditCardId);
            });
        }
    }
}
