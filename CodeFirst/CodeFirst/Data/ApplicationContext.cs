using Microsoft.EntityFrameworkCore;
using CodeFirst.Services;

namespace CodeFirst.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<CorporateClient> CorporateClients { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Ledger> Ledgers { get; set; }
        public DbSet<OneTimePayment> OneTimePayments { get; set; }
        public DbSet<PersonalClient> PersonalClients { get; set; }
        public DbSet<Software> Softwares { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<User> Users { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", PasswordHash = PasswordHasher.HashPassword("admin123"), Role = "Admin" },
                new User { Id = 2, Username = "user", PasswordHash = PasswordHasher.HashPassword("user123"), Role = "User" }
            );
            
            modelBuilder.Entity<Client>()
                .Property(c => c.PhoneNumber)
                .HasColumnType("varchar(15)");

            modelBuilder.Entity<Contract>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CorporateClient>()
                .Property(c => c.KRS)
                .HasColumnType("decimal(20,0)");

            modelBuilder.Entity<Ledger>()
                .Property(l => l.AmountPaid)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PersonalClient>()
                .Property(p => p.PESEL)
                .HasColumnType("decimal(11,0)");
            
            modelBuilder.Entity<OneTimePayment>()
                .HasKey(o => o.IdContract);

            modelBuilder.Entity<OneTimePayment>()
                .HasOne(o => o.Contract)
                .WithOne(c => c.OneTimePayment)
                .HasForeignKey<OneTimePayment>(o => o.IdContract)
                .OnDelete(DeleteBehavior.Restrict);
            
            
            modelBuilder.Entity<Client>().HasData(
                new Client { IdClient = 1, Address = "123 Main St", Email = "client1@example.com", PhoneNumber = "1234567890" },
                new Client { IdClient = 2, Address = "456 Elm St", Email = "client2@example.com", PhoneNumber = "9876543210" }
            );

            modelBuilder.Entity<CorporateClient>().HasData(
                new CorporateClient { IdClient = 1, CorpoName = "Corp1", KRS = 123456789m }
            );

            modelBuilder.Entity<PersonalClient>().HasData(
                new PersonalClient { IdClient = 2, Name = "John", Surname = "Doe", PESEL = 89012345678m }
            );

            modelBuilder.Entity<Software>().HasData(
                new Software { IdSoftware = 1, Name = "Software1", Description = "Description1", Version = "1.0", Type = "Type1" }
            );

            modelBuilder.Entity<Contract>().HasData(
                new Contract { IdContract = 1, IdClient = 1, IdSoftware = 1, Name = "Contract1", DateFrom = DateOnly.FromDateTime(DateTime.Now), Price = 1000m }
            );

            modelBuilder.Entity<Ledger>().HasData(
                new Ledger { IdPayment = 1, IdContract = 1, AmountPaid = 500m }
            );

            modelBuilder.Entity<Discount>().HasData(
                new Discount { IdDiscount = 1, Name = "Discount1", Offer = "Offer1", Amt = 10, DateFrom = DateOnly.FromDateTime(DateTime.Now), DateTo = DateOnly.FromDateTime(DateTime.Now.AddMonths(1)) }
            );

            modelBuilder.Entity<OneTimePayment>().HasData(
                new OneTimePayment { IdContract = 1, Version = "1.0", DateTo = DateOnly.FromDateTime(DateTime.Now.AddYears(1)), Status = "Active", UpdatePeriod = 12 }
            );

            modelBuilder.Entity<Subscription>().HasData(
                new Subscription { IdContract = 1, RenevalTimeInMonths = 12 }
            );
        }
    }
}
