using Microsoft.EntityFrameworkCore;
using OnlineWallet.DAL.Configuration;
using OnlineWallet.Domain.Enums;
using OnlineWallet.Domain;
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace OnlineWallet.DAL.Context
{
    public class OnlineWalletContext : IdentityDbContext<User>
    {
        public OnlineWalletContext()
        {
        }

        public OnlineWalletContext(DbContextOptions<OnlineWalletContext> options)
        : base(options)
        {
        }

        public DbSet<Balance> Balances { get; set; }

        public DbSet<Profile> Profiles { get; set; }

        public DbSet<Operation> Operations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localdb;Initial Catalog=OnlineWallet2;Persist Security Info=True;User ID=sa;Password=Qwerty123");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));

            modelBuilder.ApplyConfiguration(new ProfileConfiguration());
            modelBuilder.ApplyConfiguration(new OperationConfiguration());
            modelBuilder.ApplyConfiguration(new BalanceConfiguration());

            modelBuilder.Entity<User>().HasData(
                new User[]
                {
                    new User{Id = "15"}
                });
            modelBuilder.Entity<Profile>().HasData(
                new Profile[]
                {
                    new Profile{Id = "2", FirstName = "Nik", LastName = "Keige",
                                BirthDate = DateTime.Now, CreationDate = DateTime.Now, UserId = "15",
                                Email = "maroskin@mail.ru"}
                });
            modelBuilder.Entity<Balance>().HasData(
                new Balance[]
                {
                    new Balance{Id = "1", Amount =123, Description = "abracadabra", UserId = "15" }
                });
            modelBuilder.Entity<Operation>().HasData(
                new Operation[]
                {
                    new Operation{Id = "2", Name = "Credit", Amount = 10, StartDate = DateTime.Now,
                                  FinishDate = DateTime.Now, OperationTypes = OperationType.Income,
                                  OperationCategories= OperationCategory.Credit, Percent = 10,
                                  Description= "abra", BalanceId = "1"}
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}
