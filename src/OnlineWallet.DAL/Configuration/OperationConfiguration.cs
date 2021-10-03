using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OnlineWallet.DAL.Constants;
using OnlineWallet.Domain;
using OnlineWallet.Domain.Enums;
using System;

namespace OnlineWallet.DAL.Configuration
{
    public class OperationConfiguration : IEntityTypeConfiguration<Operation>
    {
        public void Configure(EntityTypeBuilder<Operation> builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.ToTable(TablesConstants.Operations, BranchesConstants.Balance)
                   .HasKey(operation => operation.Id);

            builder.Property(operation => operation.Name)
                   .IsRequired()
                   .HasMaxLength(ConfigurationConstants.SmallTextLength);

            builder.Property(operation => operation.Amount)
                   .IsRequired()
                   .HasMaxLength(ConfigurationConstants.SmallTextLength);

            builder.Property(operation => operation.StartDate)
                   .IsRequired()
                   .HasColumnType(ConfigurationConstants.DateTimeFormat);

            builder.Property(operation => operation.FinishDate)
                   .IsRequired()
                   .HasColumnType(ConfigurationConstants.DateTimeFormat);

            builder.Property(operation => operation.OperationTypes)
                    .HasConversion(new EnumToNumberConverter<OperationType, int>());

            builder.Property(operation => operation.OperationCategories)
                    .HasConversion(new EnumToNumberConverter<OperationCategory, int>());

            builder.Property(operation => operation.Percent)
                   .HasMaxLength(ConfigurationConstants.ShortTextLength);

            builder.Property(operation => operation.Planned);

            builder.Property(operation => operation.Residue);
            
            builder.Property(operation => operation.Description)
                   .IsRequired()
                   .HasMaxLength(ConfigurationConstants.MaxTextLength);

            builder.HasOne(operation => operation.Balance)
                   .WithMany(balance => balance.Operations)
                   .HasForeignKey(operation => operation.BalanceId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
