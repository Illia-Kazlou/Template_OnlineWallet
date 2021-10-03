using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineWallet.DAL.Constants;
using OnlineWallet.Domain;
using System;

namespace OnlineWallet.DAL.Configuration
{
    public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
    {
        public void Configure(EntityTypeBuilder<Profile> builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.ToTable(TablesConstants.Profiles, BranchesConstants.Account)
                   .HasKey(profile => profile.Id);

            builder.Property(profile => profile.FirstName)
                   .IsRequired()
                   .HasMaxLength(ConfigurationConstants.SmallTextLength);

            builder.Property(profile => profile.LastName)
                   .IsRequired()
                   .HasMaxLength(ConfigurationConstants.SmallTextLength);

            builder.Property(profile => profile.PhoneNumber)
                   .HasMaxLength(ConfigurationConstants.ShortTextLength);

            builder.Property(profile => profile.Email)
                   .IsRequired()
                   .HasMaxLength(ConfigurationConstants.ShortTextLength);

            builder.Property(profile => profile.BirthDate)
                   .IsRequired()
                   .HasColumnType(ConfigurationConstants.DateFormat);

            builder.Property(profile => profile.CreationDate)
                   .IsRequired()
                   .HasColumnType(ConfigurationConstants.DateFormat);

            builder.HasOne(profile => profile.User)
                   .WithOne(identity => identity.Profile)
                   .HasForeignKey<Profile>(profile => profile.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
