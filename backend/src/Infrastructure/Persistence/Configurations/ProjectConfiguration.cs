using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.Property(x => x.Name).UseCollation(AppDbContext.CaseInsensetiveCollation).IsRequired().HasMaxLength(200);

        builder.Property(x => x.Description).HasMaxLength(5000);

        builder.Property(x => x.CreatedByUserId).IsRequired(false);

        // При удалении пользователя-создателя сам проект остается в системе, но без владельца
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.Name)
            .HasDatabaseName("ix_projects_name");

        builder.HasIndex(x => x.CreatedByUserId)
            .HasDatabaseName("ix_projects_created_by_user_id");
    }
}