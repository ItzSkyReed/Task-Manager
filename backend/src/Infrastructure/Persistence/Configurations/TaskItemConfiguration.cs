using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("task_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.Property(x => x.Name).UseCollation(AppDbContext.CaseInsensetiveCollation).IsRequired().HasMaxLength(200);

        builder.Property(x => x.Description).HasMaxLength(4000);

        builder.Property(x => x.Status).IsRequired();

        builder.Property(x => x.ProjectId).IsRequired();

        builder.Property(x => x.AssigneeId).IsRequired(false);

        // При удалении проекта все задачи удаляются каскадно
        builder.HasOne(x => x.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // При удалении пользователя задача остается в проекте, но лишается ответственного
        builder.HasOne(x => x.Assignee)
            .WithMany()
            .HasForeignKey(x => x.AssigneeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.Name)
            .HasDatabaseName("ix_task_items_name");

        builder.HasIndex(x => x.ProjectId)
            .HasDatabaseName("ix_task_items_project_id");

        builder.HasIndex(x => x.AssigneeId)
            .HasDatabaseName("ix_task_items_assignee_id");
    }
}