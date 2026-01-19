using Domain.Models;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();

    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Accounts");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedNever();

            entity.Property(a => a.IsLocked);

            entity.Property(a => a.Balance)
                .HasColumnName("Balance")
                .HasPrecision(18, 2)
                .HasConversion(
                    b => b.Amount,
                    a => Money.Create(a).Value);

            entity.Property<PinCode?>("_pinCode")
                .HasColumnName("PinCode")
                .HasMaxLength(4)
                .IsRequired(false)
                .HasConversion(
                    p => p == null ? null : p.Value.Value,
                    v => v == null ? null : PinCode.Create(v).Value);

            entity.Property<PasswordHash?>("_password")
                .HasColumnName("PasswordHash")
                .IsRequired(false)
                .HasConversion(
                    h => h == null ? null : h.Value.Value,
                    v => v == null ? null : new PasswordHash(v));
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transactions");

            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id).ValueGeneratedNever();

            entity.Property(t => t.AccountId);
            entity.Property(t => t.Timestamp).IsRequired();

            entity.Property(t => t.Amount)
                .HasConversion(
                    m => m.Amount,
                    d => Money.Create(d).Value)
                .HasPrecision(18, 2);

            entity.HasDiscriminator<string>("TransactionType")
                .HasValue<DepositTransaction>("Deposit")
                .HasValue<WithdrawTransaction>("Withdraw");

            entity.HasIndex(t => new { t.AccountId, t.Timestamp })
                .IsDescending();

            entity.HasIndex(t => t.AccountId);
        });
    }
}