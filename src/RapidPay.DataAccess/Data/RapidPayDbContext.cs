using Microsoft.EntityFrameworkCore;
using RapidPay.DataAccess.Entities;

namespace RapidPay.DataAccess.Data
{
    public class RapidPayDbContext : DbContext, IDataAccessLayer
    {
        public RapidPayDbContext(DbContextOptions<RapidPayDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Card>()
            .ToTable("Card")
            .HasIndex(u => u.Number)
                .IsUnique();

            modelBuilder.Entity<PaymentHistory>()
            .ToTable("PaymentHistory");
        }

        public DbSet<Card> Cards { get; set; }
        public DbSet<PaymentHistory> PaymentHistories { get; set; }
    }
}
