using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineWallet.DAL.Constants;
using OnlineWallet.Domain;
using System;

namespace OnlineWallet.DAL.Configuration
{
    public class BalanceConfiguration : IEntityTypeConfiguration<Balance>
    {
        public void Configure(EntityTypeBuilder<Balance> builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.ToTable(TablesConstants.Balances, BranchesConstants.Balance)
                   .HasKey(balance => balance.Id);

            builder.Property(balance => balance.Amount)
                   .IsRequired()
                   .HasMaxLength(ConfigurationConstants.SmallTextLength);

            builder.Property(balance => balance.Description)
                   .IsRequired()
                   .HasMaxLength(ConfigurationConstants.MaxTextLength);

            builder.HasOne(balance => balance.User)
                   .WithOne(user => user.Balance)
                   .HasForeignKey<Balance>(balance => balance.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(balance => balance.Operations)
                   .WithOne(operation => operation.Balance)
                   .HasForeignKey(operation => operation.BalanceId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
