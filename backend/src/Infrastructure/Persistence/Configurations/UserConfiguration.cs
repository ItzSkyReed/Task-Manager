using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.Property(x => x.Email).UseCollation(AppDbContext.CaseInsensetiveCollation).IsRequired().HasMaxLength(255);
        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(255);
        builder.Property(x => x.Name).UseCollation(AppDbContext.CaseInsensetiveCollation).IsRequired().HasMaxLength(100);

        builder.HasIndex(x => x.Email).IsUnique().HasDatabaseName("ix_users_email");
    }
}